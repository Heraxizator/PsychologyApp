using PsychologyApp.Application.AnchorThoughts;
using PsychologyApp.Application.Articles;
using Xunit;

namespace PsychologyApp.Application.Tests.AnchorThoughts;

/// <summary>Every language shows the same thoughts, each with its reason, and every link opens an article that exists.</summary>
public sealed class AnchorThoughtContentTests
{
    public static TheoryData<string> Languages => new(["ru", "en", "fr", "de", "es", "pt", "zh"]);

    // A thought missing in one language would crash the tab there; a copied line would show one thought twice.
    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_language_has_every_thought_with_its_reason(string language)
    {
        IReadOnlyList<AnchorThought> thoughts = AnchorThoughtContent.All(language);

        Assert.Equal(AnchorThoughtContent.All("en").Select(thought => thought.Id), thoughts.Select(thought => thought.Id));
        Assert.All(thoughts, thought =>
        {
            Assert.False(string.IsNullOrWhiteSpace(thought.Text));
            Assert.False(string.IsNullOrWhiteSpace(thought.Why));
        });
        Assert.Equal(thoughts.Count, thoughts.Select(thought => thought.Text).Distinct().Count());
        Assert.Equal(thoughts.Count, thoughts.Select(thought => thought.Why).Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_thought_leads_to_an_article_of_its_language(string language)
    {
        Assert.All(AnchorThoughtContent.All(language), thought => Assert.NotNull(ArticleContent.Find(thought.ArticleId, language)));
    }

    [Fact]
    public void A_language_without_its_own_thoughts_gets_the_english_ones()
    {
        Assert.Equal(AnchorThoughtContent.All("en"), AnchorThoughtContent.All("ja"));
    }

    // A theme without thoughts would be an empty chip on the page.
    [Fact]
    public void Every_theme_has_thoughts()
    {
        IReadOnlyList<AnchorThought> thoughts = AnchorThoughtContent.All("en");
        Assert.All(Enum.GetValues<AnchorTheme>(), theme => Assert.Contains(thoughts, thought => thought.Theme == theme));
        Assert.Equal(thoughts.Count, thoughts.Select(thought => thought.Id).Distinct().Count());
    }
}
