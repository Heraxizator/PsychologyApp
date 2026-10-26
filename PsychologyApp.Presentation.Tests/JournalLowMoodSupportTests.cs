using PsychologyApp.Presentation.Features.ManageJournal;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>What the journal offers after a low check-in.</summary>
public sealed class JournalLowMoodSupportTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void A_low_mood_today_gets_a_practice_and_help(int mood)
    {
        Assert.Equal(JournalLowMoodSupportKind.PracticesAndHelp, JournalLowMoodSupport.For(isToday: true, mood, noteLooksLikeCrisis: false));
    }

    [Theory]
    [InlineData(true, 3)]
    [InlineData(true, 5)]
    [InlineData(false, 1)]
    public void A_mood_that_is_not_low_or_a_past_day_gets_nothing(bool isToday, int mood)
    {
        Assert.Equal(JournalLowMoodSupportKind.None, JournalLowMoodSupport.For(isToday, mood, noteLooksLikeCrisis: false));
    }

    // In a crisis suggestions are not shown, whatever the mood: only the way to help (a practice offered there would read as dismissive).
    [Theory]
    [InlineData(true, 1)]
    [InlineData(true, 4)]
    [InlineData(false, 2)]
    public void A_note_that_reads_like_a_crisis_gets_only_help(bool isToday, int mood)
    {
        Assert.Equal(JournalLowMoodSupportKind.HelpOnly, JournalLowMoodSupport.For(isToday, mood, noteLooksLikeCrisis: true));
    }
}
