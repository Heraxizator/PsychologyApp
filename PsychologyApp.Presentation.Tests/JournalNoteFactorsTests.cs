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
}
