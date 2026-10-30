using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Infrastructure.LocalModel;

/// <summary>
/// Resumable, verified, atomic model download. Files go to <c>{target}.partial</c> (resuming with HTTP Range),
/// each is checked for size and SHA-256, and only when everything is present the marker is written and the folder promoted
/// to <c>{target}</c>. A half-downloaded model can therefore never be picked up by the runtime.
/// </summary>
public sealed class HttpLocalModelInstaller(HttpClient http, LocalModelManifest manifest, string targetDirectory) : ILocalModelInstaller
{
    private const string MarkerFile = ".installed";
    private const int BufferSize = 128 * 1024;
    private static readonly TimeSpan ProgressInterval = TimeSpan.FromMilliseconds(250);

    private string StagingDirectory => targetDirectory + ".partial";
    private string PreviousDirectory => targetDirectory + ".previous";

    public LocalModelManifest Manifest => manifest;

    public bool IsInstalled
    {
        get
        {
            string marker = Path.Combine(targetDirectory, MarkerFile);
            if (!File.Exists(marker) || File.ReadAllText(marker).Trim() != Fingerprint(manifest))
            {
                return false;
            }

            return manifest.Files.All(file =>
            {
                string path = SafePath(targetDirectory, file.Path);
                return File.Exists(path) && new FileInfo(path).Length == file.SizeBytes;
            });
        }
    }

    public async Task InstallAsync(IProgress<ModelInstallProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        RestorePreviousIfTargetMissing();
        if (IsInstalled)
        {
            TryDeletePrevious();
            return;
        }

        Directory.CreateDirectory(StagingDirectory);
        long done = 0;
        ProgressReporter reporter = new(progress, manifest.TotalBytes);

        foreach (LocalModelFile file in manifest.Files)
        {
            string finalPath = SafePath(StagingDirectory, file.Path);
            if (File.Exists(finalPath) && new FileInfo(finalPath).Length == file.SizeBytes)
            {
                try
                {
                    await VerifyAsync(file, finalPath, cancellationToken).ConfigureAwait(false);
                    done += file.SizeBytes;
                    reporter.Report(done, file.Path, force: true);
                    continue;
                }
                catch (IOException) when (!File.Exists(finalPath))
                {
                    // A same-sized file from an older manifest failed its checksum and was discarded; fetch this version again.
                }
            }

            await DownloadFileAsync(file, finalPath, done, reporter, cancellationToken).ConfigureAwait(false);
            done += file.SizeBytes;
        }

        await File.WriteAllTextAsync(Path.Combine(StagingDirectory, MarkerFile), Fingerprint(manifest), cancellationToken).ConfigureAwait(false);

        bool movedPrevious = false;
        if (Directory.Exists(targetDirectory))
        {
            DeletePreviousIfPresent();
            Directory.Move(targetDirectory, PreviousDirectory);
            movedPrevious = true;
        }

        try
        {
            Directory.Move(StagingDirectory, targetDirectory);
        }
        catch
        {
            if (movedPrevious && !Directory.Exists(targetDirectory) && Directory.Exists(PreviousDirectory))
            {
                Directory.Move(PreviousDirectory, targetDirectory);
            }

            throw;
        }

        TryDeletePrevious();
        reporter.Report(manifest.TotalBytes, string.Empty, force: true);
    }

    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        foreach (string directory in new[] { targetDirectory, StagingDirectory, PreviousDirectory })
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }

        return Task.CompletedTask;
    }

    private void RestorePreviousIfTargetMissing()
    {
        if (!Directory.Exists(targetDirectory) && Directory.Exists(PreviousDirectory))
        {
            Directory.Move(PreviousDirectory, targetDirectory);
        }
    }

    private void DeletePreviousIfPresent()
    {
        if (Directory.Exists(PreviousDirectory))
        {
            Directory.Delete(PreviousDirectory, recursive: true);
        }
    }

    private void TryDeletePrevious()
    {
        try
        {
            DeletePreviousIfPresent();
        }
        catch (IOException)
        {
            // Keep the old directory as a recovery copy if the filesystem cannot remove it now.
        }
        catch (UnauthorizedAccessException)
        {
            // A cleanup failure must not make a fully installed model unusable.
        }
    }

    private async Task DownloadFileAsync(
        LocalModelFile file,
        string finalPath,
        long alreadyDone,
        ProgressReporter reporter,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(finalPath)!);
        string partPath = finalPath + ".part";
        long existing = File.Exists(partPath) ? new FileInfo(partPath).Length : 0;
        if (existing > file.SizeBytes)
        {
            File.Delete(partPath);
            existing = 0;
        }

        if (existing < file.SizeBytes)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, file.Url);
            if (existing > 0)
            {
                request.Headers.Range = new RangeHeaderValue(existing, null);
            }

            using HttpResponseMessage response = await http
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // A server that ignores Range answers 200 with the whole body: start over instead of appending.
            bool resumed = existing > 0 && response.StatusCode == HttpStatusCode.PartialContent;
            if (!resumed)
            {
                existing = 0;
            }

            await using Stream body = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using FileStream output = new(partPath, resumed ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, useAsync: true);

            byte[] buffer = new byte[BufferSize];
            long written = existing;
            int read;
            while ((read = await body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (read > file.SizeBytes - written)
                {
                    await output.DisposeAsync().ConfigureAwait(false);
                    File.Delete(partPath);
                    throw new InvalidDataException($"'{file.Path}' exceeded its expected size; the partial file was discarded.");
                }

                await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                written += read;
                reporter.Report(alreadyDone + written, file.Path, force: false);
            }
        }

        await VerifyAsync(file, partPath, cancellationToken).ConfigureAwait(false);
        File.Move(partPath, finalPath, overwrite: true);
    }

    private static async Task VerifyAsync(LocalModelFile file, string path, CancellationToken cancellationToken)
    {
        long length = new FileInfo(path).Length;
        if (length != file.SizeBytes)
        {
            File.Delete(path);
            throw new IOException($"'{file.Path}' has {length} bytes, expected {file.SizeBytes}. The partial file was discarded.");
        }

        if (file.Sha256 is null)
        {
            return;
        }

        string actual;
        await using (FileStream stream = File.OpenRead(path))
        {
            actual = Convert.ToHexString(await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false));
        }

        // The stream must be closed before deleting: an open handle blocks File.Delete on Windows.
        if (!actual.Equals(file.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(path);
            throw new IOException($"'{file.Path}' failed the integrity check. The file was discarded; please retry.");
        }
    }

    private static string Fingerprint(LocalModelManifest manifest) =>
        Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(
            manifest.Id + "|" + string.Join('|', manifest.Files.Select(f => $"{f.Path}:{f.SizeBytes}:{f.Sha256}")))));

    /// <summary>Keeps manifest paths inside the staging folder (no "../" escapes).</summary>
    private static string SafePath(string root, string relative)
    {
        string full = Path.GetFullPath(Path.Combine(root, relative));
        if (!full.StartsWith(Path.GetFullPath(root) + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Manifest path '{relative}' escapes the model folder.");
        }

        return full;
    }

    private sealed class ProgressReporter(IProgress<ModelInstallProgress>? progress, long total)
    {
        private DateTime _last = DateTime.MinValue;

        public void Report(long downloaded, string file, bool force)
        {
            if (progress is null)
            {
                return;
            }

            DateTime now = DateTime.UtcNow;
            if (!force && now - _last < ProgressInterval)
            {
                return;
            }

            _last = now;
            progress.Report(new ModelInstallProgress(downloaded, total, file));
        }
    }
}
