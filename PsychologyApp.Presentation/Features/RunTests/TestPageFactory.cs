using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.TestResult;
using PsychologyApp.Presentation.Pages.AlternativeTest;
using PsychologyApp.Presentation.Pages.StandardTest;
using PsychologyApp.Presentation.Pages.Question;
using PsychologyApp.Presentation.Pages.FindProblem;
using PsychologyApp.Presentation.Pages.TestHistory;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.TestsList;

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
