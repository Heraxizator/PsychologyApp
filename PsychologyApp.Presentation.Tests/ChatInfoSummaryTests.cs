using PsychologyApp.Application.Chat;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ChatInfoSummaryTests
{
    private static ChatSessionDTO Session(CompanionState state, string? emotion = "Anxiety", int? first = 7, int? last = 4) => new()
    {
        Id = 1,
        Title = "Work",
        Emotion = emotion,
        FirstIntensity = first,
        LastIntensity = last,
        StateJson = state.Serialize()
    };

    [Fact]
    public void A_chat_shows_its_feeling_tension_practice_and_step()
    {
        ChatInfoSummary summary = ChatInfoSummary.From(Session(new CompanionState
        {
            LastPractice = nameof(TechniqueId.Grounding),
            HelpedPractices = [nameof(TechniqueId.Breathing)],
            NewPlan = "  Write three questions before 10  "
        }));

        Assert.Equal("Anxiety", summary.Emotion);
        Assert.Equal("7 → 4", summary.TensionText);
        Assert.Equal(TechniqueId.Breathing, summary.Practice);
        Assert.Equal("Write three questions before 10", summary.Step);
        Assert.False(summary.IsEmpty);
    }

    // A chat that only said hello must not show rows of dashes: every part it never reached is left out.
    [Fact]
    public void A_chat_that_never_got_going_has_nothing_to_show()
    {
        ChatInfoSummary summary = ChatInfoSummary.From(Session(new CompanionState(), emotion: "Unknown", first: null, last: null));

        Assert.Null(summary.Emotion);
        Assert.Null(summary.TensionText);
        Assert.Null(summary.Practice);
        Assert.Null(summary.Step);
        Assert.True(summary.IsEmpty);
    }

    [Theory]
    [InlineData(6, 6, "6")]
    [InlineData(5, null, "5")]
    [InlineData(null, 3, "3")]
    public void Tension_rated_once_or_unchanged_shows_one_number(int? first, int? last, string expected) =>
        Assert.Equal(expected, ChatInfoSummary.From(Session(new CompanionState(), first: first, last: last)).TensionText);

    // A state the chat recognises but the page cannot name would show an empty "Feeling" row.
    [Fact]
    public void Every_recognised_feeling_has_a_name()
    {
        foreach (Application.Conversation.Companion.CompanionEmotion emotion in Enum.GetValues<Application.Conversation.Companion.CompanionEmotion>())
        {
            string? name = AppStrings.ChatEmotion(emotion.ToString());
            if (emotion == Application.Conversation.Companion.CompanionEmotion.Unknown)
            {
                Assert.Null(name);
            }
            else
            {
                Assert.False(string.IsNullOrWhiteSpace(name), emotion.ToString());
            }
        }
    }

    [Fact]
    public void An_unknown_practice_name_is_left_out_rather_than_guessed() =>
        Assert.Null(ChatInfoSummary.From(Session(new CompanionState { LastPractice = "Retired" })).Practice);
}
