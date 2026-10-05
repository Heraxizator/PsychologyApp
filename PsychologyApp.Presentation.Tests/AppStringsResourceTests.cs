using System.Reflection;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class AppStringsResourceTests
{
    private static IEnumerable<PropertyInfo> TextProperties() =>
        typeof(AppStrings)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string) && p.GetMethod is not null && p.GetIndexParameters().Length == 0
                        && p.Name is not (nameof(AppStrings.Language) or nameof(AppStrings.LanguageOverride) or nameof(AppStrings.DefaultLanguage)));

    [Fact]
    public void EveryTextExistsInBothLanguages_AndIsNotTheKeyItself()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            foreach (string language in new[] { "ru", "en" })
            {
                AppStrings.LanguageOverride = language;
                foreach (PropertyInfo property in TextProperties())
                {
                    if (property.GetCustomAttribute<ObsoleteAttribute>() is not null || property.Name == nameof(AppStrings.JournalOnThisDayLastYearEmpty))
                    {
                        continue;
                    }

                    string? text = (string?)property.GetValue(null);
                    Assert.False(string.IsNullOrWhiteSpace(text), $"{property.Name} is empty in {language}");
                }
            }
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }

    [Fact]
    public void TheLanguageSwitchPicksTheMatchingFile()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = "ru";
            Assert.Equal("Параметры", AppStrings.OptionsTitle);
            AppStrings.LanguageOverride = "en";
            Assert.Equal("Options", AppStrings.OptionsTitle);
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }
}
