using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

/// <summary>One pair of bars: the tension at the start and at the end of a chat.</summary>
public sealed record TensionBarItem(string Label, int Before, int After)
{
    private const double Base = 8;
    private const double PerPoint = 9;

    public double BeforeHeight => Base + (Before * PerPoint);
    public double AfterHeight => Base + (After * PerPoint);
    public string BeforeText => Before.ToString(CultureInfo.InvariantCulture);
    public string AfterText => After.ToString(CultureInfo.InvariantCulture);
    public bool Improved => After < Before;
}

public sealed record PracticeRowItem(string Title, string Detail, double HelpedFraction, bool IsBest, string StartText, ICommand StartCommand)
{
    public string BestText => AppStrings.ChatProfilePracticeBest;
}

/// <summary>A first line to start a conversation with, in the icon and colour of its state as on the chat list.</summary>
public sealed record ProfileOpener(string Text, string IconName, TechniqueFlavor Flavor, ICommand Command);

/// <summary>
/// The companion's profile: who it is, how it addresses the person, how their tension changed over time and what helped.
/// Everything is computed from stored chats by <see cref="IChatService.GetProfileAsync"/>; nothing leaves the phone.
/// </summary>
public sealed class CompanionProfileViewModel : BaseViewModel
{
    private const string DateFormat = "dd.MM";

    private readonly IChatService _chat;
    private readonly IChatLanguageProvider _language;
    private readonly IDialogService _dialogs;

    private CompanionProfile _profile = CompanionProfile.Empty;
    private bool _isLoading = true;

