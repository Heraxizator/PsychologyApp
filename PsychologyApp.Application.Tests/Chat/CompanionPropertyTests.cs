using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>Properties that must hold for ANY text a person can type, not just the examples someone thought of.</summary>
public class CompanionPropertyTests
{
    private static CompanionDialogue Create(bool english) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(7));

    private static CompanionReply Say(CompanionDialogue d, CompanionState s, string text) =>
        d.Respond(s, new CompanionInput.FreeText(text));

    [Property(MaxTest = 500)]
    public bool Any_text_never_throws_and_yields_a_wellformed_reply(string? text, bool english)
    {
        CompanionReply reply = Say(Create(english), new CompanionState(), text ?? string.Empty);

        return reply.Messages.All(m => !string.IsNullOrWhiteSpace(m))
            && reply.QuickReplies.All(q => !string.IsNullOrWhiteSpace(q.Label))
            && reply.State.Turns >= 0
            && reply.State.UnclearStreak >= 0;
    }

    [Property(MaxTest = 300)]
    public bool A_non_blank_message_always_gets_an_answer(NonEmptyString text)
    {
        string typed = text.Get;
        if (string.IsNullOrWhiteSpace(typed))
        {
            return true;
        }

        CompanionReply reply = Say(Create(false), new CompanionState(), typed);

        // Either something is said, or the engine did not take the message as input at all (never for non-blank text).
        return reply.Messages.Count > 0;
    }

    [Property(MaxTest = 300)]
    public bool The_state_survives_serialisation_after_any_message(string? text)
    {
        CompanionReply reply = Say(Create(false), new CompanionState(), text ?? string.Empty);

        CompanionState restored = CompanionState.Deserialize(reply.State.Serialize());

        return restored.Turns == reply.State.Turns
            && restored.Emotion == reply.State.Emotion
            && restored.UnclearStreak == reply.State.UnclearStreak
            && restored.QuestionPending == reply.State.QuestionPending
            && restored.RecentTexts.SequenceEqual(reply.State.RecentTexts);
    }

    [Property(MaxTest = 300)]
    public bool Digit_strings_never_count_as_a_turn_when_nothing_was_asked(PositiveInt number)
    {
        string digits = new string('1', 4 + number.Get % 8);

        CompanionReply reply = Say(Create(false), new CompanionState(), digits);

        return reply.State.Turns == 0 && reply.State.RecentTexts.Count == 0;
    }

    [Property(MaxTest = 300)]
    public bool Crisis_wording_is_found_whatever_surrounds_it(string? before, string? after, bool english)
    {
        string marker = english ? "I want to kill myself" : "я хочу покончить с собой";
        string text = $"{before} {marker} {after}";

        CompanionReply reply = Say(Create(english), new CompanionState(), text);

        return reply.Action?.Kind == DialogueActionKind.OpenCrisisHub;
    }

    [Property(MaxTest = 200)]
    public bool Noise_leaves_the_known_feeling_untouched(PositiveInt length)
    {
        CompanionDialogue d = Create(false);
        CompanionState known = Say(d, new CompanionState(), "мне очень тревожно из-за работы").State;
        string mash = new string('ы', 3 + length.Get % 10);

        CompanionReply reply = Say(d, known, mash);

        return reply.State.Emotion == known.Emotion && reply.State.Theme == known.Theme;
    }

    [Property(MaxTest = 300)]
    public bool The_crisis_detector_never_throws_and_ignores_blank_text(string? text)
    {
        KeywordCrisisDetector detector = new();
        bool result = detector.IsCrisis(text);

        return !string.IsNullOrWhiteSpace(text) || !result;
    }
}
