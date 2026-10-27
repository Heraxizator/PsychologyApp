using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
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

    /// <summary>First letter of the title, shown in the round avatar while the chat has no recognised state.</summary>
    public string Initial => Title.Length > 0 && char.IsLetterOrDigit(Title[0]) ? char.ToUpperInvariant(Title[0]).ToString() : "•";

    /// <summary>The icon of the recognised state, as on the test cards (<see cref="ChatCardLook"/>); empty while it is not known.</summary>
    public string IconName { get; init; } = string.Empty;

    public TechniqueFlavor Flavor { get; init; }
    public bool HasIcon => IconName.Length > 0;
    public bool ShowInitial => !HasIcon;

    public bool IsImproving { get; init; }
    public bool IsWorsening { get; init; }

    public required ICommand OpenCommand { get; init; }
    public required ICommand RenameCommand { get; init; }
    public required ICommand DeleteCommand { get; init; }

    public string RenameText => AppStrings.ChatRename;
    public string DeleteText => AppStrings.ChatDelete;

    /// <summary>Everything the row shows. Equal signatures mean the row does not need rebuilding.</summary>
    public string Signature => $"{Id}|{Title}|{Preview}|{TimeText}|{MoodText}|{IconName}";
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
        ModuleName = AppStrings.ChatsTitle;
        PageName = AppStrings.ChatsTitle;
        _chat = chat;
        _language = language;
        _dialogs = dialogs;
        _time = time;

        BindNavigation(navigationService);
        NewChatCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(null));
        OpenCrisisHubCommand = new AsyncCommand(() => NavigationService!.GoToCrisisHubAsync());
    }

    /// <summary>Set by the page: shows a text prompt and returns the entered text, or null when cancelled.</summary>
    public Func<string, string, string, string, string, Task<string?>>? PromptAsync { get; set; }

    public RangeObservableCollection<ChatListItem> Chats { get; } = [];

    public ICommand NewChatCommand { get; }
    public ICommand OpenCrisisHubCommand { get; }

    /// <summary>The chat reads crisis words only in its own languages; in any other it stays closed (<see cref="AppLanguages.HasChat"/>).</summary>
    public bool IsChatAvailable => AppLanguages.HasChat(AppStrings.Language);
    public bool IsChatUnavailable => !IsChatAvailable;
    public bool ShowEmptyState => IsEmpty && IsChatAvailable;
    public string UnavailableTitle => AppStrings.ChatUnavailableTitle;
    public string UnavailableBody => AppStrings.ChatUnavailableBody;
    public string CrisisHubText => AppStrings.CrisisHubTitle;

    public bool IsEmpty
    {
        get => _isEmpty;
        private set
        {
            if (SetProperty(ref _isEmpty, value))
            {
                OnPropertyChanged(nameof(HasChats));
                OnPropertyChanged(nameof(ShowEmptyState));
            }
        }
    }

    /// <summary>The tab is named after the assistant, like the other tabs are named after what they hold.</summary>
    public string Title => AppStrings.ShellTabChatShort;
    /// <summary>The round button in the corner is for a second chat; with no chats the empty state has its own big button.</summary>
    public bool HasChats => !IsEmpty && IsChatAvailable;

    public string NewChatText => AppStrings.ChatNew;
    public string StartChatText => AppStrings.ChatHeroStart;
    public string EmptyTitle => AppStrings.ChatEmptyTitle;
    public string EmptyBody => AppStrings.ChatEmptyBody;
    public string EmptyPrivacy => AppStrings.ChatEmptyPrivacy;
    public string OpenersTitle => AppStrings.ChatOpenersTitle;

    /// <summary>
    /// Ways to begin for someone who does not know what to write: each opens a new chat and sends its words as the person's
    /// first message, so the chat answers it like anything they would type. Only feelings the chat understands from a first
    /// line (ChatOpenersTests): a wish like "I want to sort it out" is read only after something has been told.
    /// </summary>
    public IReadOnlyList<ChatOpener> Openers =>
    [
        Opener(AppStrings.ChatOpenerAnxious),
        Opener(AppStrings.ChatOpenerSleep),
        Opener(AppStrings.ChatOpenerSad),
        Opener(AppStrings.ChatOpenerLonely)
    ];

    private ChatOpener Opener(string text) =>
        new(text, new AsyncCommand(() => NavigationService!.GoToChatAsync(opener: text)));

    public async Task RefreshAsync()
    {
        IReadOnlyList<ChatSessionDTO> sessions = await _chat.GetChatsAsync();
        DateTime now = _time.GetUtcNow().UtcDateTime;

        List<ChatListItem> fresh = sessions.Where(s => s.HasConversation()).Select(s => ToItem(s, now)).ToList();

        // Coming back from a chat that changed nothing must not rebuild every row.
        if (!Chats.Select(c => c.Signature).SequenceEqual(fresh.Select(c => c.Signature)))
        {
            Chats.ReplaceAll(fresh);
        }

        IsEmpty = Chats.Count == 0;
    }

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(Title), nameof(NewChatText), nameof(StartChatText), nameof(EmptyTitle), nameof(EmptyBody), nameof(EmptyPrivacy),
            nameof(OpenersTitle), nameof(Openers), nameof(IsChatAvailable), nameof(IsChatUnavailable), nameof(ShowEmptyState), nameof(HasChats),
            nameof(UnavailableTitle), nameof(UnavailableBody), nameof(CrisisHubText));

    private ChatListItem ToItem(ChatSessionDTO session, DateTime now)
    {
        long id = session.Id;
        int? first = session.FirstIntensity;
        int? last = session.LastIntensity;
        (string? icon, TechniqueFlavor flavor) = ChatCardLook.For(session.Emotion);

        return new ChatListItem
        {
            IconName = icon ?? string.Empty,
            Flavor = flavor,
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

/// <summary>A way to begin a chat offered on the empty chat list.</summary>
public sealed record ChatOpener(string Text, ICommand Command);
