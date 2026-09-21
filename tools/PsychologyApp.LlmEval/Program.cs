using System.Diagnostics;
using System.Text;
using System.Text.Json;
using PsychologyApp.LocalLlm;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;

// Usage: dotnet run -c Release --project tools/PsychologyApp.LlmEval -- <model-dir> [report.md] [--temperature 0.3] [--max-tokens 70] [--limit 6]
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

int? maxTokens = ReadIntOption("--max-tokens");
int? limit = ReadIntOption("--limit");
int? ReadIntOption(string name)
{
    int at = Array.IndexOf(args, name);
    return at >= 0 && at + 1 < args.Length ? int.Parse(args[at + 1]) : null;
}

EvalPrompt[] prompts = JsonSerializer.Deserialize<EvalPrompt[]>(
    File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "prompts.json")),
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

Console.WriteLine($"Loading model from {modelDir} ...");
Stopwatch load = Stopwatch.StartNew();
using OnnxGenAiEngine model = new(modelDir);
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

foreach (EvalPrompt prompt in limit is { } n ? prompts.Where(p => p.Expected != "Crisis").Take(n).ToArray() : prompts)
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
request = maxTokens is { } m ? request with { MaxNewTokens = m } : request;

    Stopwatch sw = Stopwatch.StartNew();
    string raw = model.Generate(request, CancellationToken.None);
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

    Console.WriteLine($"[{prompt.Expected,-14}] {sw.Elapsed.TotalSeconds,5:F1}s  {(safe is not null ? "ok  " : "REJ ")} {raw.Replace('\n', ' ')}");
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

