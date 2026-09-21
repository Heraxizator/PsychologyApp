using System.Text;
using System.Text.Json;
using Microsoft.ML.OnnxRuntimeGenAI;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.LocalLlm;

/// <summary>
/// Synchronous generation core on top of ONNX Runtime GenAI, shared by the Android adapter and the evaluation tool so that
/// evaluation results describe exactly what ships (same prompt formatting, sampling and per-family quirks).
/// Not thread-safe: callers serialise access.
/// </summary>
public sealed class OnnxGenAiEngine(string modelDirectory) : IDisposable
{
    private Model? _model;
    private Tokenizer? _tokenizer;
    private ModelQuirks _quirks = ModelQuirks.None;

    public bool IsLoaded => _model is not null;

    public string ModelDirectory => modelDirectory;

    /// <summary>Loads weights and tokenizer (seconds, hundreds of MB to GB of RAM). Throws if the model folder is unusable.</summary>
    public void Load()
    {
        if (_model is not null)
        {
            return;
        }

        _quirks = ModelQuirks.ForModelType(ReadModelType(modelDirectory));
        _model = new Model(modelDirectory);
        _tokenizer = new Tokenizer(_model);
    }

    /// <summary>Generates a reply, calling <paramref name="onText"/> with each newly decoded piece of text as it appears.</summary>
    public string Generate(LlmRequest request, CancellationToken cancellationToken, Action<string>? onText = null)
    {
        Load();

        string prompt = _tokenizer!.ApplyChatTemplate(string.Empty, BuildMessagesJson(request, _quirks), string.Empty, true);
        using Sequences input = _tokenizer.Encode(prompt);

        using GeneratorParams options = new(_model!);
        options.SetSearchOption("max_length", input[0].Length + request.MaxNewTokens);
        options.SetSearchOption("do_sample", true);
        options.SetSearchOption("temperature", request.Temperature);
        options.SetSearchOption("top_p", 0.9);
        // The bundled configs default to top_k = 1 (greedy) which makes small models loop; let top_p do the sampling.
        options.SetSearchOption("top_k", 40);
        options.SetSearchOption("repetition_penalty", 1.1);

        using Generator generator = new(_model!, options);
        generator.AppendTokenSequences(input);
        using TokenizerStream stream = _tokenizer.CreateStream();

        StringBuilder output = new();
        while (!generator.IsDone())
        {
            cancellationToken.ThrowIfCancellationRequested();
            generator.GenerateNextToken();
            ReadOnlySpan<int> sequence = generator.GetSequence(0);
            string piece = stream.Decode(sequence[^1]);
            if (piece.Length > 0)
            {
                output.Append(piece);
                onText?.Invoke(piece);
            }
        }

        return output.ToString();
    }

    public void Dispose()
    {
        _tokenizer?.Dispose();
        _model?.Dispose();
        _tokenizer = null;
        _model = null;
    }

    /// <summary>Hand-written JSON: reflection-based serialization is not trim-safe in release builds.</summary>
    internal static string BuildMessagesJson(LlmRequest request, ModelQuirks quirks)
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter writer = new(buffer))
        {
            writer.WriteStartArray();
            WriteMessage(writer, "system", request.SystemPrompt + quirks.SystemSuffix);
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

    private static string? ReadModelType(string directory)
    {
        try
        {
            using JsonDocument config = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "genai_config.json")));
            return config.RootElement.GetProperty("model").GetProperty("type").GetString();
        }
        catch (Exception ex) when (ex is IOException or JsonException or KeyNotFoundException)
        {
            return null;
        }
    }
}

/// <summary>Model-family specifics that the generic chat template cannot express.</summary>
public sealed record ModelQuirks(string SystemSuffix)
{
    public static ModelQuirks None { get; } = new(string.Empty);

    /// <summary>Qwen3 starts with a long "&lt;think&gt;" monologue unless told otherwise; "/no_think" is its documented soft switch.</summary>
    public static ModelQuirks Qwen3 { get; } = new(" /no_think");

    public static ModelQuirks ForModelType(string? modelType) => modelType switch
    {
        "qwen3" => Qwen3,
        _ => None
    };
}
