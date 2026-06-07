using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.Views.TechniquePages;
using PsychologyApp.Presentation.Views.Tests;

namespace PsychologyApp.Presentation.Services;

public interface ITestPageFactory
{
    TestsListPage CreateTestsListPage();
    TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null);
    FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Action action, string? testId = null);
    QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null);
    StandardTestPage CreateStandardTestPage();
    AlternativeTestPage CreateAlternativeTestPage();
}

public sealed class TestPageFactory(
    IPageViewModelActivator pageViewModelActivator,
    ITestsListViewModelFactory testsListViewModelFactory,
    ITheoryViewModelFactory theoryViewModelFactory,
    IFindProblemViewModelFactory findProblemViewModelFactory,
    IQuestionViewModelFactory questionViewModelFactory,
    IStandardTestViewModelFactory standardTestViewModelFactory,
    IAlternativeTestViewModelFactory alternativeTestViewModelFactory) : ITestPageFactory
{
    public TestsListPage CreateTestsListPage() =>
        new(pageViewModelActivator, testsListViewModelFactory);

    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        new(theoryViewModelFactory, content, techniqueId);

    public FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Action action, string? testId = null) =>
        new(findProblemViewModelFactory, description, algorithm, comment, action, testId);

    public QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        new(questionViewModelFactory, questions, scoreAnalyzer, singleAnswer, session);

    public StandardTestPage CreateStandardTestPage() =>
        new(pageViewModelActivator, standardTestViewModelFactory);

    public AlternativeTestPage CreateAlternativeTestPage() =>
        new(pageViewModelActivator, alternativeTestViewModelFactory);
}
