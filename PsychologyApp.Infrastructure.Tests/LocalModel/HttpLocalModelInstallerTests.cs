using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Infrastructure.LocalModel;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.LocalModel;

public sealed class HttpLocalModelInstallerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "model-installer-" + Guid.NewGuid().ToString("N"));

    private static readonly byte[] ConfigBytes = "{\"model\":\"x\"}"u8.ToArray();
    private static readonly byte[] WeightsBytes = Enumerable.Range(0, 5000).Select(i => (byte)(i % 251)).ToArray();

    private sealed class FakeServer(bool honourRange = true) : HttpMessageHandler
    {
        public Dictionary<string, byte[]> Files { get; } = new()
        {
            ["https://example.test/genai_config.json"] = ConfigBytes,
            ["https://example.test/model.onnx.data"] = WeightsBytes
        };

        public List<(string Url, RangeHeaderValue? Range)> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string url = request.RequestUri!.ToString();
            Requests.Add((url, request.Headers.Range));

            if (!Files.TryGetValue(url, out byte[]? body))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            long? from = request.Headers.Range?.Ranges.FirstOrDefault()?.From;
            if (honourRange && from is long start)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.PartialContent) { Content = new ByteArrayContent(body[(int)start..]) });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(body) });
        }
    }

    private static LocalModelManifest Manifest(string? weightsSha = null, long? weightsSize = null) => new(
        "test-model",
        "Test model",
        "https://example.test/license",
        [
            new LocalModelFile("genai_config.json", "https://example.test/genai_config.json", ConfigBytes.Length),
            new LocalModelFile("model.onnx.data", "https://example.test/model.onnx.data", weightsSize ?? WeightsBytes.Length, weightsSha ?? Convert.ToHexString(SHA256.HashData(WeightsBytes)))
        ],
        ["en"]);

    private string Target => Path.Combine(_root, "llm");

    private HttpLocalModelInstaller Create(FakeServer server, LocalModelManifest? manifest = null) =>
        new(new HttpClient(server), manifest ?? Manifest(), Target);

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [Fact]
    public async Task Installs_all_files_verifies_them_and_reports_full_progress()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server);
        List<ModelInstallProgress> reports = [];

        await installer.InstallAsync(new Progress<ModelInstallProgress>(reports.Add));
        await Task.Delay(50);

        Assert.True(installer.IsInstalled);
        Assert.Equal(WeightsBytes, await File.ReadAllBytesAsync(Path.Combine(Target, "model.onnx.data")));
        Assert.Equal(ConfigBytes, await File.ReadAllBytesAsync(Path.Combine(Target, "genai_config.json")));
        Assert.False(Directory.Exists(Target + ".partial"));
        Assert.Equal(1.0, reports[^1].Fraction);
    }

    [Fact]
    public async Task Already_installed_model_makes_no_requests()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server);
        await installer.InstallAsync();
        server.Requests.Clear();

        await installer.InstallAsync();

        Assert.Empty(server.Requests);
    }

    [Fact]
    public async Task Interrupted_download_resumes_with_a_range_request()
    {
        FakeServer server = new();
        string staging = Target + ".partial";
        Directory.CreateDirectory(staging);
        await File.WriteAllBytesAsync(Path.Combine(staging, "model.onnx.data.part"), WeightsBytes[..2000]);
        HttpLocalModelInstaller installer = Create(server);

        await installer.InstallAsync();

        (string Url, RangeHeaderValue? Range) weights = server.Requests.Single(r => r.Url.EndsWith("model.onnx.data"));
        Assert.Equal(2000, weights.Range!.Ranges.Single().From);
        Assert.Equal(WeightsBytes, await File.ReadAllBytesAsync(Path.Combine(Target, "model.onnx.data")));
    }

    [Fact]
    public async Task Server_that_ignores_range_restarts_the_file_instead_of_corrupting_it()
    {
        FakeServer server = new(honourRange: false);
        string staging = Target + ".partial";
        Directory.CreateDirectory(staging);
        await File.WriteAllBytesAsync(Path.Combine(staging, "model.onnx.data.part"), WeightsBytes[..2000]);
        HttpLocalModelInstaller installer = Create(server);

        await installer.InstallAsync();

        Assert.Equal(WeightsBytes, await File.ReadAllBytesAsync(Path.Combine(Target, "model.onnx.data")));
    }

    [Fact]
    public async Task Checksum_mismatch_discards_the_file_and_leaves_the_model_uninstalled()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server, Manifest(weightsSha: new string('0', 64)));

        IOException error = await Assert.ThrowsAsync<IOException>(() => installer.InstallAsync());

        Assert.Contains("integrity", error.Message);
        Assert.False(installer.IsInstalled);
        Assert.False(Directory.Exists(Target));
        Assert.False(File.Exists(Path.Combine(Target + ".partial", "model.onnx.data.part")));
    }

    [Fact]
    public async Task Wrong_size_is_rejected()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server, Manifest(weightsSize: WeightsBytes.Length + 10));

        await Assert.ThrowsAsync<IOException>(() => installer.InstallAsync());

        Assert.False(installer.IsInstalled);
    }

    [Fact]
    public async Task Http_error_leaves_the_model_uninstalled()
    {
        FakeServer server = new();
        server.Files.Remove("https://example.test/model.onnx.data");
        HttpLocalModelInstaller installer = Create(server);

        await Assert.ThrowsAsync<HttpRequestException>(() => installer.InstallAsync());

        Assert.False(installer.IsInstalled);
        Assert.False(Directory.Exists(Target));
    }

    [Fact]
    public async Task Cancellation_stops_the_install_without_marking_it_installed()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server);
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => installer.InstallAsync(cancellationToken: cts.Token));

        Assert.False(installer.IsInstalled);
    }

    [Fact]
    public async Task Manifest_that_escapes_the_model_folder_is_refused()
    {
        FakeServer server = new();
        server.Files["https://example.test/evil"] = [1, 2, 3];
        LocalModelManifest evil = new("evil", "Evil", "x", [new LocalModelFile("../outside.bin", "https://example.test/evil", 3)], ["en"]);
        HttpLocalModelInstaller installer = Create(server, evil);

        await Assert.ThrowsAsync<InvalidOperationException>(() => installer.InstallAsync());

        Assert.False(File.Exists(Path.Combine(_root, "outside.bin")));
    }

    [Fact]
    public async Task Changed_manifest_makes_the_installed_copy_outdated()
    {
        FakeServer server = new();
        await Create(server).InstallAsync();

        HttpLocalModelInstaller newer = Create(server, Manifest() with { Id = "test-model-v2" });

        Assert.False(newer.IsInstalled);
    }

    [Fact]
    public async Task Delete_removes_installed_and_partial_data()
    {
        FakeServer server = new();
        HttpLocalModelInstaller installer = Create(server);
        await installer.InstallAsync();
        Directory.CreateDirectory(Target + ".partial");

        await installer.DeleteAsync();

        Assert.False(installer.IsInstalled);
        Assert.False(Directory.Exists(Target));
        Assert.False(Directory.Exists(Target + ".partial"));
    }
}
