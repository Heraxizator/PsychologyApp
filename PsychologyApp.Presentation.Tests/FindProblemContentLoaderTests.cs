using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class FindProblemContentLoaderTests
{
    [Fact]
    public async Task LoadAsync_MapsDefinitionFields()
    {
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Beck",
            Subtitle = "Sub",
            Description = "Description",
            Comment = "Comment",
            Algorithm = ["Step 1", "Step 2"],
            Kind = TestKind.Questionnaire,
            AnalyzerId = "beck"
        });

        FindProblemContentLoader loader = new();

        FindProblemContentSnapshot? snapshot = await loader.LoadAsync("beck", catalog);

        Assert.NotNull(snapshot);
        Assert.Equal("Description", snapshot!.Description);
        Assert.Equal("Comment", snapshot.Comment);
        Assert.Equal(["Step 1", "Step 2"], snapshot.Algorithm);
    }

    [Fact]
    public async Task LoadAsync_ReturnsNull_WhenDefinitionMissing()
    {
        FindProblemContentLoader loader = new();

        FindProblemContentSnapshot? snapshot = await loader.LoadAsync("missing", new FakeTestCatalogService());

        Assert.Null(snapshot);
    }
}
