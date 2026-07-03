using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Pages.RunTests.TestResult;
using PsychologyApp.Presentation.Pages.RunTests.AlternativeTest;
using PsychologyApp.Presentation.Pages.RunTests.StandardTest;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Pages.RunTests.FindProblem;
using PsychologyApp.Presentation.Pages.RunTests.TestHistory;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;

namespace PsychologyApp.Presentation.Features.RunTests;

public interface ITestPageFactory
{
    TestsListPage CreateTestsListPage();
    TestHistoryPage CreateTestHistoryPage(string testId, string testTitle);
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
    IFindProblemViewModelFactory findProblemViewModelFactory,
    IQuestionViewModelFactory questionViewModelFactory,
    ILuscherTestViewModelFactory luscherTestViewModelFactory,
    ITestResultViewModelFactory testResultViewModelFactory) : ITestPageFactory
{
    public TestsListPage CreateTestsListPage() =>
        new(pageViewModelActivator, testsListViewModelFactory);

    public TestHistoryPage CreateTestHistoryPage(string testId, string testTitle) =>
        new(testHistoryViewModelFactory, testId, testTitle);

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
