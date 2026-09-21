using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.ML.OnnxRuntimeGenAI;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;

// Usage: dotnet run -c Release --project tools/PsychologyApp.LlmEval -- <model-dir> [report.md] [--temperature 0.5]
// Runs the companion prompt set through a real GenAI-format model with exactly the app's prompt, guard and analyzer,
// and writes a markdown report for human review (psychologist / native speaker).

if (args.Length < 1 || !File.Exists(Path.Combine(args[0], "genai_config.json")))
{
    Console.Error.WriteLine("Usage: LlmEval <model-dir containing genai_config.json> [report.md] [--temperature 0.5]");
    return 1;
}

string modelDir = args[0];
string reportPath = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : "llm-eval-report.md";
float? temperature = null;
int tempIndex = Array.IndexOf(args, "--temperature");
if (tempIndex >= 0 && tempIndex + 1 < args.Length)
{
    temperature = float.Parse(args[tempIndex + 1], System.Globalization.CultureInfo.InvariantCulture);
}

EvalPrompt[] prompts = JsonSerializer.Deserialize<EvalPrompt[]>(
    File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "prompts.json")),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

Console.WriteLine($"Loading model from {modelDir} ...");
Stopwatch load = Stopwatch.StartNew();
using DesktopModel model = new(modelDir);
model.Load();
Console.WriteLine($"Loaded in {load.Elapsed.TotalSeconds:F1}s");

LexiconSituationAnalyzer analyzer = new();
KeywordCrisisDetector crisis = new();

StringBuilder report = new();
report.AppendLine("# Companion LLM evaluation");
report.AppendLine();
report.AppendLine($"- Model: `{modelDir}`");
report.AppendLine($"- Temperature override: {temperature?.ToString() ?? "none (app default)"}");
report.AppendLine($"- Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
report.AppendLine();

int analyzerHits = 0, analyzerTotal = 0, accepted = 0, generated = 0, crisisHits = 0, crisisTotal = 0;
double totalSeconds = 0;

foreach (EvalPrompt prompt in prompts)
{
    bool english = prompt.Lang == "en";
    report.AppendLine($"## {prompt.Text}");
    report.AppendLine();

    bool isCrisisCase = prompt.Expected == "Crisis";
    bool crisisDetected = crisis.IsCrisis(prompt.Text);
    if (isCrisisCase)
    {
        crisisTotal++;
        crisisHits += crisisDetected ? 1 : 0;
        report.AppendLine($"- Expected: crisis. Detector: **{(crisisDetected ? "caught (model not called)" : "MISSED")}**");
        report.AppendLine();
        continue;
    }

    SituationAnalysis analysis = analyzer.Analyze(prompt.Text);
    analyzerTotal++;
    bool hit = prompt.Expected.Split('|').Contains(analysis.Emotion.ToString()); // "A|B" marks genuinely ambiguous messages
    analyzerHits += hit ? 1 : 0;

    LlmRequest request = CompanionPromptBuilder.Build(
        english,
        [new LlmMessage(LlmRole.User, prompt.Text)],
        analysis);
request = temperature is { } t ? request with { Temperature = t } : request;

    Stopwatch sw = Stopwatch.StartNew();
    string? raw = await model.GenerateAsync(request);
    sw.Stop();

    string? safe = CompanionReplyGuard.Sanitize(raw, english, prompt.Text);
    generated++;
    totalSeconds += sw.Elapsed.TotalSeconds;
    accepted += safe is not null ? 1 : 0;

    report.AppendLine($"- Analyzer: **{analysis.Emotion}** (expected {prompt.Expected}) {(hit ? "OK" : "MISS")}; themes: {string.Join(", ", analysis.Themes)}");
    report.AppendLine($"- Time: {sw.Elapsed.TotalSeconds:F1}s");
    report.AppendLine($"- Guard: **{(safe is not null ? "accepted" : "REJECTED -> scripted fallback")}**");
    report.AppendLine();
    report.AppendLine($"> Raw: {raw?.Replace("\n", " ")}");
    report.AppendLine();
    if (safe is not null)
    {
        report.AppendLine($"> Shown to user: {safe}");
        report.AppendLine();
    }

    Console.WriteLine($"[{prompt.Expected,-14}] {sw.Elapsed.TotalSeconds,5:F1}s  {(safe is not null ? "ok  " : "REJ ")} {Trim(raw)}");
}

report.Insert(0,
    $"# Summary{Environment.NewLine}{Environment.NewLine}" +
    $"- Analyzer accuracy: {analyzerHits}/{analyzerTotal}{Environment.NewLine}" +
    $"- Crisis phrases caught: {crisisHits}/{crisisTotal}{Environment.NewLine}" +
    $"- Model replies accepted by the guard: {accepted}/{generated}{Environment.NewLine}" +
    $"- Mean generation time: {(generated == 0 ? 0 : totalSeconds / generated):F1}s{Environment.NewLine}{Environment.NewLine}");

File.WriteAllText(reportPath, report.ToString());
Console.WriteLine();
Console.WriteLine($"Analyzer {analyzerHits}/{analyzerTotal}, crisis {crisisHits}/{crisisTotal}, guard accepted {accepted}/{generated}, mean {(generated == 0 ? 0 : totalSeconds / generated):F1}s");
Console.WriteLine($"Report: {Path.GetFullPath(reportPath)}");
return 0;

static string Trim(string? text) =>
    text is null ? "(null)" : (text.Replace("\n", " ").Length > 110 ? text.Replace("\n", " ")[..110] + "..." : text.Replace("\n", " "));

internal sealed record EvalPrompt(string Lang, string Expected, string Text);

/// <summary>Desktop twin of the Android adapter: same prompt formatting and sampling options, so results transfer (speed does not).</summary>
internal sealed class DesktopModel(string directory) : IDisposable
{
    private Model? _model;
    private Tokenizer? _tokenizer;

    public void Load()
    {
        _model = new Model(directory);
        _tokenizer = new Tokenizer(_model);
    }

    public Task<string?> GenerateAsync(LlmRequest request)
    {
        string prompt = _tokenizer!.ApplyChatTemplate(string.Empty, BuildMessagesJson(request), string.Empty, true);
        using Sequences input = _tokenizer.Encode(prompt);

        using GeneratorParams options = new(_model!);
        options.SetSearchOption("max_length", input[0].Length + request.MaxNewTokens);
        options.SetSearchOption("do_sample", true);
        options.SetSearchOption("temperature", request.Temperature);
        options.SetSearchOption("top_p", 0.9);

        using Generator generator = new(_model!, options);
        generator.AppendTokenSequences(input);
        using TokenizerStream stream = _tokenizer.CreateStream();

        StringBuilder output = new();
        while (!generator.IsDone())
        {
            generator.GenerateNextToken();
            ReadOnlySpan<int> sequence = generator.GetSequence(0);
            output.Append(stream.Decode(sequence[^1]));
        }

        return Task.FromResult<string?>(output.ToString().Trim());
    }

    public void Dispose()
    {
        _tokenizer?.Dispose();
        _model?.Dispose();
    }

    private static string BuildMessagesJson(LlmRequest request)
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter writer = new(buffer))
        {
            writer.WriteStartArray();
            Write(writer, "system", request.SystemPrompt);
            foreach (LlmMessage message in request.Messages)
            {
                Write(writer, message.Role == LlmRole.User ? "user" : "assistant", message.Content);
            }

            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    private static void Write(Utf8JsonWriter writer, string role, string content)
    {
        writer.WriteStartObject();
        writer.WriteString("role", role);
        writer.WriteString("content", content);
        writer.WriteEndObject();
    }
}
