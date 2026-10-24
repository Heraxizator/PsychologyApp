using PsychologyApp.Application.Articles;
using Xunit;

namespace PsychologyApp.Application.Tests.Articles;

public sealed class ArticleSearchTests
{
    [Fact]
    public void A_word_in_the_title_puts_that_article_first()
    {
        Assert.Equal(ArticleIds.Panic, ArticleSearch.Find("паническ", "ru")[0].Id);
        Assert.Equal(ArticleIds.Ocd, ArticleSearch.Find("OCD", "en")[0].Id);
    }

    [Fact]
    public void The_search_reads_the_articles_of_the_interface_language()
    {
        // Chinese has no spaces between words: a query must still be found inside a title.
        Assert.Equal(ArticleIds.Panic, ArticleSearch.Find("惊恐", "zh")[0].Id);
        Assert.Equal(ArticleIds.Panic, ArticleSearch.Find("Panikattacken", "de")[0].Id);
        Assert.Empty(ArticleSearch.Find("Panikattacken", "en"));
    }

    [Fact]
    public void Text_inside_the_article_and_spelling_with_yo_are_found()
    {
        // People type "е" for "ё" and look for what they feel, which is often only in the body of an article.
        Assert.Contains(ArticleSearch.Find("ком в горле", "ru"), article => article.Id == ArticleIds.BodyAndStress);
        Assert.Contains(ArticleSearch.Find("еще", "ru"), article => article.Id == ArticleIds.LowMood);
    }

    [Theory]
    [InlineData("")]
    [InlineData("а")]
    [InlineData("zzzxqw")]
    public void A_too_short_or_unknown_query_finds_nothing(string query) =>
        Assert.Empty(ArticleSearch.Find(query, "ru"));
}
