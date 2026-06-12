using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Views.Practice.Techniques;
using PsychologyApp.Presentation.Views.Tests;

namespace PsychologyApp.Presentation.Services;

public interface ITestPageFactory
{
    TestsListPage CreateTestsListPage();
    TestHistoryPage CreateTestHistoryPage(string testId, string testTitle);
    TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null);
    FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null);
    QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null);
    StandardTestPage CreateStandardTestPage();
    AlternativeTestPage CreateAlternativeTestPage();
    TestResultPage CreateTestResultPage(TestResultInfo result);
}

public sealed class TestPageFactory(
    IPageViewModelActivator pageViewModelActivator,
    ITestsListViewModelFactory testsListViewModelFactory,
    ITestHistoryViewModelFactory testHistoryViewModelFactory,
    ITheoryViewModelFactory theoryViewModelFactory,
    IFindProblemViewModelFactory findProblemViewModelFactory,
    IQuestionViewModelFactory questionViewModelFactory,
    ILuscherTestViewModelFactory luscherTestViewModelFactory,
    ITestResultViewModelFactory testResultViewModelFactory) : ITestPageFactory
{
    public TestsListPage CreateTestsListPage() =>
        new(pageViewModelActivator, testsListViewModelFactory);

    public TestHistoryPage CreateTestHistoryPage(string testId, string testTitle) =>
        new(testHistoryViewModelFactory, testId, testTitle);

    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        new(theoryViewModelFactory, content, techniqueId);

    public FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        new(findProblemViewModelFactory, description, algorithm, comment, startTest, testId);

    public QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        new(questionViewModelFactory, questions, scoreAnalyzer, singleAnswer, session);

    public StandardTestPage CreateStandardTestPage() =>
        new(pageViewModelActivator, luscherTestViewModelFactory);

    public AlternativeTestPage CreateAlternativeTestPage() =>
        new(pageViewModelActivator, luscherTestViewModelFactory);

    public TestResultPage CreateTestResultPage(TestResultInfo result) =>
        new(testResultViewModelFactory, result);
}
