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
    public void BothLanguagesUseTheSamePlaceholders()
    {
        System.Text.RegularExpressions.Regex placeholder = new(@"{(d+)}");
        System.Xml.Linq.XDocument ru = System.Xml.Linq.XDocument.Load(FindResource("StringsRu.resx"));
        System.Xml.Linq.XDocument en = System.Xml.Linq.XDocument.Load(FindResource("StringsEn.resx"));
        Dictionary<string, string> enValues = en.Root!.Elements("data").ToDictionary(d => (string)d.Attribute("name")!, d => (string)d.Element("value")!);

        Assert.Equal(ru.Root!.Elements("data").Count(), enValues.Count);
        foreach (System.Xml.Linq.XElement data in ru.Root!.Elements("data"))
        {
            string key = (string)data.Attribute("name")!;
            string[] ruHoles = placeholder.Matches((string)data.Element("value")!).Select(m => m.Value).Distinct().Order().ToArray();
            string[] enHoles = placeholder.Matches(enValues[key]).Select(m => m.Value).Distinct().Order().ToArray();
            Assert.True(ruHoles.SequenceEqual(enHoles), $"{key}: placeholders differ between languages");
        }
    }

    private static string FindResource(string name)
    {
        string? dir = AppContext.BaseDirectory;
        while (dir is not null && !File.Exists(Path.Combine(dir, "PsychologyApp.Presentation.Core", "Common", name)))
        {
            dir = Path.GetDirectoryName(dir);
        }

        return Path.Combine(dir ?? throw new FileNotFoundException(name), "PsychologyApp.Presentation.Core", "Common", name);
    }

    [Fact]
    public void TextsWithParametersAreFormattedInBothLanguages()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = "ru";
            Assert.Equal("Полюс №3", AppStrings.PoleNumber(3));
            AppStrings.LanguageOverride = "en";
            Assert.Equal("Pole #3", AppStrings.PoleNumber(3));
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
