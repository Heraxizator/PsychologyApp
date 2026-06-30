using System.Text.Json.Serialization;

namespace PsychologyApp.Application.Models.Tests;

public sealed record QuestionnaireResultDetail(
    string TestId,
    string? AnalyzerId,
    DateTime CompletedAtUtc,
    int DurationSeconds,
    string? Construct,
    IReadOnlyList<QuestionnaireResultQuestion> Questions);

public sealed record QuestionnaireResultQuestion(
    int QuestionNumber,
    string? Context,
    IReadOnlyList<int> SelectedAnswerBalls,
    IReadOnlyList<string> SelectedAnswerTexts);
