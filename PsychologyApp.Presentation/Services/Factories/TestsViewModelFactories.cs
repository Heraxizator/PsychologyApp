using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Services.Factories;

public interface ITestsListViewModelFactory
{
    TestsListViewModel Create(INavigation navigation);
}

public sealed class TestsListViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : ITestsListViewModelFactory
{
    public TestsListViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation));
}

public interface IStandardTestViewModelFactory
{
    StandardTestViewModel Create(INavigation navigation);
}

public sealed class StandardTestViewModelFactory : IStandardTestViewModelFactory
{
    public StandardTestViewModel Create(INavigation navigation) => new(navigation);
}

public interface IAlternativeTestViewModelFactory
{
    AlternativeTestViewModel Create(INavigation navigation);
}

public sealed class AlternativeTestViewModelFactory : IAlternativeTestViewModelFactory
{
    public AlternativeTestViewModel Create(INavigation navigation) => new(navigation);
}

public interface IQuestionViewModelFactory
{
    QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer);
}

public sealed class QuestionViewModelFactory(
    IToastService toastService,
    IDialogService dialogService,
    Func<INavigation, INavigationService> navigationServiceFactory) : IQuestionViewModelFactory
{
    public QuestionViewModel Create(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer) =>
        new(navigation, questions, analyzer, singleAnswer, toastService, dialogService, navigationServiceFactory(navigation));
}

public interface IFindProblemViewModelFactory
{
    FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action);
}

public sealed class FindProblemViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IFindProblemViewModelFactory
{
    public FindProblemViewModel Create(
        INavigation navigation,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action) =>
        new(navigation, navigationServiceFactory(navigation), description, algorithm, comment, action);
}

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(INavigation navigation, string content);
}

public sealed class TheoryViewModelFactory : ITheoryViewModelFactory
{
    public TheoryViewModel Create(INavigation navigation, string content) =>
        new(navigation, content);
}
