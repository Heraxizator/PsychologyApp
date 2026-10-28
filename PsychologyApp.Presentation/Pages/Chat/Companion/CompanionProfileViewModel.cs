using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

/// <summary>
/// The companion's page, laid out like the app's settings: who it is and what it is not, then its actions as rows (a new chat, how
/// it addresses the person, urgent help, forgetting, deleting). The profile is read from stored chats; nothing leaves the phone.
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
        EditNameCommand = new AsyncCommand(EditNameAsync);
        ForgetCommand = new AsyncCommand(ForgetAsync);
        DeleteAllCommand = new AsyncCommand(DeleteAllAsync);
    }

    public ICommand BackCommand { get; }
    public ICommand NewChatCommand { get; }
    public ICommand CrisisCommand { get; }
    public ICommand EditNameCommand { get; }
    public ICommand ForgetCommand { get; }
    public ICommand DeleteAllCommand { get; }

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
    public string Disclaimer => AppStrings.ChatProfileDisclaimerShort;
    public string PrivacyText => AppStrings.ChatProfilePrivacyLine;
    public string CrisisText => AppStrings.CrisisHubTitle;
    public string NewChatText => AppStrings.ChatNew;
    public string NameTitle => AppStrings.ChatProfileNameTitle;
    public string ForgetText => AppStrings.ChatProfileForget;
    public string DeleteAllText => AppStrings.ChatProfileDeleteAll;

    /// <summary>The name it uses, under its row, so the row says what is set without opening it.</summary>
    public string UserName => _profile.UserName ?? string.Empty;
    public bool HasUserName => UserName.Length > 0;

    /// <summary>Loads the profile. Safe to call on every appearing.</summary>
    public async Task RefreshAsync()
    {
        _profile = await _chat.GetProfileAsync();
        NotifyAll();
        IsLoading = false;
    }

    protected override void RefreshLocalizedProperties() => NotifyAll();

    private async Task EditNameAsync()
    {
        string? name = await _dialogs.PromptTextAsync(
            AppStrings.ChatProfileNamePromptTitle,
            AppStrings.ChatProfileNamePromptMessage,
            AppStrings.ChatRenameAccept,
            AppStrings.ChatCancel,
            _profile.UserName ?? string.Empty,
            maxLength: 30);
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
        nameof(HasHistory), nameof(Title), nameof(Disclaimer), nameof(PrivacyText), nameof(CrisisText), nameof(NewChatText),
        nameof(NameTitle), nameof(ForgetText), nameof(DeleteAllText), nameof(UserName), nameof(HasUserName));
}
