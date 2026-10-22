using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The rules of the app's languages: which one a stored or shown name means, what content it shows, where the chat is open.</summary>
public sealed class AppLanguagesTests
{
    [Theory]
    [InlineData("fr", "fr")]
    [InlineData("Français", "fr")]
    [InlineData("Deutsch", "de")]
    [InlineData("中文", "zh")]
    [InlineData("pt-BR", "pt")]
    [InlineData("English", "en")]
    [InlineData("Английский", "en")]
    [InlineData("Русский", "ru")]
    [InlineData("", "ru")]
    [InlineData("klingon", "ru")]
    public void A_stored_or_shown_language_reads_as_its_key(string value, string key) =>
        Assert.Equal(key, AppLanguages.Normalize(value));

    [Fact]
    public void Every_language_but_Russian_shows_the_English_content_until_its_own_exists()
    {
        Assert.Equal(AppLanguages.Russian, AppLanguages.ContentLanguage(AppLanguages.Russian));
        Assert.All(AppLanguages.All.Where(key => key != AppLanguages.Russian),
            key => Assert.Equal(AppLanguages.English, AppLanguages.ContentLanguage(key)));
    }

    [Fact]
    public void The_chat_is_open_only_where_it_can_read_crisis_words()
    {
        Assert.Equal([AppLanguages.Russian, AppLanguages.English], AppLanguages.All.Where(AppLanguages.HasChat));
    }

    [Fact]
    public void A_phone_in_a_known_language_starts_the_app_in_it()
    {
        Assert.Equal(AppLanguages.German, AppLanguages.FromSystem("de"));
        Assert.Null(AppLanguages.FromSystem("ja"));
    }
}
