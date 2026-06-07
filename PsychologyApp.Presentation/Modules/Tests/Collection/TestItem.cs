using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public interface ITestBuilder
{
    ITestBuilder SetTestId(string testId);
    ITestBuilder SetAnalyzerId(string analyzerId);
    ITestBuilder SetTitle(string title);
    ITestBuilder SetSubtitle(string subtitle);
    ITestBuilder SetDescription(string description);
    ITestBuilder SetComment(string comment);
    ITestBuilder SetAlgorithm(List<string> algorithms);
    ITestBuilder ConfigureYesNoQuestionnaire(INavigationService navigationService, Func<int, string> scoreAnalyzer, List<string> baseAnswers, List<int> baseBalls, List<string> baseContexts, bool singleAnswer);
    ITestBuilder SetActionQuestions(INavigationService navigationService, Func<int, string> scoreAnalyzer, List<Question> questions, bool singleAnswer, string testId, string? analyzerId = null);
    TestItem Build();
}

public sealed class TestBuilder : ITestBuilder
{
    private TestItem TestItem { get; } = new();

    public TestItem Build() => TestItem;

    public ITestBuilder ConfigureYesNoQuestionnaire(INavigationService navigationService, Func<int, string> scoreAnalyzer, List<string> baseAnswers, List<int> baseBalls, List<string> baseContexts, bool singleAnswer)
    {
        if (baseAnswers.Count != baseBalls.Count)
        {
            throw new ArgumentException();
        }

        List<Question> questions = [];

        for (int contextIndex = 0; contextIndex < baseContexts.Count; contextIndex++)
        {
            List<Answer> answers = [];

            for (int answerIndex = 0; answerIndex < baseAnswers.Count; answerIndex++)
            {
                answers.Add(new Answer
                {
                    Number = contextIndex + 1,
                    Ball = baseBalls[answerIndex],
                    Text = baseAnswers[answerIndex],
                    Selected = false
                });
            }

            questions.Add(new Question
            {
                Number = contextIndex + 1,
                Answers = answers,
                Context = baseContexts[contextIndex],
            });
        }

        return SetActionQuestions(navigationService, scoreAnalyzer, questions, singleAnswer, definitionTestId: null);
    }

    public ITestBuilder SetActionQuestions(INavigationService navigationService, Func<int, string> scoreAnalyzer, List<Question> questions, bool singleAnswer, string testId, string? analyzerId = null)
    {
        TestItem.TestId = testId;
        TestItem.AnalyzerId = analyzerId;
        TestItem.Action = () => navigationService
            .GoToQuestionPageAsync(
                questions,
                scoreAnalyzer,
                singleAnswer,
                new TestSessionInfo { TestId = testId, AnalyzerId = analyzerId })
            .FireAndForget();
        return this;
    }

    private ITestBuilder SetActionQuestions(INavigationService navigationService, Func<int, string> scoreAnalyzer, List<Question> questions, bool singleAnswer, string? definitionTestId)
    {
        string testId = definitionTestId ?? Slugify(TestItem.Title);
        return SetActionQuestions(navigationService, scoreAnalyzer, questions, singleAnswer, testId, TestItem.AnalyzerId);
    }

    private static string Slugify(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToLowerInvariant();

    public ITestBuilder SetAlgorithm(List<string> algorithms)
    {
        TestItem.Algorithm = algorithms;
        return this;
    }

    public ITestBuilder SetComment(string comment)
    {
        TestItem.Comment = comment;
        return this;
    }

    public ITestBuilder SetDescription(string description)
    {
        TestItem.Description = description;
        return this;
    }

    public ITestBuilder SetSubtitle(string subtitle)
    {
        TestItem.Subtitle = subtitle;
        return this;
    }

    public ITestBuilder SetTitle(string title)
    {
        TestItem.Title = title;
        return this;
    }

    public ITestBuilder SetTestId(string testId)
    {
        TestItem.TestId = testId;
        return this;
    }

    public ITestBuilder SetAnalyzerId(string analyzerId)
    {
        TestItem.AnalyzerId = analyzerId;
        return this;
    }
}

public class TestItem
{
    public string TestId { get; set; } = string.Empty;
    public string? AnalyzerId { get; set; }
    public string Title { get; set; } = default!;
    public string Subtitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Comment { get; set; } = default!;
    public List<string> Algorithm { get; set; } = default!;
    public Action Action { get; set; } = default!;
    public string? LastResultSummary { get; set; }
    public bool HasLastResult => !string.IsNullOrWhiteSpace(LastResultSummary);

    public static TestBuilder CreateBuilder() => new();
}

