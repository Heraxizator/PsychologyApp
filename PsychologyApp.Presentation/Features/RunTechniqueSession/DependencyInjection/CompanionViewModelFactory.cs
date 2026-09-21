using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Companion;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface ICompanionViewModelFactory
{
    CompanionViewModel Create(INavigation navigation);
}

public sealed class CompanionViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ISituationAnalyzer analyzer,
    ICrisisDetector crisisDetector,
    ILocalLanguageModel model) : ICompanionViewModelFactory
{
    public CompanionViewModel Create(INavigation navigation) =>
        new(navigationServiceFactory(NavigationContext.From(navigation)), analyzer, crisisDetector, model);
}
