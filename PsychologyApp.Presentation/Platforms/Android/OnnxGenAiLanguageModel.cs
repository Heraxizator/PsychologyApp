using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.LocalLlm;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// Android adapter for the on-device LLM (<see cref="OnnxGenAiEngine"/>). Finds a GenAI-format model folder
/// (<c>genai_config.json</c>) in <c>llm/</c> under the app data directory, or under the app's external files directory
/// (reachable with <c>adb push</c>). Fully offline: no network access anywhere in this class.
/// The model is loaded lazily (or via <see cref="WarmUpAsync"/>) because loading takes seconds and gigabytes of RAM.
/// </summary>
public sealed class OnnxGenAiLanguageModel(ILogger<OnnxGenAiLanguageModel> logger) : ILocalLanguageModel, IDisposable
{
    public const string ModelFolderName = "llm";
    private const string ConfigFileName = "genai_config.json";
    private const int MinAndroidApi = 24;

    private readonly SemaphoreSlim _gate = new(1, 1);
    private OnnxGenAiEngine? _engine;
    private bool _loadFailed;

    public bool IsAvailable =>
        OperatingSystem.IsAndroidVersionAtLeast(MinAndroidApi) && !_loadFailed && FindModelDirectory() is not null;

    public async Task WarmUpAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return;
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await Task.Run(EnsureLoaded, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            return null;
        }

        // One generation at a time: the model is not re-entrant and phones have little spare memory.
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await Task.Run(() =>
            {
                EnsureLoaded();
                return _engine?.Generate(request, cancellationToken);
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "On-device generation failed");
            return null;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ReleaseAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            _engine?.Dispose();
            _engine = null;
            _loadFailed = false;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        _engine?.Dispose();
        _gate.Dispose();
    }

    /// <summary>Folders searched for an installed model, in priority order.</summary>
    internal static IEnumerable<string> CandidateDirectories()
    {
        yield return Path.Combine(FileSystem.AppDataDirectory, ModelFolderName);

        string? external = global::Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
        if (external is not null)
        {
            yield return Path.Combine(external, ModelFolderName);
        }
    }

    private static string? FindModelDirectory() =>
        CandidateDirectories().FirstOrDefault(dir => File.Exists(Path.Combine(dir, ConfigFileName)));

    private void EnsureLoaded()
    {
        if (_engine is not null || _loadFailed)
        {
            return;
        }

        string? directory = FindModelDirectory();
        if (directory is null)
        {
            return;
        }

        OnnxGenAiEngine engine = new(directory);
        try
        {
            engine.Load();
            _engine = engine;
            logger.LogInformation("On-device model loaded from {Directory}", directory);
        }
        catch (Exception ex)
        {
            _loadFailed = true;
            engine.Dispose();
            logger.LogError(ex, "Could not load the on-device model from {Directory}", directory);
        }
    }
}
