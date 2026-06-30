namespace PsychologyApp.Domain.Tests;

public static class QuestionnaireScoreCalculator
{
    public static int CalculateScore(IReadOnlyList<QuestionnaireQuestionAnswers> questions) =>
        questions.SelectMany(question => question.SelectedBalls).Sum();

    public static bool AllQuestionsAnswered(IReadOnlyList<QuestionnaireQuestionAnswers> questions) =>
        questions.Count > 0 && questions.All(question => question.SelectedBalls.Count > 0);
}
