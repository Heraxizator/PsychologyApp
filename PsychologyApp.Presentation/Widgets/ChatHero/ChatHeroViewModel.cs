using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Widgets.ChatHero;

/// <summary>The prominent "Talk" card at the top of the home screen: resume the last conversation, start a new one or see all chats.</summary>
public sealed class ChatHeroViewModel : BaseViewModel
{
    private const int PreviewLength = 90;

    private readonly IChatService _chat;
    private long? _lastChatId;
    private string _subtitle = AppStrings.ChatHeroFreshSubtitle;
    private string _preview = string.Empty;
    private bool _hasLastChat;

    public ChatHeroViewModel(IChatService chat, INavigationService navigationService)
    {
        _chat = chat;
        BindNavigation(navigationService);

        PrimaryCommand = new AsyncCommand(() => navigationService.GoToChatAsync(_lastChatId));
        NewChatCommand = new AsyncCommand(() => navigationService.GoToChatAsync(null));
        AllChatsCommand = new AsyncCommand(() => navigationService.GoToChatListAsync());
    }

    public ICommand PrimaryCommand { get; }
    public ICommand NewChatCommand { get; }
    public ICommand AllChatsCommand { get; }

    public string Title => AppStrings.ChatHeroTitle;
    public string PrimaryText => _hasLastChat ? AppStrings.ChatHeroContinue : AppStrings.ChatHeroStart;
    public string NewChatText => AppStrings.ChatHeroNewChat;
    public string AllChatsText => AppStrings.ChatHeroAllChats;

    public string Subtitle
    {
        get => _subtitle;
        private set => SetProperty(ref _subtitle, value);
    }

    public string Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public bool HasLastChat
    {
        get => _hasLastChat;
        private set
        {
            if (SetProperty(ref _hasLastChat, value))
            {
                Notify(nameof(PrimaryText));
            }
        }
    }

    public async Task RefreshAsync()
    {
        ChatSessionDTO? last = await _chat.GetLastChatAsync();
        _lastChatId = last?.Id;
        HasLastChat = last is not null;
        Subtitle = last?.Title ?? AppStrings.ChatHeroFreshSubtitle;
        Preview = Shorten(last?.Preview);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(Title), nameof(PrimaryText), nameof(NewChatText), nameof(AllChatsText));
        if (!_hasLastChat)
        {
            Subtitle = AppStrings.ChatHeroFreshSubtitle;
        }
    }

    private static string Shorten(string? text)
    {
        string flat = (text ?? string.Empty).Replace('\n', ' ').Trim();
        return flat.Length <= PreviewLength ? flat : flat[..PreviewLength].TrimEnd() + "…";
    }
}
