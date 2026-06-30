using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using Moq;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class QuestionnaireResultDetailServiceTests
{
    private readonly QuestionnaireResultDetailService _service = new();

    [Fact]
    public async Task BuildAndSerialize_RoundTripsDetail()
    {
        var catalog = new Mock<ITestCatalogService>();
        catalog
            .Setup(c => c.GetByIdAsync("beck", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestDefinition
            {
                TestId = "beck",
                Title = "Beck",
                Subtitle = "Sub",
                Description = "Desc",
                Comment = "Note",
                Algorithm = ["Step"],
                Kind = TestKind.Questionnaire,
                Construct = "Depression"
            });

        DateTime started = DateTime.UtcNow.AddMinutes(-1);
        DateTime completed = DateTime.UtcNow;
        List<Question> questions =
        [
            new()
            {
                Number = 1,
                Context = "Q1",
                Answers = [new Answer { Ball = 2, Text = "Often", Selected = true }]
            }
        ];

        QuestionnaireResultDetail? detail = await _service.BuildAsync(
            new QuestionnaireDetailBuildRequest(questions, "beck", "beck", started, completed),
            catalog.Object);

        Assert.NotNull(detail);
        string? json = _service.Serialize(detail);
        QuestionnaireResultDetail? parsed = _service.TryParse(json);

        Assert.NotNull(parsed);
        Assert.Equal("beck", parsed!.TestId);
        Assert.Equal("Depression", parsed.Construct);
        Assert.Single(parsed.Questions);
    }
}
