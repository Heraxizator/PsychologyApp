using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record QuestionnaireSubmission(
    int Score,
    string Interpretation,
    TechniqueId? RecommendedTechnique,
    string? InterpretationDetail);

public sealed class QuestionnaireSubmissionService(IQuestionnaireScoringService scoringService)
{
    public bool TryValidateAllAnswered(IEnumerable<Question> questions) =>
        scoringService.TryValidateAllAnswered(questions);

    public QuestionnaireSubmission Calculate(
        IEnumerable<Question> questions,
        string? analyzerId)
    {
        QuestionnaireScoringResult result = scoringService.Calculate(questions, analyzerId);
        string interpretation = TestScoreLabelMapper.GetSummary(analyzerId, result.Score) ?? string.Empty;
        string? interpretationDetail = TestScoreLabelMapper.GetDetail(analyzerId, result.Score);

        return new QuestionnaireSubmission(
            result.Score,
            interpretation,
            result.RecommendedTechnique,
            interpretationDetail);
    }

    public Task SaveAsync(
        IUserProgressService progress,
        TestSessionInfo session,
        int score,
        string summary,
        CancellationToken cancellationToken,
        string? detailJson = null)
    {
        if (string.IsNullOrWhiteSpace(session.TestId))
        {
            return Task.CompletedTask;
        }

        return progress.SaveTestResultAsync(session.TestId, score, summary, detailJson: detailJson, cancellationToken);
    }
}
