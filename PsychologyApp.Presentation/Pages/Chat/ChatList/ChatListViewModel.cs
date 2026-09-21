using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.Chat.ChatList;

public sealed class ChatListItem
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required string Preview { get; init; }
    public required string TimeText { get; init; }

    /// <summary>"8 → 4" when tension was measured at least once, otherwise empty.</summary>
    public string MoodText { get; init; } = string.Empty;

    public bool HasMood => MoodText.Length > 0;
    public bool IsImproving { get; init; }
    public bool IsWorsening { get; init; }

    public required ICommand OpenCommand { get; init; }
    public required ICommand RenameCommand { get; init; }
    public required ICommand DeleteCommand { get; init; }

    public string RenameText => AppStrings.ChatRename;
    public string DeleteText => AppStrings.ChatDelete;
}

/// <summary>The messenger's chat list: every conversation with its last message, time and how the tension moved.</summary>
public sealed class ChatListViewModel : BaseViewModel
{
    private const int PreviewLength = 110;

    private readonly IChatService _chat;
    private readonly IChatLanguageProvider _language;
    private readonly IDialogService _dialogs;
    private readonly TimeProvider _time;
    private bool _isEmpty;

    public ChatListViewModel(
        IChatService chat,
        INavigationService navigationService,
        IChatLanguageProvider language,
        IDialogService dialogs,
        TimeProvider time)
    {
        _chat = chat;
        _language = language;
        _dialogs = dialogs;
        _time = time;

        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(GoBackAsync);
        NewChatCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(null));
    }

    /// <summary>Set by the page: shows a text prompt and returns the entered text, or null when cancelled.</summary>
    public Func<string, string, string, string, string, Task<string?>>? PromptAsync { get; set; }

    public ObservableCollection<ChatListItem> Chats { get; } = [];

    public ICommand BackCommand { get; }
    public ICommand NewChatCommand { get; }

    public bool IsEmpty
    {
        get => _isEmpty;
        private set => SetProperty(ref _isEmpty, value);
    }

    public string Title => AppStrings.ChatsTitle;
    public string NewChatText => AppStrings.ChatNew;
    public string EmptyTitle => AppStrings.ChatEmptyTitle;
    public string EmptyBody => AppStrings.ChatEmptyBody;

    public async Task RefreshAsync()
    {
        IReadOnlyList<ChatSessionDTO> sessions = await _chat.GetChatsAsync();
        DateTime now = _time.GetUtcNow().UtcDateTime;

        Chats.Clear();
        foreach (ChatSessionDTO session in sessions.Where(s => s.HasConversation()))
        {
            Chats.Add(ToItem(session, now));
        }

        IsEmpty = Chats.Count == 0;
    }

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(Title), nameof(NewChatText), nameof(EmptyTitle), nameof(EmptyBody));

    private ChatListItem ToItem(ChatSessionDTO session, DateTime now)
    {
        long id = session.Id;
        int? first = session.FirstIntensity;
        int? last = session.LastIntensity;

        return new ChatListItem
        {
            Id = id,
            Title = session.Title,
            Preview = Shorten(session.Preview),
            TimeText = ChatTimeFormatter.Short(session.UpdatedAt, now, _language.IsEnglish),
            MoodText = MoodText(first, last),
            IsImproving = first is { } f && last is { } l && l < f,
            IsWorsening = first is { } f2 && last is { } l2 && l2 > f2,
            OpenCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(id)),
            RenameCommand = new AsyncCommand(() => RenameAsync(id, session.Title)),
            DeleteCommand = new AsyncCommand(() => DeleteAsync(id))
        };
    }

    private async Task RenameAsync(long id, string current)
    {
        if (PromptAsync is null)
        {
            return;
        }

        string? name = await PromptAsync(
            AppStrings.ChatRenameTitle,
            AppStrings.ChatRenameMessage,
            AppStrings.ChatRenameAccept,
            AppStrings.ChatCancel,
            current);
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        await _chat.RenameAsync(id, name);
        await RefreshAsync();
    }

    private async Task DeleteAsync(long id)
    {
        bool confirmed = await _dialogs.AskAsync(
            AppStrings.ChatDeleteTitle,
            AppStrings.ChatDeleteBody,
            AppStrings.ChatDelete,
            AppStrings.ChatCancel);
        if (!confirmed)
        {
            return;
        }

        await _chat.DeleteAsync(id);
        await RefreshAsync();
    }

    private static string MoodText(int? first, int? last) => (first, last) switch
    {
        ({ } f, { } l) when f != l => $"{f} → {l}",
        (_, { } l) => l.ToString(System.Globalization.CultureInfo.InvariantCulture),
        _ => string.Empty
    };

    private static string Shorten(string? text)
    {
        string flat = (text ?? string.Empty).Replace('\n', ' ').Trim();
        return flat.Length <= PreviewLength ? flat : flat[..PreviewLength].TrimEnd() + "…";
    }
}
