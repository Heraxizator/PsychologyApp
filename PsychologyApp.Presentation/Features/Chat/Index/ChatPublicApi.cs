using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Widgets.ChatHero;

namespace PsychologyApp.Presentation.Features.Chat.Index;

/// <summary>
/// Public entry point for the Chat slice.
/// Other slices (the home screen) may import this namespace only.
/// </summary>
public interface IChatHeroFactory
{
    /// <summary>Creates the prominent "Talk" card for the home screen.</summary>
    ChatHeroViewModel CreateHero(INavigationService navigationService);
}
