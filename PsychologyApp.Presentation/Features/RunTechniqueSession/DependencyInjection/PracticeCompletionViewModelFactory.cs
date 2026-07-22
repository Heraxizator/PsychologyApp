using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface IPracticeCompletionViewModelFactory
{
    PracticeCompletionViewModel Create(ContentPage page, int streakDays, string? completedItemKey = null, long? sessionResultId = null);
}

public sealed class PracticeCompletionViewModelFactory(
    IUserProgressService userProgressService,
    NextPracticeResolver nextPracticeResolver,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IPracticeCompletionViewModelFactory
{
    public PracticeCompletionViewModel Create(
        ContentPage page,
        int streakDays,
        string? completedItemKey = null,
        long? sessionResultId = null) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            userProgressService,
            nextPracticeResolver,
            streakDays,
            completedItemKey,
            sessionResultId);
}
