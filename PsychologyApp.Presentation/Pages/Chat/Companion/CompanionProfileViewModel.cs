using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

/// <summary>
/// The companion's card: who it is in one screen, a way to urgent help and to a new chat. How it addresses the person, forgetting
/// and deleting are rare, so they sit behind "Chat settings". The profile is read from stored chats; nothing leaves the phone.
/// </summary>
public sealed class CompanionProfileViewModel : BaseViewModel
{
    private readonly IChatService _chat;
    private readonly IDialogService _dialogs;

    private CompanionProfile _profile = CompanionProfile.Empty;
    private bool _isLoading = true;

    public CompanionProfileViewModel(
        IChatService chat,
        INavigationService navigationService,
        IDialogService dialogs)
    {
        ModuleName = AppStrings.ChatsTitle;
        PageName = AppStrings.ChatProfileTitle;
        _chat = chat;
        _dialogs = dialogs;

        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(GoBackAsync);
        NewChatCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(null));
        CrisisCommand = new AsyncCommand(() => NavigationService!.GoToCrisisHubAsync());
        SettingsCommand = new AsyncCommand(OpenSettingsAsync);
    }

    /// <summary>Set by the page: shows a text prompt and returns the entered text, or null when cancelled.</summary>
    public Func<string, string, string, string, string, Task<string?>>? PromptAsync { get; set; }

    public ICommand BackCommand { get; }
    public ICommand NewChatCommand { get; }
    public ICommand CrisisCommand { get; }
    public ICommand SettingsCommand { get; }

    public bool HasHistory => _profile.HasHistory;

    /// <summary>True until the first load has finished. The page shows a loading state instead of a flash of empty text.</summary>
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

    public string Title => AppStrings.ChatProfileTitle;
    public string Tagline => AppStrings.ChatProfileTaglineShort;
    public string Disclaimer => AppStrings.ChatProfileDisclaimerShort;
    public string OfflineText => AppStrings.ChatProfileOfflineShort;
    public string OnPhoneText => AppStrings.ChatProfileOnPhone;
    public string CrisisText => AppStrings.CrisisHubTitle;
    public string NewChatText => AppStrings.ChatNew;
    public string SettingsText => AppStrings.ChatProfileSettings;

    /// <summary>Loads the profile. Safe to call on every appearing.</summary>
    public async Task RefreshAsync()
    {
        _profile = await _chat.GetProfileAsync();
        NotifyAll();
        IsLoading = false;
    }

    protected override void RefreshLocalizedProperties() => NotifyAll();

    /// <summary>The rare actions in one sheet; deleting chats is offered only when there are some.</summary>
    private async Task OpenSettingsAsync()
    {
        List<string> options = [AppStrings.ChatProfileNameTitle, AppStrings.ChatProfileForget];
        if (HasHistory)
        {
            options.Add(AppStrings.ChatProfileDeleteAll);
        }

        string? picked = await _dialogs.PickOptionAsync(AppStrings.ChatProfileSettings, options, AppStrings.ChatCancel);
        if (picked == AppStrings.ChatProfileNameTitle)
        {
            await EditNameAsync();
        }
        else if (picked == AppStrings.ChatProfileForget)
        {
            await ForgetAsync();
        }
        else if (picked == AppStrings.ChatProfileDeleteAll)
        {
            await DeleteAllAsync();
        }
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
        nameof(HasHistory), nameof(Title), nameof(Tagline), nameof(Disclaimer), nameof(OfflineText), nameof(OnPhoneText),
        nameof(CrisisText), nameof(NewChatText), nameof(SettingsText));
}
