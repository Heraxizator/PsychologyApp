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

    private static void AssertUniqueNonEmpty(IReadOnlyList<string> keys)
    {
        Assert.NotEmpty(keys);
        Assert.Equal(keys.Count, keys.Distinct(StringComparer.Ordinal).Count());
        Assert.All(keys, key => Assert.False(string.IsNullOrWhiteSpace(key)));
    }
}
