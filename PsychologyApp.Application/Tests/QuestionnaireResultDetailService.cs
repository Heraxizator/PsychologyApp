using System.Text.Json;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Serialization;

namespace PsychologyApp.Application.Tests;

public sealed record QuestionnaireDetailBuildRequest(
    IEnumerable<Question> Questions,
    string TestId,
    string? AnalyzerId,
    DateTime StartedAtUtc,
    DateTime CompletedAtUtc);

public interface IQuestionnaireResultDetailService
{
    Task<QuestionnaireResultDetail?> BuildAsync(
        QuestionnaireDetailBuildRequest request,
        ITestCatalogService testCatalogService,
        CancellationToken cancellationToken = default);

    string? Serialize(QuestionnaireResultDetail? detail);

    QuestionnaireResultDetail? TryParse(string? detailJson);
}

public sealed class QuestionnaireResultDetailService : IQuestionnaireResultDetailService
{
    public async Task<QuestionnaireResultDetail?> BuildAsync(
        QuestionnaireDetailBuildRequest request,
        ITestCatalogService testCatalogService,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TestId))
        {
            return null;
        }

        string? construct = null;
        try
        {
            TestDefinition? definition = await testCatalogService.GetByIdAsync(request.TestId, cancellationToken);
            construct = definition?.Construct;
        }
        catch
        {
            // Non-critical: do not block saving result if catalog lookup fails.
        }

        int durationSeconds = Math.Max(
            0,
            (int)request.CompletedAtUtc.Subtract(request.StartedAtUtc).TotalSeconds);

        return new QuestionnaireResultDetail(
            TestId: request.TestId,
            AnalyzerId: request.AnalyzerId,
            CompletedAtUtc: request.CompletedAtUtc,
            DurationSeconds: durationSeconds,
            Construct: construct,
            Questions: request.Questions.Select(question => new QuestionnaireResultQuestion(
                QuestionNumber: question.Number,
                Context: question.Context,
                SelectedAnswerBalls: question.Answers.Where(answer => answer.Selected).Select(answer => answer.Ball).ToList(),
                SelectedAnswerTexts: question.Answers.Where(answer => answer.Selected).Select(answer => answer.Text ?? string.Empty).ToList()
            )).ToList());
    }

    public string? Serialize(QuestionnaireResultDetail? detail) =>
        detail is null
            ? null
            : JsonSerializer.Serialize(detail, TestJsonSerializerContext.Default.QuestionnaireResultDetail);

    public QuestionnaireResultDetail? TryParse(string? detailJson)
    {
        if (string.IsNullOrWhiteSpace(detailJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize(detailJson, TestJsonSerializerContext.Default.QuestionnaireResultDetail);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
