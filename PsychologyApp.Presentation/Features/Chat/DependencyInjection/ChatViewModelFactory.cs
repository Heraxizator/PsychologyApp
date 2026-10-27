using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Pages.Chat.Conversation;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.Chat.DependencyInjection;

public interface IChatViewModelFactory
{
    ChatViewModel CreateConversation(long? sessionId, INavigation navigation, string? topic = null, string? opener = null);

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
    public ChatViewModel CreateConversation(long? sessionId, INavigation navigation, string? topic = null, string? opener = null) =>
        new(chat, Resolve(navigation), language, time, sessionId, topic, opener);

    public ChatListViewModel CreateList(INavigation navigation) =>
        new(chat, Resolve(navigation), language, dialogs, time);

    public CompanionProfileViewModel CreateProfile(INavigation navigation) =>
        new(chat, Resolve(navigation), dialogs);

    private INavigationService Resolve(INavigation navigation) =>
        navigationServiceFactory(NavigationContext.From(navigation));
}
