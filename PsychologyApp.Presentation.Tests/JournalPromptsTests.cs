using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.ManageJournal;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The question of the day above the journal note.</summary>
public sealed class JournalPromptsTests
{
    // The same question all day, a different one the next, and every question comes round within the cycle.
    [Fact]
    public void The_question_changes_with_the_day_and_goes_through_all_of_them()
    {
        DateOnly start = new(2026, 10, 26);

        Assert.NotEqual(JournalPrompts.For(start), JournalPrompts.For(start.AddDays(1)));
        Assert.Equal(
            Enumerable.Range(1, JournalPrompts.Count),
            Enumerable.Range(0, JournalPrompts.Count).Select(offset => JournalPrompts.For(start.AddDays(offset))).Order());
    }

    // A number without a text would show the resource key above the note.
    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    [InlineData("zh")]
    public void Every_question_has_a_text(string language)
    {
        for (int number = 1; number <= JournalPrompts.Count; number++)
        {
            string key = $"JournalPrompt{number:00}";
            Assert.NotEqual(key, AppStrings.In(language, key));
        }
    }
}
