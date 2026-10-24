using PsychologyApp.Application.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Practice;

public class EmotionPaletteTests
{
    // Every language offers the same wheel: a missing family or a shorter list would leave a feeling without a word to pick.
    [Theory]
    [InlineData("ru")]
    [InlineData("fr")]
    [InlineData("de")]
    [InlineData("es")]
    [InlineData("pt")]
    [InlineData("zh")]
    public void EachLanguageHasTheEnglishFamiliesInOrderWithAsManyDistinctShades(string language)
    {
        IReadOnlyList<EmotionGroup> english = EmotionPalette.For("en");
        IReadOnlyList<EmotionGroup> translated = EmotionPalette.For(language);

        Assert.Equal(english.Select(g => g.Family), translated.Select(g => g.Family));
        for (int i = 0; i < english.Count; i++)
        {
            Assert.Equal(english[i].Shades.Count, translated[i].Shades.Distinct().Count());
            Assert.DoesNotContain(translated[i].Name, translated[i].Shades);
            Assert.NotEqual(english[i].Name, translated[i].Name);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("it")]
    public void ALanguageWithoutItsOwnWordsGetsTheEnglishOnes(string? language) =>
        Assert.Same(EmotionPalette.For("en"), EmotionPalette.For(language));
}
