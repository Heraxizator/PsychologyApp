using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ContentAssetsTests
{
    [Fact]
    public void Localized_ReturnsOriginalPathForRussian()
    {
        try
        {
            AppStrings.LanguageOverride = "ru";

            Assert.Equal("tests/beck.json", ContentAssets.Localized("tests/beck.json"));
            Assert.Equal("Psyhosomatic.txt", ContentAssets.PsychosomaticFile);
            Assert.Equal("quotes/quotes.ru.json", ContentAssets.QuotesFile);
        }
        finally
        {
            AppStrings.LanguageOverride = null;
        }
    }

    [Fact]
    public void Localized_ReturnsEnglishAssetsWhenLanguageOverrideIsEn()
    {
        try
        {
            AppStrings.LanguageOverride = "en";

            Assert.Equal("tests/beck.en.json", ContentAssets.Localized("tests/beck.json"));
            Assert.Equal("Psyhosomatic.en.txt", ContentAssets.PsychosomaticFile);
            Assert.Equal("quotes/quotes.en.json", ContentAssets.QuotesFile);
        }
        finally
        {
            AppStrings.LanguageOverride = null;
        }
    }
}
