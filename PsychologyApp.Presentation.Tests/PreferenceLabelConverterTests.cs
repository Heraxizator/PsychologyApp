using System.Globalization;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.UI.Converters;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class PreferenceLabelConverterTests : IDisposable
{
    public PreferenceLabelConverterTests()
    {
        UserPreferences.UseInMemoryStorage();
    }

    public void Dispose()
    {
        AppStrings.LanguageOverride = null;
        UserPreferences.ResetInMemoryStorage();
    }

    [Fact]
    public void Convert_ReturnsRussianLabelInEnglishUi()
    {
        AppStrings.LanguageOverride = "en";
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("ru", typeof(string), PreferenceLabelKind.Language, CultureInfo.InvariantCulture));

        Assert.Equal("Russian", label);
    }

    [Fact]
    public void Convert_ReturnsLocalizedThemeLabelForPreviewLanguage()
    {
        AppStrings.LanguageOverride = "ru";
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("light", typeof(string), PreferenceLabelKind.Theme, CultureInfo.InvariantCulture));

        Assert.Equal("Светлая", label);
    }

    [Fact]
    public void Convert_WithColorKind_ReturnsColorLabelNotLanguageLabel()
    {
        AppStrings.LanguageOverride = "ru";
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("blue", typeof(string), PreferenceLabelKind.Color, CultureInfo.InvariantCulture));

        Assert.Equal("Синий", label);
    }

    [Fact]
    public void Convert_WithoutParameter_DoesNotTreatColorKeyAsLanguage()
    {
        AppStrings.LanguageOverride = "ru";
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("blue", typeof(string), null, CultureInfo.InvariantCulture));

        Assert.Equal("Русский", label);
    }
}
