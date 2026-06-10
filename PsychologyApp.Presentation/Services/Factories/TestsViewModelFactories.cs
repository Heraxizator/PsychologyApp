using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Services.Factories;

public interface ITestsListViewModelFactory
{
    TestsListViewModel Create(INavigation navigation);
}

public sealed class TestsListViewModelFactory(
    Func<INavigation, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService) : ITestsListViewModelFactory
{
    public TestsListViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation), userProgressService);
}

public interface ITestHistoryViewModelFactory
{
    TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle);
}

public sealed class TestHistoryViewModelFactory(
    Func<INavigation, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService) : ITestHistoryViewModelFactory
{
    public TestHistoryViewModel Create(INavigation navigation, string testId, string testTitle) =>
        new(navigation, navigationServiceFactory(navigation), userProgressService, testId, testTitle);
}

public interface IStandardTestViewModelFactory
{
    StandardTestViewModel Create(INavigation navigation);
}

public sealed class StandardTestViewModelFactory(IUserProgressService userProgressService) : IStandardTestViewModelFactory
{
    public StandardTestViewModel Create(INavigation navigation) => new(navigation, userProgressService);
}

public interface IAlternativeTestViewModelFactory
{
    AlternativeTestViewModel Create(INavigation navigation);
}

public sealed class AlternativeTestViewModelFactory(IUserProgressService userProgressService) : IAlternativeTestViewModelFactory
{
    public AlternativeTestViewModel Create(INavigation navigation) => new(navigation, userProgressService);
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
    Func<INavigation, INavigationService> navigationServiceFactory) : IQuestionViewModelFactory
{
    public QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        INavigationService navigationService = navigationServiceFactory(navigation);
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

public sealed class FindProblemViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IFindProblemViewModelFactory
{
    public FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action,
        string? testId = null) =>
        new(navigation, navigationServiceFactory(navigation), description, algorithm, comment, action, testId);
}

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory : ITheoryViewModelFactory
{
    public TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null) =>
        new(navigation, content, techniqueId);
}
