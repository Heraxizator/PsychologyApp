using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Articles;

/// <summary>Every language carries the same articles with the same structure, and every link leads to a practice that exists under that name.</summary>
public sealed class ArticleContentTests
{
    private static readonly string[] AllLanguages = ["ru", "en", "fr", "de", "es", "pt", "zh"];

    public static TheoryData<string> Languages => new(AllLanguages);

    private static IEnumerable<Article> Everywhere => AllLanguages.SelectMany(ArticleContent.All);

    // A checklist with a missing item or a quiz with a moved right answer would give a reader of one language another verdict.
    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_language_has_the_same_articles_and_structure_as_english(string language)
    {
        IReadOnlyList<Article> ru = ArticleContent.All(language);
        IReadOnlyList<Article> en = ArticleContent.All("en");

        Assert.Equal(en.Select(article => article.Id), ru.Select(article => article.Id));
        foreach ((Article r, Article e) in ru.Zip(en))
        {
            Assert.Equal(
                r.Blocks.Select(block => (block.Kind, block.Technique, block.ArticleId, block.TestId, block.Items?.Count, block.Threshold, block.Correct)),
                e.Blocks.Select(block => (block.Kind, block.Technique, block.ArticleId, block.TestId, block.Items?.Count, block.Threshold, block.Correct)));
            Assert.Equal(r.Minutes, e.Minutes);
            Assert.Equal(r.Icon, e.Icon);
        }
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_article_is_in_exactly_one_section(string language)
    {
        // An article left out of the sections would never show on the tab.
        IReadOnlyList<ArticleSection> sections = ArticleContent.Sections(language);
        List<string> placed = sections.SelectMany(section => section.Articles).Select(article => article.Id).ToList();

        Assert.Equal(placed.Distinct().Count(), placed.Count);
        Assert.Equal(ArticleContent.All(language).Select(article => article.Id).Order(), placed.Order());
        Assert.All(sections, section => Assert.False(string.IsNullOrWhiteSpace(section.Title)));
        Assert.Equal(ArticleContent.Sections("en").Select(section => section.Key), sections.Select(section => section.Key));
    }

    [Fact]
    public void Every_article_has_a_title_a_summary_and_text()
    {
        foreach (Article article in Everywhere)
        {
            Assert.False(string.IsNullOrWhiteSpace(article.Title));
            Assert.False(string.IsNullOrWhiteSpace(article.Summary));
            Assert.Contains(article.Blocks, block => block.Kind == ArticleBlockKind.Paragraph);
            Assert.Equal(article.Id, ArticleContent.Find(article.Id, "ru")!.Id);
        }
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public async Task Practice_links_name_the_practice_as_the_catalogue_does(string language)
    {
        TechniqueCatalogService catalog = new(new BuiltInTechniqueCatalogProvider(() => language));

        foreach (ArticleBlock link in ArticleContent.All(language).SelectMany(article => article.Blocks).Where(block => block.Kind == ArticleBlockKind.Practice))
        {
            Assert.NotNull(link.Technique);
            BuiltInTechniqueDefinition definition = await catalog.GetAsync(link.Technique.Value);
            Assert.Equal(definition.ListTitle, link.Text);
        }
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public void Article_links_lead_to_an_existing_article_under_its_title(string language)
    {
        // A link to a missing article would be a dead button; a different title would promise one article and open another.
        foreach (ArticleBlock link in ArticleContent.All(language).SelectMany(article => article.Blocks).Where(block => block.Kind == ArticleBlockKind.Article))
        {
            Article? target = ArticleContent.Find(link.ArticleId ?? "", language);

            Assert.NotNull(target);
            Assert.Equal(target.Title, link.Text);
        }
    }

    [Fact]
    public void Interactive_blocks_can_be_answered()
    {
        // A checklist whose threshold cannot be reached, or a quiz whose right answer is out of range, would give the reader a wrong verdict.
        foreach (ArticleBlock block in Everywhere.SelectMany(article => article.Blocks))
        {
            switch (block.Kind)
            {
                case ArticleBlockKind.Checklist:
                    Assert.InRange(block.Threshold, 1, block.Items!.Count);
                    Assert.False(string.IsNullOrWhiteSpace(block.Verdict));
                    Assert.False(string.IsNullOrWhiteSpace(block.VerdictLow));
                    break;
                case ArticleBlockKind.Quiz:
                    Assert.InRange(block.Correct, 0, block.Items!.Count - 1);
                    Assert.True(block.Items.Count >= 2);
                    Assert.False(string.IsNullOrWhiteSpace(block.Explanation));
                    break;
                case ArticleBlockKind.Cycle:
                    Assert.InRange(block.Items!.Count, 3, 6);
                    break;
                case ArticleBlockKind.Tests:
                    // The link opens this questionnaire directly; an unknown id would land on the whole list instead.
                    Assert.Contains(block.TestId, new[] { "pcptsd5", "pcl5", "phq9", "gad7", "isi", "scoff" });
                    break;
            }
        }
    }

    [Fact]
    public void The_trauma_start_leads_to_the_trauma_screen_and_the_flashback_help()
    {
        // The hub is where a positive screen and the catalogue card send the person; without these links it would be a dead end.
        foreach (string language in AllLanguages)
        {
            IReadOnlyList<ArticleBlock> blocks = ArticleContent.Find(ArticleIds.TraumaStart, language)!.Blocks;

            Assert.Contains(blocks, block => block.Kind == ArticleBlockKind.Tests);
            Assert.Contains(blocks, block => block.Technique == TechniqueId.Flashback);
        }
    }

    [Theory]
    [InlineData(ArticleIds.Specialist)]
    [InlineData(ArticleIds.LowMood)]
    [InlineData(ArticleIds.Narcissism)]
    [InlineData(ArticleIds.EmotionalAbuse)]
    [InlineData(ArticleIds.Grief)]
    [InlineData(ArticleIds.SupportInCrisis)]
    [InlineData(ArticleIds.TraumaStart)]
    [InlineData(ArticleIds.TraumaReaction)]
    [InlineData(ArticleIds.TraumaTreatment)]
    [InlineData(ArticleIds.Depression)]
    [InlineData(ArticleIds.ParentStress)]
    [InlineData(ArticleIds.HelpingLovedOne)]
    [InlineData(ArticleIds.EatingDisorders)]
    [InlineData(ArticleIds.Bipolar)]
    [InlineData(ArticleIds.Psychosis)]
    public void Articles_that_speak_of_self_harm_or_danger_lead_to_crisis_help(string id)
    {
        // Naming thoughts of death or a danger at home without a way to help would leave the person alone with them.
        foreach (string language in AllLanguages)
        {
            Assert.Contains(ArticleContent.Find(id, language)!.Blocks, block => block.Kind == ArticleBlockKind.CrisisHelp);
        }
    }

    [Fact]
    public void The_body_article_says_when_a_doctor_is_needed()
    {
        // A health article that only offers practices could keep someone with a dangerous symptom away from care.
        foreach (string language in AllLanguages)
        {
            Article body = ArticleContent.Find(ArticleIds.BodyAndStress, language)!;
            int warning = body.Blocks.ToList().FindIndex(block => block.Kind == ArticleBlockKind.Warning);

            Assert.True(warning >= 0);
            Assert.Equal(ArticleBlockKind.Bullets, body.Blocks[warning + 1].Kind);
        }
    }

    [Fact]
    public void A_language_without_its_own_articles_gets_the_english_ones() =>
        Assert.Same(ArticleContent.All("en"), ArticleContent.All("it"));
}
