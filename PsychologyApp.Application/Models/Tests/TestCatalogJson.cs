namespace PsychologyApp.Application.Models.Tests;

public sealed record JsonNavigationTestDefinition(
    string Title,
    string Subtitle,
    string Description,
    List<string> Algorithm,
    string Comment,
    string NavigationTarget,
    string? BetterWhen = null,
    int? EstimatedMinutes = null,
    int? QuestionCount = null,
    string? Construct = null);

public sealed record JsonSimpleQuestionnaireDefinition(
    string Title,
    string Subtitle,
    string Description,
    List<string> Algorithm,
    string Comment,
    string AnalyzerId,
    List<string> Answers,
    List<int> Balls,
    List<string> Questions,
    bool SingleAnswer,
    string? BetterWhen = null,
    int? EstimatedMinutes = null,
    int? QuestionCount = null,
    string? Construct = null);

public sealed record JsonAnswerDefinition(int Ball, string Text);

public sealed record JsonGroupedQuestionDefinition(List<JsonAnswerDefinition> Answers);

public sealed record JsonGroupedQuestionnaireDefinition(
    string Title,
    string Subtitle,
    string Description,
    List<string> Algorithm,
    string Comment,
    string AnalyzerId,
    bool SingleAnswer,
    List<JsonGroupedQuestionDefinition> Questions,
    string? BetterWhen = null,
    int? EstimatedMinutes = null,
    int? QuestionCount = null,
    string? Construct = null);
