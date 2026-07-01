using System.Globalization;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Converters;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class PreferenceLabelConverterTests : IDisposable
{
    public void Dispose() => AppStrings.LanguageOverride = null;

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
    public void Convert_WithoutParameter_PassesValueThrough()
    {
        AppStrings.LanguageOverride = "ru";
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("blue", typeof(string), null, CultureInfo.InvariantCulture));

        Assert.Equal("blue", label);
    }

    [Fact]
    public void Convert_WithPassThroughKind_ReturnsReminderHourLabel()
    {
        PreferenceLabelConverter converter = new();

        string label = Assert.IsType<string>(
            converter.Convert("19:00", typeof(string), PreferenceLabelKind.PassThrough, CultureInfo.InvariantCulture));

        Assert.Equal("19:00", label);
    }
}
