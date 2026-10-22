using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Pages.Chat.Conversation;

namespace PsychologyApp.Presentation.Features.Chat;

public interface IChatPageFactory
{
    ChatListPage CreateChatListPage(INavigation? hostNavigation);

    /// <param name="sessionId">The chat to open, or null to start a new one.</param>
    ChatPage CreateChatPage(long? sessionId, INavigation hostNavigation, string? topic = null, string? opener = null);

    CompanionProfilePage CreateCompanionProfilePage(INavigation hostNavigation);
}

public sealed class ChatPageFactory(IChatViewModelFactory viewModelFactory) : IChatPageFactory
{
    public ChatListPage CreateChatListPage(INavigation? hostNavigation) => new(viewModelFactory, hostNavigation);

    public ChatPage CreateChatPage(long? sessionId, INavigation hostNavigation, string? topic = null, string? opener = null) =>
        new(viewModelFactory, sessionId, hostNavigation, topic, opener);

    public CompanionProfilePage CreateCompanionProfilePage(INavigation hostNavigation) => new(viewModelFactory, hostNavigation);
}
