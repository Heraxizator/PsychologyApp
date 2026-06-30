using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class QuestionnaireDetailBuilder(
    IQuestionnaireResultDetailService detailService,
    ITestCatalogService testCatalogService)
{
    public Task<QuestionnaireResultDetail?> BuildAsync(
        IEnumerable<Question> questions,
        TestSessionInfo session,
        DateTime startedAtUtc,
        CancellationToken cancellationToken = default) =>
        detailService.BuildAsync(
            new QuestionnaireDetailBuildRequest(
                questions,
                session.TestId,
                session.AnalyzerId,
                startedAtUtc,
                DateTime.UtcNow),
            testCatalogService,
            cancellationToken);

    public string? Serialize(QuestionnaireResultDetail? detail) => detailService.Serialize(detail);

    public async Task<string?> BuildJsonAsync(
        IEnumerable<Question> questions,
        TestSessionInfo session,
        DateTime startedAtUtc,
        CancellationToken cancellationToken = default)
    {
        QuestionnaireResultDetail? detail = await BuildAsync(questions, session, startedAtUtc, cancellationToken);
        return Serialize(detail);
    }
}
