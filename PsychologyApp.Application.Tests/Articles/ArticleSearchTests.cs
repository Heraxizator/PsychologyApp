using PsychologyApp.Application.Articles;
using Xunit;

namespace PsychologyApp.Application.Tests.Articles;

public sealed class ArticleSearchTests
{
    [Fact]
    public void A_word_in_the_title_puts_that_article_first()
    {
        Assert.Equal(ArticleIds.Panic, ArticleSearch.Find("паническ", english: false)[0].Id);
        Assert.Equal(ArticleIds.Ocd, ArticleSearch.Find("OCD", english: true)[0].Id);
    }

    [Fact]
    public void Text_inside_the_article_and_spelling_with_yo_are_found()
    {
        // People type "е" for "ё" and look for what they feel, which is often only in the body of an article.
        Assert.Contains(ArticleSearch.Find("ком в горле", english: false), article => article.Id == ArticleIds.BodyAndStress);
        Assert.Contains(ArticleSearch.Find("еще", english: false), article => article.Id == ArticleIds.LowMood);
    }

    [Theory]
    [InlineData("")]
    [InlineData("а")]
    [InlineData("zzzxqw")]
    public void A_too_short_or_unknown_query_finds_nothing(string query) =>
        Assert.Empty(ArticleSearch.Find(query, english: false));
}
