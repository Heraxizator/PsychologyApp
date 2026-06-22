using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.ViewModels.Practice.Techniques;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Services.Factories;

public interface ITestsListViewModelFactory
{
    TestsListViewModel Create(INavigation navigation);
}

public sealed class TestsListViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IDatabaseReadySignal databaseReadySignal,
    TestsListLoader testsListLoader,
    ILogger<TestsListViewModel> logger) : ViewModelFactoryBase, ITestsListViewModelFactory
{
    public TestsListViewModel Create(INavigation navigation) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            databaseReadySignal,
            testsListLoader,
            logger);
}

public interface ITestHistoryViewModelFactory
{
    TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle);
}

public sealed class TestHistoryViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService,
    ITestCatalogService testCatalogService,
    IDatabaseReadySignal databaseReadySignal,
    TestHistoryLoader historyLoader,
    TestRetakeOperations retakeOperations,
    ILogger<TestHistoryViewModel> logger) : ViewModelFactoryBase, ITestHistoryViewModelFactory
{
    public TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            userProgressService,
            testCatalogService,
            databaseReadySignal,
            historyLoader,
            retakeOperations,
            logger,
            testId,
            testTitle);
}

public interface ILuscherTestViewModelFactory
{
    LuscherTestViewModel Create(INavigation navigation, LuscherMode mode);
}

public sealed class LuscherTestViewModelFactory(
    IUserProgressService userProgressService,
    LuscherTestSubmissionService submissionService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ILuscherTestViewModelFactory
{
    public LuscherTestViewModel Create(INavigation navigation, LuscherMode mode) =>
        new(
            mode,
            ResolveNavigation(navigationServiceFactory, navigation),
            userProgressService,
            submissionService);
}

public interface ITestResultViewModelFactory
{
    TestResultViewModel Create(INavigation navigation, TestResultInfo result);
}

public sealed class TestResultViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITestCatalogService testCatalogService,
    TestRetakeOperations retakeOperations) : ViewModelFactoryBase, ITestResultViewModelFactory
{
    public TestResultViewModel Create(INavigation navigation, TestResultInfo result) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            testCatalogService,
            retakeOperations,
            result);
}

public interface IQuestionViewModelFactory
{
    QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null);
}

public sealed class QuestionViewModelFactory(
    IToastService toastService,
    IDialogService dialogService,
    IUserProgressService userProgressService,
    QuestionnaireSubmissionService submissionService,
    ITestCatalogService testCatalogService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IQuestionViewModelFactory
{
    public QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        INavigationService navigationService = ResolveNavigation(navigationServiceFactory, navigation);
        return new(
            questions,
            analyzer,
            singleAnswer,
            toastService,
            dialogService,
            navigationService,
            userProgressService,
            submissionService,
            testCatalogService,
            session);
    }
}

public interface IFindProblemViewModelFactory
{
    FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null);
}

public sealed class FindProblemViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITestCatalogService testCatalogService,
    FindProblemContentLoader contentLoader) : ViewModelFactoryBase, IFindProblemViewModelFactory
{
    public FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            testCatalogService,
            contentLoader,
            description,
            algorithm,
            comment,
            startTest,
            testId);
}

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ITheoryViewModelFactory
{
    public TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null) =>
        new(ResolveNavigation(navigationServiceFactory, navigation), content, techniqueId);
}
