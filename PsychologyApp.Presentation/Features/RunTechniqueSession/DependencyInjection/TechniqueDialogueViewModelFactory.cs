using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface ITechniqueDialogueViewModelFactory
{
    TechniqueDialogueViewModel Create(TechniqueId techniqueId, INavigation navigation);
}

public sealed class TechniqueDialogueViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITechniqueCatalogService techniqueCatalogService,
    IUserProgressService userProgressService,
    IConversationScenarioProvider scenarioProvider,
    ICrisisDetector crisisDetector,
    TechniqueSessionCompletionService sessionCompletionService) : ITechniqueDialogueViewModelFactory
{
    public TechniqueDialogueViewModel Create(TechniqueId techniqueId, INavigation navigation) =>
        new(
            techniqueId,
            navigationServiceFactory(NavigationContext.From(navigation)),
            userProgressService,
            techniqueCatalogService,
            scenarioProvider,
            crisisDetector,
            sessionCompletionService);
}
