using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>A language reads its own content file when there is one, otherwise the English one, never an empty screen.</summary>
[Collection("Localization")]
public sealed class ContentAssetsTests
{
    [Theory]
    [InlineData("ru", new[] { "tests/beck.json" })]
    [InlineData("en", new[] { "tests/beck.en.json", "tests/beck.json" })]
    [InlineData("fr", new[] { "tests/beck.fr.json", "tests/beck.en.json", "tests/beck.json" })]
    public void A_language_tries_its_own_file_then_english_then_the_original(string language, string[] expected) =>
        WithLanguage(language, () => Assert.Equal(expected, ContentAssets.Candidates("tests/beck.json")));

    [Theory]
    [InlineData("ru", new[] { "quotes/quotes.ru.json" })]
    [InlineData("en", new[] { "quotes/quotes.en.json" })]
    [InlineData("zh", new[] { "quotes/quotes.zh.json", "quotes/quotes.en.json" })]
    public void Quotes_come_from_the_language_file_or_the_english_one(string language, string[] expected) =>
        WithLanguage(language, () => Assert.Equal(expected, ContentAssets.QuotesFiles));

    private static void WithLanguage(string language, Action check)
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = language;
            check();
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }
}
