using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;
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
    IUserProgressService userProgressService) : ITestsListViewModelFactory
{
    public TestsListViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), userProgressService);
}

public interface ITestHistoryViewModelFactory
{
    TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle);
}

public sealed class TestHistoryViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService) : ITestHistoryViewModelFactory
{
    public TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), userProgressService, testId, testTitle);
}

public interface IStandardTestViewModelFactory
{
    StandardTestViewModel Create(INavigation navigation);
}

public sealed class StandardTestViewModelFactory(
    IUserProgressService userProgressService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IStandardTestViewModelFactory
{
    public StandardTestViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), userProgressService);
}

public interface IAlternativeTestViewModelFactory
{
    AlternativeTestViewModel Create(INavigation navigation);
}

public sealed class AlternativeTestViewModelFactory(
    IUserProgressService userProgressService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IAlternativeTestViewModelFactory
{
    public AlternativeTestViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), userProgressService);
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
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IQuestionViewModelFactory
{
    public QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        INavigationService navigationService = navigationServiceFactory(NavigationContext.From(navigation));
        return new(
            navigation,
            questions,
            analyzer,
            singleAnswer,
            toastService,
            dialogService,
            navigationService,
            userProgressService,
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
        Action action,
        string? testId = null);
}

public sealed class FindProblemViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IFindProblemViewModelFactory
{
    public FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action,
        string? testId = null) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), description, algorithm, comment, action, testId);
}

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ITheoryViewModelFactory
{
    public TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), content, techniqueId);
}
