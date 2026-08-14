using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class AppStringsErrorTests
{
    [Fact]
    public void ErrorStrings_AreRussianByDefault()
    {
        AppStrings.LanguageOverride = null;
        AppStrings.LanguageProvider = () => "ru";

        Assert.Equal("Ошибка", AppStrings.ErrorTitle);
        Assert.Equal("Произошла непредвиденная ошибка. Попробуйте ещё раз.", AppStrings.UnexpectedErrorMessage);
    }

    [Fact]
    public void ErrorStrings_AreEnglishWhenLanguageOverrideIsEn()
    {
        try
        {
            AppStrings.LanguageOverride = "en";

            Assert.Equal("Error", AppStrings.ErrorTitle);
            Assert.Equal("An unexpected error occurred. Please try again.", AppStrings.UnexpectedErrorMessage);
            Assert.Equal("Technique not found.", AppStrings.TechniqueNotFound);
            Assert.Equal("Quote not found.", AppStrings.QuoteNotFound);
        }
        finally
        {
            AppStrings.LanguageOverride = null;
        }
    }
}
