using System.Text.Json;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>Optional local evaluation for consented, anonymised multi-turn conversations; never prints user text.</summary>
public sealed class RealDialoguesTests(ITestOutputHelper output)
{
    private sealed record Turn(
        string? Text,
        string? Intent = null,
        string? Emotion = null,
        string? Goal = null,
        string? Event = null,
        string? Action = null,
        bool? QuestionPending = null,
        bool? OffersPractice = null);

    private sealed record Dialogue(string? Lang, Turn[]? Turns);

    [Fact]
    public void Real_dialogues_match_their_independent_intent_and_context_labels()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Conversation", "Data", "real", "dialogues"));
        if (!Directory.Exists(dir))
        {
            output.WriteLine("No consented real dialogue cases found; evaluation skipped.");
            return;
        }

        Dialogue[] dialogues = [.. Directory.GetFiles(dir, "*.json")
            .OrderBy(path => path, StringComparer.Ordinal)
            .SelectMany(path => JsonSerializer.Deserialize<Dialogue[]>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [])];
        if (dialogues.Length == 0)
        {
            output.WriteLine("No consented real dialogue cases found; evaluation skipped.");
            return;
        }

        LexiconSituationAnalyzer analyzer = new();
        int checks = 0;
        List<string> misses = [];
        for (int index = 0; index < dialogues.Length; index++)
        {
            Dialogue sample = dialogues[index];
            int checksBeforeDialogue = checks;
            if (sample.Lang is not ("ru" or "en") || sample.Turns is not { Length: > 0 })
            {
                misses.Add($"dialogue {index + 1}: invalid language or empty turns");
                continue;
            }

            bool english = sample.Lang.Equals("en", StringComparison.OrdinalIgnoreCase);
            CompanionDialogue engine = new(analyzer, new KeywordCrisisDetector(), english, new Random(17));
            CompanionState state = engine.Open(new CompanionState(), previous: null).State;

            for (int turnIndex = 0; turnIndex < sample.Turns.Length; turnIndex++)
            {
                Turn turn = sample.Turns[turnIndex];
                if (string.IsNullOrWhiteSpace(turn.Text))
                {
                    misses.Add($"dialogue {index + 1}, turn {turnIndex + 1}: missing text");
                    continue;
                }

                SituationAnalysis analysis = analyzer.Analyze(turn.Text);
                CompanionReply reply = engine.Respond(state, new CompanionInput.FreeText(turn.Text));
                state = reply.State;

                Check(turn.Intent, UtteranceClassifier.Classify(turn.Text, analysis.Emotion != CompanionEmotion.Unknown).ToString(), "intent");
                Check(turn.Emotion, state.Emotion, "emotion");
                CheckNullable(turn.Goal, state.Goal, "goal");
                CheckNullable(turn.Event, state.Event, "event");
                Check(turn.Action, reply.Action?.Kind.ToString() ?? "None", "action");
                Check(turn.QuestionPending, state.QuestionPending, "questionPending");
                bool offersPractice = reply.QuickReplies.Any(quickReply =>
                    quickReply.Kind == ChatQuickReplyKinds.Practice
                    || (quickReply.Kind == ChatQuickReplyKinds.Act && quickReply.Payload == "practice"));
                Check(turn.OffersPractice, offersPractice, "offersPractice");

                void Check<T>(T? expected, T actual, string field)
                {
                    if (expected is null)
                    {
                        return;
                    }

                    checks++;
                    if (!EqualityComparer<T>.Default.Equals(expected, actual))
                    {
                        misses.Add($"dialogue {index + 1}, turn {turnIndex + 1}, {field}: expected {expected}, got {actual}");
                    }
                }

                void CheckNullable(string? expected, string? actual, string field)
                {
                    if (expected is null)
                    {
                        return;
                    }

                    string? expectedValue = expected.Equals("None", StringComparison.OrdinalIgnoreCase) ? null : expected;
                    checks++;
                    if (!string.Equals(expectedValue, actual, StringComparison.Ordinal))
                    {
                        misses.Add($"dialogue {index + 1}, turn {turnIndex + 1}, {field}: expected {expected}, got {actual ?? "None"}");
                    }
                }
            }

            if (checks == checksBeforeDialogue)
            {
                misses.Add($"dialogue {index + 1}: no expected labels");
            }
        }

        output.WriteLine($"Real dialogue cases: {dialogues.Length}; labeled checks: {checks}; mismatches: {misses.Count}.");
        misses.ForEach(output.WriteLine);
        Assert.Empty(misses);
    }
}
