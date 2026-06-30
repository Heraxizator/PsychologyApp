using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Application.Tests;

public sealed record QuestionnaireScoringResult(
    int Score,
    int BandIndex,
    TechniqueId? RecommendedTechnique,
    string? AnalyzerId);

public interface IQuestionnaireScoringService
{
    bool TryValidateAllAnswered(IEnumerable<Question> questions);

    QuestionnaireScoringResult Calculate(IEnumerable<Question> questions, string? analyzerId);
}

public sealed class QuestionnaireScoringService : IQuestionnaireScoringService
{
    public bool TryValidateAllAnswered(IEnumerable<Question> questions) =>
        QuestionnaireScoreCalculator.AllQuestionsAnswered(ToAnswerModels(questions));

    public QuestionnaireScoringResult Calculate(IEnumerable<Question> questions, string? analyzerId)
    {
        IReadOnlyList<QuestionnaireQuestionAnswers> answerModels = ToAnswerModels(questions);
        int score = QuestionnaireScoreCalculator.CalculateScore(answerModels);
        int bandIndex = TestScoreInterpreter.GetBandIndex(analyzerId, score);
        TechniqueId? recommended = analyzerId is not null
            ? TestScoreRecommendation.RecommendTechnique(analyzerId, score)
            : null;

        return new QuestionnaireScoringResult(score, bandIndex, recommended, analyzerId);
    }

    private static IReadOnlyList<QuestionnaireQuestionAnswers> ToAnswerModels(IEnumerable<Question> questions) =>
        questions
            .Select(question => new QuestionnaireQuestionAnswers(
                question.Answers.Where(answer => answer.Selected).Select(answer => answer.Ball).ToList()))
            .ToList();
}
