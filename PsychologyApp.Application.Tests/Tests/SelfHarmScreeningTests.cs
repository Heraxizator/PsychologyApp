using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public class SelfHarmScreeningTests
{
    private static List<Question> Questionnaire(int count, int answeredItemIndex, params int[] selectedBalls)
    {
        List<Question> questions = [];
        for (int i = 0; i < count; i++)
        {
            Question question = new() { Number = i + 1, Answers = [.. Enumerable.Range(0, 4).Select(b => new Answer { Ball = b })] };
            if (i == answeredItemIndex)
            {
                foreach (Answer answer in question.Answers.Where(a => selectedBalls.Contains(a.Ball)))
                {
                    answer.Selected = true;
                }
            }
            else
            {
                question.Answers[0].Selected = true;
            }

            questions.Add(question);
        }

        return questions;
    }

    [Theory]
    [InlineData("beck", 21, 1)]
    [InlineData("beck", 21, 3)]
    [InlineData("phq9", 9, 1)]
    [InlineData("phq9", 9, 2)]
    public void AnyAnswerOtherThanNeverToTheSuicideItemIsEndorsed(string analyzer, int count, int ball) =>
        Assert.True(SelfHarmScreening.IsEndorsed(analyzer, Questionnaire(count, 8, ball)));

    [Theory]
    [InlineData("beck", 21)]
    [InlineData("phq9", 9)]
    public void NeverIsNotEndorsed(string analyzer, int count) =>
        Assert.False(SelfHarmScreening.IsEndorsed(analyzer, Questionnaire(count, 8, 0)));

    [Fact]
    public void AMultiSelectGroupCountsWhenAnyTickedStatementIsAbove0()
    {
        Assert.True(SelfHarmScreening.IsEndorsed("beck", Questionnaire(21, 8, 0, 2)));
    }

    [Fact]
    public void OtherItemsDoNotTriggerIt()
    {
        List<Question> questions = Questionnaire(21, 3, 3);

        Assert.False(SelfHarmScreening.IsEndorsed("beck", questions));
    }

    [Theory]
    [InlineData("gad7")]
    [InlineData("pss10")]
    [InlineData(null)]
    public void QuestionnairesWithoutSuchAnItemNeverTriggerIt(string? analyzer) =>
        Assert.False(SelfHarmScreening.IsEndorsed(analyzer, Questionnaire(9, 8, 3)));

    [Fact]
    public void ACatalogueOfAnUnexpectedLengthIsNotMisread() =>
        Assert.False(SelfHarmScreening.IsEndorsed("beck", Questionnaire(20, 8, 3)));

    [Fact]
    public void TheScoringServiceReportsIt()
    {
        QuestionnaireScoringResult result = new QuestionnaireScoringService().Calculate(Questionnaire(9, 8, 1), "phq9");

        Assert.True(result.SelfHarmItemEndorsed);
        Assert.Equal(1, result.Score);
    }

    [Fact]
    public void TheBeckScoreTakesTheHighestTickedStatementOfAGroup()
    {
        List<Question> questions = Questionnaire(21, 0, 1, 2, 3);

        QuestionnaireScoringResult result = new QuestionnaireScoringService().Calculate(questions, "beck");

        Assert.Equal(3, result.Score);
    }
}
