using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Features.Chat.Index;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Pages.Chat.Conversation;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Widgets.ChatHero;

namespace PsychologyApp.Presentation.Features.Chat.DependencyInjection;

public interface IChatViewModelFactory : IChatHeroFactory
{
    ChatViewModel CreateConversation(long? sessionId, INavigation navigation);

    ChatListViewModel CreateList(INavigation navigation);

    CompanionProfileViewModel CreateProfile(INavigation navigation);
}

public sealed class ChatViewModelFactory(
    IChatService chat,
    IChatLanguageProvider language,
    IDialogService dialogs,
    TimeProvider time,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IChatViewModelFactory
{
    public ChatViewModel CreateConversation(long? sessionId, INavigation navigation) =>
        new(chat, Resolve(navigation), language, time, sessionId);

    public ChatListViewModel CreateList(INavigation navigation) =>
        new(chat, Resolve(navigation), language, dialogs, time);

    public CompanionProfileViewModel CreateProfile(INavigation navigation) =>
        new(chat, Resolve(navigation), language, dialogs);

    public ChatHeroViewModel CreateHero(INavigationService navigationService) =>
        new(chat, navigationService);

    private INavigationService Resolve(INavigation navigation) =>
        navigationServiceFactory(NavigationContext.From(navigation));
}
