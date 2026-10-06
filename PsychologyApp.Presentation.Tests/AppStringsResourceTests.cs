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

    [Fact]
    public void EveryKeyUsedInTheSourceExistsInTheResources()
    {
        string common = Path.GetDirectoryName(FindResource("StringsRu.resx"))!;
        System.Text.RegularExpressions.Regex used = new("\\bR\\(\"([^\"]+)\"\\)");
        HashSet<string> keys = System.Xml.Linq.XDocument.Load(Path.Combine(common, "StringsRu.resx")).Root!.Elements("data")
            .Select(d => (string)d.Attribute("name")!).ToHashSet();

        foreach (string file in Directory.GetFiles(common, "AppStrings*.cs"))
        {
            foreach (System.Text.RegularExpressions.Match match in used.Matches(File.ReadAllText(file)))
            {
                Assert.True(keys.Contains(match.Groups[1].Value), $"{Path.GetFileName(file)}: no text for {match.Groups[1].Value}");
            }
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
    public void TextsWithComputedPartsAreStillFormattedInBothLanguages()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = "ru";
            Assert.StartsWith("На этой неделе: 3 ", AppStrings.WeeklyInsightLine(3, string.Empty));
            Assert.EndsWith("настроение ↑", AppStrings.WeeklyInsightLine(3, "↑"));
            AppStrings.LanguageOverride = "en";
            Assert.StartsWith("This week: 3 ", AppStrings.WeeklyInsightLine(3, string.Empty));
            Assert.EndsWith("mood ↑", AppStrings.WeeklyInsightLine(3, "↑"));
            Assert.Contains("2026", AppStrings.JournalDayEmptyHint(new DateOnly(2026, 10, 6)).Replace("26", "2026"));
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