    public CompanionProfileViewModel(
        IChatService chat,
        INavigationService navigationService,
        IChatLanguageProvider language,
        IDialogService dialogs)
    {
        ModuleName = AppStrings.ChatsTitle;
        PageName = AppStrings.ChatProfileTitle;
        _chat = chat;
        _language = language;
        _dialogs = dialogs;

        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(GoBackAsync);
        NewChatCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(null));
        EditNameCommand = new AsyncCommand(EditNameAsync);
        ForgetCommand = new AsyncCommand(ForgetAsync);
        DeleteAllCommand = new AsyncCommand(DeleteAllAsync);
    }

    /// <summary>Set by the page: shows a text prompt and returns the entered text, or null when cancelled.</summary>
    public Func<string, string, string, string, string, Task<string?>>? PromptAsync { get; set; }

    public ICommand BackCommand { get; }
    public ICommand NewChatCommand { get; }
    public ICommand EditNameCommand { get; }
    public ICommand ForgetCommand { get; }
    public ICommand DeleteAllCommand { get; }

    public RangeObservableCollection<TensionBarItem> TensionBars { get; } = [];
    public RangeObservableCollection<PracticeRowItem> Practices { get; } = [];

    /// <summary>
    /// Something to do on the page, not only to read: each line opens a new chat and sends itself as the first message, the same
    /// lines as on the empty chat list (the chat understands each of them from a first line, ChatOpenersTests).
    /// </summary>
    public IReadOnlyList<ProfileOpener> Openers =>
    [
        Opener(AppStrings.ChatOpenerAnxious, ChatCardLook.For("Anxiety")),
        Opener(AppStrings.ChatOpenerSleep, ("Bedtime", TechniqueFlavor.Body)),
        Opener(AppStrings.ChatOpenerSad, ChatCardLook.For("Sadness")),
        Opener(AppStrings.ChatOpenerLonely, ChatCardLook.For("Loneliness"))
    ];

    public string OpenersTitle => AppStrings.ChatProfileOpenersTitle;

    public bool HasName => _profile.UserName is not null;
    public string NameText => _profile.UserName ?? AppStrings.ChatProfileNameEmpty;

    public string Tagline => ChatProfileContent.Tagline(_language.IsEnglish);
    public string AboutBody => ChatProfileContent.About(_language.IsEnglish);
    public string PrivacyLine => ChatProfileContent.PrivacyLine(_language.IsEnglish);
    public string Disclaimer => ChatProfileContent.Disclaimer(_language.IsEnglish);

    public bool HasTension => TensionBars.Count > 0;
    public string TensionCaption => ChatProfileContent.TensionCaption(_profile, _language.IsEnglish);
    public bool HasPractices => Practices.Count > 0;
    public bool HasHistory => _profile.HasHistory;

    /// <summary>True until the first load has finished. The page shows a loading state instead of a flash of zeroes.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                OnPropertyChanged(nameof(HasLoaded));
            }
        }
    }

    public bool HasLoaded => !IsLoading;
    public string LoadingText => AppStrings.ChatProfileLoadingText;

    public string Title => AppStrings.ChatProfileTitle;
    public string NameTitle => AppStrings.ChatProfileNameTitle;
    public string NameEditText => AppStrings.ChatProfileNameEdit;
    public string TensionTitle => AppStrings.ChatProfileTensionTitle;
    public string TensionStartText => AppStrings.ChatProfileTensionStart;
    public string TensionEndText => AppStrings.ChatProfileTensionEnd;
    public string PracticesTitle => AppStrings.ChatProfilePracticesTitle;
    public string AboutTitle => AppStrings.ChatProfileAboutTitle;
    public string NewChatText => AppStrings.ChatNew;
    public string ForgetText => AppStrings.ChatProfileForget;
    public string DeleteAllText => AppStrings.ChatProfileDeleteAll;

    /// <summary>Loads the profile. Safe to call on every appearing.</summary>
    public async Task RefreshAsync()
    {
        CompanionProfile fresh = await _chat.GetProfileAsync();
        _profile = fresh;

        Fill(TensionBars, fresh.Tension.Select(t => new TensionBarItem(t.AtUtc.ToLocalTime().ToString(DateFormat, CultureInfo.InvariantCulture), t.Before, t.After)));
        Fill(Practices, fresh.Practices.Select((p, i) => ToPracticeRow(p, isBest: i == 0 && p.Helped > 0)));

        NotifyAll();
        IsLoading = false;
    }

    protected override void RefreshLocalizedProperties() => NotifyAll();

    private ProfileOpener Opener(string text, (string? Icon, TechniqueFlavor Flavor) look) =>
        new(text, look.Icon ?? string.Empty, look.Flavor, new AsyncCommand(() => NavigationService!.GoToChatAsync(opener: text)));

    private PracticeRowItem ToPracticeRow(PracticeStat stat, bool isBest)
    {
        TechniqueId technique = stat.Technique;
        double fraction = stat.Tried == 0 ? 0 : Math.Clamp((double)stat.Helped / stat.Tried, 0, 1);
        return new PracticeRowItem(
            CompanionContent.TechniqueTitle(technique, _language.IsEnglish),
            AppStrings.ChatProfilePracticeDetail(stat.Helped, Math.Max(stat.Tried, stat.Helped)),
            fraction,
            isBest,
            AppStrings.ChatProfilePracticeStart,
            new AsyncCommand(() => NavigationService!.GoToTechniqueAsync(technique)));
    }

    private async Task EditNameAsync()
    {
        if (PromptAsync is null)
        {
            return;
        }

        string? name = await PromptAsync(
            AppStrings.ChatProfileNamePromptTitle,
            AppStrings.ChatProfileNamePromptMessage,
            AppStrings.ChatRenameAccept,
            AppStrings.ChatCancel,
            _profile.UserName ?? string.Empty);
        if (name is null)
        {
            return;
        }

        await _chat.SetUserNameAsync(name);
        await RefreshAsync();
    }

    private async Task ForgetAsync()
    {
        if (!await _dialogs.AskAsync(AppStrings.ChatProfileForgetTitle, AppStrings.ChatProfileForgetBody, AppStrings.ChatProfileConfirm, AppStrings.ChatCancel))
        {
            return;
        }

        await _chat.ForgetMemoryAsync();
        await RefreshAsync();
    }

    private async Task DeleteAllAsync()
    {
        if (!await _dialogs.AskAsync(AppStrings.ChatProfileDeleteAllTitle, AppStrings.ChatProfileDeleteAllBody, AppStrings.ChatDelete, AppStrings.ChatCancel))
        {
            return;
        }

        await _chat.DeleteAllChatsAsync();

        // The chat this profile was opened from no longer exists: go back to the start instead of leaving a dead conversation underneath.
        await NavigationService!.GoToRootAsync();
    }

    private void NotifyAll() => Notify(
        nameof(HasName), nameof(NameText), nameof(Tagline), nameof(AboutBody), nameof(PrivacyLine), nameof(Disclaimer),
        nameof(HasTension), nameof(TensionCaption), nameof(HasPractices), nameof(HasHistory),
        nameof(Title), nameof(NameTitle), nameof(NameEditText),
        nameof(TensionTitle), nameof(TensionStartText), nameof(TensionEndText), nameof(PracticesTitle),
        nameof(AboutTitle), nameof(NewChatText), nameof(ForgetText), nameof(DeleteAllText), nameof(LoadingText),
        nameof(Openers), nameof(OpenersTitle));

    private static void Fill<T>(RangeObservableCollection<T> target, IEnumerable<T> items) => target.ReplaceAll(items);
}
