using PsychologyApp.Presentation.Pages.FindProblem;
using PsychologyApp.Presentation.Pages.Question;
using PsychologyApp.Presentation.Pages.TestResult;
using PsychologyApp.Presentation.Pages.LuscherTest;
using PsychologyApp.Presentation.Pages.TestHistory;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.TestsList;

namespace PsychologyApp.Presentation.App.Providers;

public interface ITestsListViewModelFactory
{
    TestsListViewModel Create(ContentPage page);
}

public sealed class TestsListViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IDatabaseReadySignal databaseReadySignal,
    TestsListLoader testsListLoader,
    ILogger<TestsListViewModel> logger) : ViewModelFactoryBase, ITestsListViewModelFactory
{
    public TestsListViewModel Create(ContentPage page) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            databaseReadySignal,
            testsListLoader,
            logger);
}

public interface ITestHistoryViewModelFactory
{
    TestHistoryViewModel Create(ContentPage page, string testId, string testTitle);
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
    public TestHistoryViewModel Create(ContentPage page, string testId, string testTitle) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
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
    LuscherTestViewModel Create(ContentPage page, LuscherMode mode);
}

public sealed class LuscherTestViewModelFactory(
    IUserProgressService userProgressService,
    ILuscherResultService luscherResultService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ILuscherTestViewModelFactory
{
    public LuscherTestViewModel Create(ContentPage page, LuscherMode mode) =>
        new(
            mode,
            ResolveNavigation(navigationServiceFactory, page),
            userProgressService,
            luscherResultService);
}

public interface ITestResultViewModelFactory
{
    TestResultViewModel Create(ContentPage page, TestResultInfo result);
}

public sealed class TestResultViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITestCatalogService testCatalogService,
    TechniqueCatalogGateway techniqueCatalog,
    TestRetakeOperations retakeOperations,
    IUserProgressService userProgressService,
    TestTrendResolver trendResolver) : ViewModelFactoryBase, ITestResultViewModelFactory
{
    public TestResultViewModel Create(ContentPage page, TestResultInfo result) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            testCatalogService,
            techniqueCatalog,
            retakeOperations,
            userProgressService,
            trendResolver,
            result);
}

public interface IQuestionViewModelFactory
{
    QuestionViewModel Create(
        ContentPage page,
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
    TestRunCoordinator runCoordinator,
    ITestCatalogService testCatalogService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IQuestionViewModelFactory
{
    public QuestionViewModel Create(
        ContentPage page,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        INavigationService navigationService = ResolveNavigation(navigationServiceFactory, page);
        return new(
            questions,
            analyzer,
            singleAnswer,
            toastService,
            dialogService,
            navigationService,
            userProgressService,
            submissionService,
            runCoordinator,
            testCatalogService,
            session);
    }
}

public interface IFindProblemViewModelFactory
{
    FindProblemViewModel Create(
        ContentPage page,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null);
}

public sealed class FindProblemViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITestCatalogService testCatalogService,
    FindProblemContentLoader contentLoader,
    TestRunCoordinator testRunCoordinator) : ViewModelFactoryBase, IFindProblemViewModelFactory
{
    public FindProblemViewModel Create(
        ContentPage page,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            testCatalogService,
            contentLoader,
            testRunCoordinator,
            description,
            algorithm,
            comment,
            startTest,
            testId);
}
