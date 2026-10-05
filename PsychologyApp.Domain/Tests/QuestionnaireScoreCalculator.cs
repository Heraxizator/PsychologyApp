namespace PsychologyApp.Domain.Tests;

public static class QuestionnaireScoreCalculator
{
    /// <summary>
    /// A question contributes one value: the highest of the statements chosen. In a single-answer question that is the only one;
    /// in a group where several statements may be ticked (the Beck inventory says "tick each one that fits"), the standard scoring
    /// takes the highest, so ticking more statements never pushes the total past what one answer per group could give.
    /// </summary>
    public static int CalculateScore(IReadOnlyList<QuestionnaireQuestionAnswers> questions) =>
        questions.Sum(question => question.SelectedBalls.Count == 0 ? 0 : question.SelectedBalls.Max());

    public static bool AllQuestionsAnswered(IReadOnlyList<QuestionnaireQuestionAnswers> questions) =>
        questions.Count > 0 && questions.All(question => question.SelectedBalls.Count > 0);
}
