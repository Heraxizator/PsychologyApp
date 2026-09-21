using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntimeGenAI;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// On-device LLM through ONNX Runtime GenAI. Loads any GenAI-format model (a folder with <c>genai_config.json</c>) found in
/// <c>llm/</c> under the app data directory, or under the app's external files directory (reachable with <c>adb push</c>).
/// Fully offline: no network access anywhere in this class.
/// The model is loaded lazily (or via <see cref="WarmUpAsync"/>) because loading takes seconds and hundreds of MB of RAM.
/// </summary>
public sealed class OnnxGenAiLanguageModel(ILogger<OnnxGenAiLanguageModel> logger) : ILocalLanguageModel, IDisposable
{
    public const string ModelFolderName = "llm";
    private const string ConfigFileName = "genai_config.json";
    private const int MinAndroidApi = 24;

    private readonly SemaphoreSlim _gate = new(1, 1);
    private Model? _model;
    private Tokenizer? _tokenizer;
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
            return await Task.Run(() => Generate(request, cancellationToken), cancellationToken).ConfigureAwait(false);
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

    public void Dispose()
    {
        _tokenizer?.Dispose();
        _model?.Dispose();
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

    private string? Generate(LlmRequest request, CancellationToken cancellationToken)
    {
        EnsureLoaded();
        if (_model is null || _tokenizer is null)
        {
            return null;
        }

        string prompt = _tokenizer.ApplyChatTemplate(string.Empty, BuildMessagesJson(request), string.Empty, true);
        using Sequences input = _tokenizer.Encode(prompt);

        using GeneratorParams options = new(_model);
        options.SetSearchOption("max_length", input[0].Length + request.MaxNewTokens);
        options.SetSearchOption("do_sample", true);
        options.SetSearchOption("temperature", request.Temperature);
        options.SetSearchOption("top_p", 0.9);

        using Generator generator = new(_model, options);
        generator.AppendTokenSequences(input);
        using TokenizerStream stream = _tokenizer.CreateStream();

        StringBuilder output = new();
        while (!generator.IsDone())
        {
            cancellationToken.ThrowIfCancellationRequested();
            generator.GenerateNextToken();
            ReadOnlySpan<int> sequence = generator.GetSequence(0);
            output.Append(stream.Decode(sequence[^1]));
        }

        return output.ToString().Trim();
    }

    private void EnsureLoaded()
    {
        if (_model is not null || _loadFailed)
        {
            return;
        }

        string? directory = FindModelDirectory();
        if (directory is null)
        {
            return;
        }

        try
        {
            _model = new Model(directory);
            _tokenizer = new Tokenizer(_model);
            logger.LogInformation("On-device model loaded from {Directory}", directory);
        }
        catch (Exception ex)
        {
            _loadFailed = true;
            logger.LogError(ex, "Could not load the on-device model from {Directory}", directory);
            _tokenizer?.Dispose();
            _model?.Dispose();
            _tokenizer = null;
            _model = null;
        }
    }

    /// <summary>Hand-written JSON: reflection-based serialization is not trim-safe in release builds.</summary>
    private static string BuildMessagesJson(LlmRequest request)
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter writer = new(buffer))
        {
            writer.WriteStartArray();
            WriteMessage(writer, "system", request.SystemPrompt);
            foreach (LlmMessage message in request.Messages)
            {
                WriteMessage(writer, message.Role == LlmRole.User ? "user" : "assistant", message.Content);
            }

            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private static void WriteMessage(Utf8JsonWriter writer, string role, string content)
    {
        writer.WriteStartObject();
        writer.WriteString("role", role);
        writer.WriteString("content", content);
        writer.WriteEndObject();
    }
}
