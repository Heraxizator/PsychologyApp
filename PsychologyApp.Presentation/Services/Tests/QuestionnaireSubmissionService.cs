using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed record QuestionnaireSubmission(
    int Score,
    string Interpretation,
    TechniqueId? RecommendedTechnique,
    string? InterpretationDetail);

public sealed class QuestionnaireSubmissionService
{
    public bool TryValidateAllAnswered(IEnumerable<Question> questions) =>
        questions.All(question => question.Answers.Any(answer => answer.Selected is true));

    public QuestionnaireSubmission Calculate(
        IEnumerable<Question> questions,
        Func<int, string> analyzer,
        TestSessionInfo? session)
    {
        int score = questions
            .SelectMany(question => question.Answers)
            .Where(answer => answer.Selected is true)
            .Sum(answer => answer.Ball);

        string interpretation = analyzer.Invoke(score);
        string? analyzerId = session?.AnalyzerId;
        TechniqueId? recommended = analyzerId is not null
            ? TestScoreAnalyzers.RecommendTechnique(analyzerId, score)
            : null;
        string? interpretationDetail = analyzerId is not null
            ? TestScoreAnalyzers.ResolveDetail(analyzerId)?.Invoke(score)
            : null;

        return new QuestionnaireSubmission(score, interpretation, recommended, interpretationDetail);
    }

    public Task SaveAsync(
        IUserProgressService progress,
        TestSessionInfo session,
        int score,
        string summary,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(session.TestId))
        {
            return Task.CompletedTask;
        }

        return progress.SaveTestResultAsync(session.TestId, score, summary, detailJson: null, cancellationToken);
    }
}
