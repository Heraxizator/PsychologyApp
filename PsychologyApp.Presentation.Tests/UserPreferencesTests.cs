using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class UserPreferencesTests
{
    [Fact]
    public void DefaultState_MatchesBuiltInDefaults()
    {
        UserPreferencesState state = new();

        Assert.Equal(UserPreferences.DefaultLanguage, state.Language);
        Assert.Equal(UserPreferences.DefaultTheme, state.Theme);
        Assert.Equal(UserPreferences.DefaultColor, state.Color);
        Assert.Equal(UserPreferences.DefaultForm, state.Form);
        Assert.Equal(UserPreferences.DefaultSize, state.Size);
        Assert.False(state.IsBold);
    }

    [Fact]
    public void PreferenceKeys_AreStable()
    {
        Assert.Equal("Language", UserPreferences.LanguageKey);
        Assert.Equal("Theme", UserPreferences.ThemeKey);
        Assert.Equal("Color", UserPreferences.ColorKey);
        Assert.Equal("Form", UserPreferences.FormKey);
        Assert.Equal("Size", UserPreferences.SizeKey);
        Assert.Equal("IsBold", UserPreferences.IsBoldKey);
    }

    [Fact]
    public void NormalizeThemeKey_MapsLegacyEnglishAndRussian()
    {
        Assert.Equal("dark", UserPreferences.NormalizeThemeKey("Dark"));
        Assert.Equal("light", UserPreferences.NormalizeThemeKey("Light"));
        Assert.Equal("dark", UserPreferences.NormalizeThemeKey("Тёмная"));
        Assert.Equal("light", UserPreferences.NormalizeThemeKey("Светлая"));
    }

    [Fact]
    public void IsDarkTheme_RecognizesKeysAndLegacyValues()
    {
        Assert.True(UserPreferences.IsDarkTheme("dark"));
        Assert.True(UserPreferences.IsDarkTheme("Тёмная"));
        Assert.True(UserPreferences.IsDarkTheme("Dark"));
        Assert.False(UserPreferences.IsDarkTheme("light"));
        Assert.False(UserPreferences.IsDarkTheme("Светлая"));
    }

    [Fact]
    public void NormalizeColorKey_MapsLegacyEnglishAndRussian()
    {
        Assert.Equal("blue", UserPreferences.NormalizeColorKey("Blue"));
        Assert.Equal("red", UserPreferences.NormalizeColorKey("Красный"));
    }

    [Fact]
    public void NormalizeSizeKey_MapsLegacyEnglishAndRussian()
    {
        Assert.Equal("large", UserPreferences.NormalizeSizeKey("Large"));
        Assert.Equal("medium", UserPreferences.NormalizeSizeKey("Средний"));
        Assert.Equal("small", UserPreferences.NormalizeSizeKey("Малый"));
    }

    [Fact]
    public void NormalizeLanguageKey_MapsDisplayAndKeys()
    {
        Assert.Equal("en", UserPreferences.NormalizeLanguageKey("English"));
        Assert.Equal("en", UserPreferences.NormalizeLanguageKey("Английский"));
        Assert.Equal("ru", UserPreferences.NormalizeLanguageKey("Русский"));
        Assert.Equal("ru", UserPreferences.NormalizeLanguageKey("ru"));
    }

    [Fact]
    public void GetThemeLabel_ReturnsEnglishLabels()
    {
        Assert.Equal("Dark", UserPreferences.GetThemeLabel("dark", "en"));
        Assert.Equal("Light", UserPreferences.GetThemeLabel("light", "en"));
    }

    [Fact]
    public void GetThemeLabel_ReturnsRussianLabels()
    {
        Assert.Equal("Тёмная", UserPreferences.GetThemeLabel("dark", "ru"));
        Assert.Equal("Светлая", UserPreferences.GetThemeLabel("light", "ru"));
    }

    [Fact]
    public void GetFormLabel_ReturnsEnglishLabels()
    {
        Assert.Equal("Rounded", UserPreferences.GetFormLabel("rounded", "en"));
        Assert.Equal("Square corners", UserPreferences.GetFormLabel("square", "en"));
    }

    [Fact]
    public void Load_PreservesOnboardingWhenSavingSettingsFields()
    {
        UserPreferencesState original = new()
        {
            Language = "ru",
            Theme = "light",
            Color = "blue",
            Form = "rounded",
            Size = "medium",
            IsBold = false,
            HasCompletedOnboarding = true,
            OnboardingConcern = "anxiety"
        };

        UserPreferencesState merged = new()
        {
            Language = "en",
            Theme = original.Theme,
            Color = original.Color,
            Form = original.Form,
            Size = original.Size,
            IsBold = original.IsBold,
            HasCompletedOnboarding = original.HasCompletedOnboarding,
            OnboardingConcern = original.OnboardingConcern
        };

        Assert.True(merged.HasCompletedOnboarding);
        Assert.Equal("anxiety", merged.OnboardingConcern);
    }

    [Fact]
    public void ApplyAccentColor_DoesNotThrowWhenResourcesUnavailable()
    {
        Exception? exception = Record.Exception(() => UserPreferences.ApplyAccentColor("red"));
        Assert.Null(exception);
    }

    [Fact]
    public void ApplyTypography_DoesNotThrowWhenResourcesUnavailable()
    {
        Exception? exception = Record.Exception(() => UserPreferences.ApplyTypography("large", true));
        Assert.Null(exception);
    }

    [Fact]
    public void State_RoundTripsThroughCopy()
    {
        UserPreferencesState original = new()
        {
            Language = "en",
            Theme = "dark",
            Color = "green",
            Form = "square",
            Size = "large",
            IsBold = true
        };

        UserPreferencesState copy = new()
        {
            Language = original.Language,
            Theme = original.Theme,
            Color = original.Color,
            Form = original.Form,
            Size = original.Size,
            IsBold = original.IsBold
        };

        Assert.Equal(original.Language, copy.Language);
        Assert.Equal(original.Theme, copy.Theme);
        Assert.Equal(original.Color, copy.Color);
        Assert.Equal(original.Form, copy.Form);
        Assert.Equal(original.Size, copy.Size);
        Assert.Equal(original.IsBold, copy.IsBold);
    }
}
