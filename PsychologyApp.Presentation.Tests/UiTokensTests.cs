using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class UiTokensTests
{
    [Fact]
    public void RequiredColorKeys_AreUniqueAndNonEmpty()
    {
        AssertUniqueNonEmpty(UiTokenCatalog.RequiredColorKeys);
    }

    [Fact]
    public void RequiredTypographyKeys_AreUniqueAndNonEmpty()
    {
        AssertUniqueNonEmpty(UiTokenCatalog.RequiredTypographyKeys);
    }

    [Fact]
    public void RequiredStyleKeys_AreUniqueAndNonEmpty()
    {
        AssertUniqueNonEmpty(UiTokenCatalog.RequiredStyleKeys);
    }

    [Fact]
    public void RequiredStyleKeys_IncludeHeroTokens()
    {
        Assert.Contains("HeroCardStyle", UiTokenCatalog.RequiredStyleKeys);
        Assert.Contains("HeroQuoteStyle", UiTokenCatalog.RequiredStyleKeys);
    }

    [Fact]
    public void RequiredStyleKeys_AreDeclaredInStyleDictionaries()
    {
        string stylesRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation", "Resources", "Styles"));
        string declarations = string.Concat(
            Directory.EnumerateFiles(stylesRoot, "*.xaml").Select(File.ReadAllText));

        foreach (string key in UiTokenCatalog.RequiredStyleKeys)
        {
            Assert.Contains($"x:Key=\"{key}\"", declarations, StringComparison.Ordinal);
        }
    }

    private static void AssertUniqueNonEmpty(IReadOnlyList<string> keys)
    {
        Assert.NotEmpty(keys);
        Assert.Equal(keys.Count, keys.Distinct(StringComparer.Ordinal).Count());
        Assert.All(keys, key => Assert.False(string.IsNullOrWhiteSpace(key)));
    }
}
