using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Domain.Tests.Tests;

public sealed class QuestionnaireScoreCalculatorTests
{
    [Fact]
    public void CalculateScore_TakesTheHighestStatementOfEachGroup()
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> questions =
        [
            new([1, 2]),
            new([3])
        ];

        Assert.Equal(5, QuestionnaireScoreCalculator.CalculateScore(questions));
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
    [Fact]
    public void CalculateScore_TickingEveryStatementOfAGroupNeverExceedsItsHighest()
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> questions = [new([0, 1, 2, 3]), new([0, 1, 2, 3])];

        Assert.Equal(6, QuestionnaireScoreCalculator.CalculateScore(questions));
    }

    [Fact]
    public void CalculateScore_AnUnansweredGroupAddsNothing()
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> questions = [new([2]), new([])];

        Assert.Equal(2, QuestionnaireScoreCalculator.CalculateScore(questions));
    }
}
