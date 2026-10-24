using PsychologyApp.Application.Articles;
using Xunit;

namespace PsychologyApp.Application.Tests.Articles;

public sealed class ArticleSpeechTests
{
    [Fact]
    public void The_text_is_read_in_order_without_links()
    {
        // "Practice: Box breathing" read out between paragraphs would sound like the voice lost its place.
        Article panic = ArticleContent.Find(ArticleIds.Panic, "ru")!;

        IReadOnlyList<string> parts = ArticleSpeech.Parts(panic.Title, panic.Blocks);

        Assert.Equal(panic.Title, parts[0]);
        Assert.Equal(panic.Blocks.First(block => block.Kind == ArticleBlockKind.Paragraph).Text, parts[1]);
        Assert.DoesNotContain("Квадратное дыхание", parts);
        Assert.Contains(panic.Blocks.First(block => block.Kind == ArticleBlockKind.Cycle).Items![0], parts);
    }
}
