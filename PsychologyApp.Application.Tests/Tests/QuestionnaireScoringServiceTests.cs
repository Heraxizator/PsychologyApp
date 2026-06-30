using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class QuestionnaireScoringServiceTests
{
    private readonly QuestionnaireScoringService _service = new();

    [Fact]
    public void Calculate_ReturnsScoreBandAndRecommendation()
    {
        List<Question> questions =
        [
            new()
            {
                Number = 1,
                Answers = [new Answer { Ball = 12, Selected = true }]
            }
        ];

        QuestionnaireScoringResult result = _service.Calculate(questions, "beck");

        Assert.Equal(12, result.Score);
        Assert.Equal(1, result.BandIndex);
        Assert.Equal(TechniqueId.Spin, result.RecommendedTechnique);
    }

    [Fact]
    public void TryValidateAllAnswered_ReturnsFalse_WhenMissingSelection()
    {
        List<Question> questions =
        [
            new() { Number = 1, Answers = [new Answer { Ball = 1, Selected = false }] }
        ];

        Assert.False(_service.TryValidateAllAnswered(questions));
    }
}
