using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class AnswerTextTests
{
    [Fact]
    public void Toggle_adds_a_chip_to_an_empty_answer()
    {
        Assert.Equal("Окно", AnswerText.Toggle(string.Empty, "Окно"));
    }

    [Fact]
    public void Toggle_appends_after_typed_text_with_a_comma()
    {
        Assert.Equal("Окно, Чашка", AnswerText.Toggle("Окно", "Чашка"));
    }

    [Fact]
    public void Toggle_removes_a_chip_that_is_already_there_whatever_the_case()
    {
        Assert.Equal("Окно, Лампа", AnswerText.Toggle("Окно, чашка, Лампа", "Чашка"));
    }

    [Fact]
    public void Toggle_handles_a_chip_that_itself_contains_a_comma()
    {
        string added = AnswerText.Toggle("Острота меньше", "Важного больше, чем казалось");

        Assert.Equal("Острота меньше, Важного больше, чем казалось", added);
        Assert.True(AnswerText.Contains(added, "Важного больше, чем казалось"));
        Assert.Equal("Острота меньше", AnswerText.Toggle(added, "Важного больше, чем казалось"));
    }

    [Fact]
    public void Toggle_does_not_take_a_chip_out_of_a_longer_part()
    {
        Assert.Equal("Чашка чая, Чашка", AnswerText.Toggle("Чашка чая", "Чашка"));
    }

    [Fact]
    public void Toggle_ignores_a_blank_chip()
    {
        Assert.Equal("Окно", AnswerText.Toggle("Окно", "  "));
    }

    [Theory]
    [InlineData("Окно, Чашка", "чашка", true)]
    [InlineData("Окно, Чашка большая", "Чашка", false)]
    [InlineData(null, "Окно", false)]
    [InlineData("Окно", "", false)]
    public void Contains_matches_whole_parts_only(string? answer, string chip, bool expected)
    {
        Assert.Equal(expected, AnswerText.Contains(answer, chip));
    }

    [Fact]
    public void AppendSpoken_capitalises_a_fresh_answer()
    {
        Assert.Equal("Вчера поссорился с братом", AnswerText.AppendSpoken("", "вчера поссорился с братом"));
    }

    [Fact]
    public void AppendSpoken_adds_after_existing_text_with_a_space()
    {
        Assert.Equal("Ссора. потом помирились", AnswerText.AppendSpoken("Ссора.  ", "потом помирились"));
    }

    [Fact]
    public void AppendSpoken_keeps_the_answer_when_nothing_was_heard()
    {
        Assert.Equal("Ссора", AnswerText.AppendSpoken("Ссора", "   "));
    }

    [Fact]
    public void Shorten_keeps_short_text_on_one_line()
    {
        Assert.Equal("Ссора с братом", AnswerText.Shorten("Ссора\n с   братом"));
    }

    [Fact]
    public void Shorten_cuts_long_text_with_an_ellipsis()
    {
        string shortened = AnswerText.Shorten("Коллега проигнорировал моё предложение на встрече", 20);

        Assert.True(shortened.Length <= 20);
        Assert.EndsWith("…", shortened);
        Assert.StartsWith("Коллега", shortened);
    }
}
