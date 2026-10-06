using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The settings screen and the onboarding read and write these keys; an alias that stops mapping would silently reset someone's choice.</summary>
[Collection("Localization")]
public sealed class UserPreferencesOptionsTests
{
    [Theory]
    [InlineData("dark", "dark")]
    [InlineData("Тёмная", "dark")]
    [InlineData("Темная", "dark")]
    [InlineData("Light", "light")]
    [InlineData("Светлая", "light")]
    [InlineData("", "light")]
    [InlineData("   ", "light")]
    public void ThemeNamesAndDisplayTextsMapToTheStoredKey(string input, string expected) =>
        Assert.Equal(expected, UserPreferences.NormalizeThemeKey(input));

    [Theory]
    [InlineData("Blue", "blue")]
    [InlineData("Красный", "red")]
    [InlineData("Желтый", "yellow")]
    [InlineData("Зеленый", "green")]
    [InlineData("Зелёный", "green")]
    [InlineData("", "blue")]
    public void ColorsMapToTheStoredKey(string input, string expected) =>
        Assert.Equal(expected, UserPreferences.NormalizeColorKey(input));

    [Theory]
    [InlineData("Normal", "rounded")]
    [InlineData("С закруглением", "rounded")]
    [InlineData("Без закругления", "square")]
    [InlineData("", "rounded")]
    public void FormsMapToTheStoredKey(string input, string expected) =>
        Assert.Equal(expected, UserPreferences.NormalizeFormKey(input));

    [Theory]
    [InlineData("Большой", "large")]
    [InlineData("Medium", "medium")]
    [InlineData("Малый", "small")]
    [InlineData("", "medium")]
    public void SizesMapToTheStoredKey(string input, string expected) =>
        Assert.Equal(expected, UserPreferences.NormalizeSizeKey(input));

    [Theory]
    [InlineData("en", true)]
    [InlineData("EN", true)]
    [InlineData("English", true)]
    [InlineData("Английский", true)]
    [InlineData("ru", false)]
    [InlineData("Русский", false)]
    public void TheLanguageNameTellsEnglishFromRussian(string language, bool english) =>
        Assert.Equal(english, UserPreferences.IsEnglish(language));

    [Fact]
    public void EveryStoredKeyHasALabelInBothLanguages()
    {
        foreach (string language in new[] { "ru", "en" })
        {
            foreach (string key in UserPreferences.ThemeKeys) Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetThemeLabel(key, language)));
            foreach (string key in UserPreferences.ColorKeys) Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetColorLabel(key, language)));
            foreach (string key in UserPreferences.FormKeys) Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetFormLabel(key, language)));
            foreach (string key in UserPreferences.SizeKeys) Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetSizeLabel(key, language)));
            foreach (string key in UserPreferences.LanguageKeys) Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetLanguageLabel(key, language)));
        }
    }

    [Fact]
    public void ALabelParsesBackToItsKey()
    {
        foreach (string language in new[] { "ru", "en" })
        {
            foreach (string key in UserPreferences.ThemeKeys) Assert.Equal(key, UserPreferences.ParseThemeKey(UserPreferences.GetThemeLabel(key, language)));
            foreach (string key in UserPreferences.ColorKeys) Assert.Equal(key, UserPreferences.ParseColorKey(UserPreferences.GetColorLabel(key, language)));
            foreach (string key in UserPreferences.FormKeys) Assert.Equal(key, UserPreferences.ParseFormKey(UserPreferences.GetFormLabel(key, language)));
            foreach (string key in UserPreferences.SizeKeys) Assert.Equal(key, UserPreferences.ParseSizeKey(UserPreferences.GetSizeLabel(key, language)));
            foreach (string key in UserPreferences.LanguageKeys) Assert.Equal(key, UserPreferences.ParseLanguageKey(UserPreferences.GetLanguageLabel(key, language)));
        }
    }

    [Fact]
    public void ReminderHoursAreClampedAndParsedFromTheirLabels()
    {
        foreach (int hour in UserPreferences.PracticeReminderHourKeys)
        {
            Assert.Equal(hour, UserPreferences.ParsePracticeReminderHourKey(UserPreferences.GetPracticeReminderHourLabel(hour)));
        }

        Assert.Equal(UserPreferences.PracticeReminderHourKeys[0], UserPreferences.NormalizePracticeReminderHour(-5));
        Assert.Equal(UserPreferences.PracticeReminderHourKeys[^1], UserPreferences.NormalizePracticeReminderHour(99));
        Assert.Equal(UserPreferences.DefaultPracticeReminderHour, UserPreferences.ParsePracticeReminderHourKey("not a time"));
        Assert.Equal(UserPreferences.PracticeReminderHourKeys.Count, UserPreferences.GetPracticeReminderHourOptions().Count);
    }

    [Fact]
    public void QuoteAndChatReminderHoursRoundTripThroughTheirLabels()
    {
        foreach (int hour in UserPreferences.QuoteReminderHourKeys)
        {
            Assert.Equal(hour, UserPreferences.ParseQuoteReminderHourKey(UserPreferences.GetQuoteReminderHourLabel(hour)));
            Assert.Equal(hour, UserPreferences.ParseChatReminderHourKey(UserPreferences.GetChatReminderHourLabel(hour)));
        }

        Assert.Equal(UserPreferences.QuoteReminderHourKeys.Count, UserPreferences.GetQuoteReminderHourOptions().Count);
        Assert.Equal(UserPreferences.QuoteReminderHourKeys.Count, UserPreferences.GetChatReminderHourOptions().Count);
    }

    [Fact]
    public void EveryOnboardingConcernHasALabelAndNormalisesToItself()
    {
        foreach (string key in UserPreferences.OnboardingConcernKeysList)
        {
            Assert.Equal(key, UserPreferences.NormalizeOnboardingConcernKey(key));
            Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetOnboardingConcernLabel(key, "ru")));
            Assert.False(string.IsNullOrWhiteSpace(UserPreferences.GetOnboardingConcernLabel(key, "en")));
            Assert.Equal(key, UserPreferences.ParseOnboardingConcernKey(UserPreferences.GetOnboardingConcernLabel(key, "en")));
        }

        Assert.Equal(UserPreferences.DefaultOnboardingConcern, UserPreferences.NormalizeOnboardingConcernKey("nonsense"));
    }

    [Fact]
    public void ANewStateStartsWithTheDocumentedDefaults()
    {
        UserPreferencesState state = new();

        Assert.Equal("ru", state.Language);
        Assert.Equal("light", state.Theme);
        Assert.True(state.QuestionnaireAutoAdvance);
        Assert.False(state.HasCompletedOnboarding);
        Assert.True(state.PracticeRemindersEnabled);
        Assert.False(state.QuoteRemindersEnabled);
        Assert.Equal(UserPreferences.DefaultPracticeReminderHour, state.PracticeReminderHour);
    }
}
