using PsychologyApp.Application.Chat;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class ChatTextTests
{
    [Theory]
    [InlineData("мне тревожно", "Мне тревожно")]
    [InlineData("  привет  ", "Привет")]
    [InlineData("я устала. он опять кричал", "Я устала. Он опять кричал")]
    [InlineData("почему так? не понимаю! всё плохо", "Почему так? Не понимаю! Всё плохо")]
    [InlineData("мне плохо… я не знаю", "Мне плохо… Я не знаю")]
    [InlineData("«я не знаю», сказала она", "«Я не знаю», сказала она")]
    [InlineData("первая строка\nвторая строка", "Первая строка\nВторая строка")]
    [InlineData("i feel anxious. it never stops", "I feel anxious. It never stops")]
    [InlineData("Уже Заглавная", "Уже Заглавная")]
    public void Sentences_start_with_a_capital(string input, string expected) =>
        Assert.Equal(expected, ChatText.Capitalize(input));

    [Theory]
    [InlineData("дела, работа и т. д. потом отдых", "Дела, работа и т. д. потом отдых")]
    [InlineData("в 5 часов. потом", "В 5 часов. Потом")]
    [InlineData("3.5 часа сна", "3.5 часа сна")]
    public void Abbreviations_and_numbers_do_not_trigger_capitals(string input, string expected) =>
        Assert.Equal(expected, ChatText.Capitalize(input));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Blank_text_becomes_empty(string? input) => Assert.Equal(string.Empty, ChatText.Capitalize(input));

    [Fact]
    public void Text_starting_with_an_emoji_or_symbol_capitalises_the_first_letter()
    {
        Assert.Equal("😔 Мне грустно", ChatText.Capitalize("😔 мне грустно"));
    }
}
