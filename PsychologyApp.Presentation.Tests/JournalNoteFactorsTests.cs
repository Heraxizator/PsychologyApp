using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class JournalNoteFactorsTests
{
    [Fact]
    public void ToggleFactor_AddsAndRemovesWithoutDuplicates()
    {
        string note = JournalNoteFactors.ToggleFactor(string.Empty, JournalNoteFactors.SleepKey);
        Assert.True(JournalNoteFactors.HasFactor(note, JournalNoteFactors.SleepKey));
        Assert.Contains(AppStrings.JournalFactorSleep.Trim(), note, StringComparison.Ordinal);

        string again = JournalNoteFactors.ToggleFactor(note, JournalNoteFactors.SleepKey);
        Assert.False(JournalNoteFactors.HasFactor(again, JournalNoteFactors.SleepKey));

        string withPrompt = JournalNoteFactors.ToggleFactor(
            AppStrings.JournalPromptHelped,
            JournalNoteFactors.PeopleKey);
        Assert.True(JournalNoteFactors.HasFactor(withPrompt, JournalNoteFactors.PeopleKey));
        Assert.Contains(AppStrings.JournalPromptHelped, withPrompt, StringComparison.Ordinal);

        string toggledTwice = JournalNoteFactors.ToggleFactor(withPrompt, JournalNoteFactors.PeopleKey);
        toggledTwice = JournalNoteFactors.ToggleFactor(toggledTwice, JournalNoteFactors.PeopleKey);
        Assert.Equal(1, toggledTwice.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Count(line => line.StartsWith(AppStrings.JournalFactorPeople, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void Analyze_ReturnsCountAndAverage_WhenActivityPresent()
    {
        string sleepNote = JournalNoteFactors.ToggleFactor(string.Empty, JournalNoteFactors.SleepKey);
        string walkNote = JournalNoteFactors.ToggleFactor(string.Empty, JournalNoteFactors.WalkKey);

        IReadOnlyList<JournalActivityInsight> insights = JournalNoteFactors.Analyze(
        [
            (sleepNote, 4),
            (sleepNote, 2),
            (walkNote, 5)
        ]);

        JournalActivityInsight sleep = Assert.Single(insights, item => item.Key == JournalNoteFactors.SleepKey);
        Assert.Equal(2, sleep.Count);
        Assert.Equal(AppStrings.FormatAverageMood(3), sleep.AverageMoodDisplay);

        JournalActivityInsight walk = Assert.Single(insights, item => item.Key == JournalNoteFactors.WalkKey);
        Assert.Equal(1, walk.Count);
        Assert.Null(walk.AverageMoodDisplay);
    }

    [Fact]
    public void StripFactorLines_RemovesAllKnownPrefixes()
    {
        string note = string.Join(
            Environment.NewLine,
            JournalNoteFactors.GetPrefix(JournalNoteFactors.StressKey),
            "body text",
            JournalNoteFactors.GetPrefix(JournalNoteFactors.HomeKey));

        Assert.Equal("body text", JournalNoteFactors.StripFactorLines(note));
    }
}
