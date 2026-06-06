namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public sealed record JsonNavigationTestDefinition(
    string Title,
    string Subtitle,
    string Description,
    List<string> Algorithm,
    string Comment,
    string NavigationTarget);

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
    bool SingleAnswer);

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
    List<JsonGroupedQuestionDefinition> Questions);
