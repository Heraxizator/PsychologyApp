using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Domain.Tests.Tests;

public sealed class QuestionnaireScoreCalculatorTests
{
    [Fact]
    public void CalculateScore_SumsSelectedBalls()
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> questions =
        [
            new([1, 2]),
            new([3])
        ];

        Assert.Equal(6, QuestionnaireScoreCalculator.CalculateScore(questions));
    }

    [Fact]
    public void AllQuestionsAnswered_ReturnsFalse_WhenAnyQuestionUnanswered()
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> questions =
        [
            new([1]),
            new([])
        ];

        Assert.False(QuestionnaireScoreCalculator.AllQuestionsAnswered(questions));
    }
}
