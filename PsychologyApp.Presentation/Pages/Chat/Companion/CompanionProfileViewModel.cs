using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
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

public sealed record EmotionRowItem(string Name, string PercentText, double Share);

public sealed record InsightItem(string Text);

/// <summary>
/// The companion's profile: face, level of trust, how the person's tension changed over time, what helped, what they talk about.
/// Everything is computed from stored chats by <see cref="IChatService.GetProfileAsync"/>; nothing leaves the phone.
/// </summary>
public sealed class CompanionProfileViewModel : BaseViewModel
{
    private const string DateFormat = "dd.MM";

    private readonly IChatService _chat;
    private readonly IChatLanguageProvider _language;
    private readonly IDialogService _dialogs;

    private CompanionProfile _profile = CompanionProfile.Empty;
    private bool _loaded;

    public CompanionProfileViewModel(
        IChatService chat,
        INavigationService navigationService,
        IChatLanguageProvider language,
        IDialogService dialogs)
    {
        _chat = chat;
        _language = language;
        _dialogs = dialogs;

        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(GoBackAsync);
        NewChatCommand = new AsyncCommand(() => NavigationService!.GoToChatAsync(null));
        AllChatsCommand = new AsyncCommand(() => NavigationService!.GoToChatListAsync());
        EditNameCommand = new AsyncCommand(EditNameAsync);
        ForgetCommand = new AsyncCommand(ForgetAsync);
        DeleteAllCommand = new AsyncCommand(DeleteAllAsync);
    }

    /// <summary>Set by the page: shows a text prompt and returns the entered text, or null when cancelled.</summary>
    public Func<string, string, string, string, string, Task<string?>>? PromptAsync { get; set; }

    /// <summary>Raised after the numbers were (re)loaded, so the page can play its count-up and bar animations.</summary>
    public event EventHandler? ProfileLoaded;

    public ICommand BackCommand { get; }
    public ICommand NewChatCommand { get; }
    public ICommand AllChatsCommand { get; }
    public ICommand EditNameCommand { get; }
    public ICommand ForgetCommand { get; }
    public ICommand DeleteAllCommand { get; }

    public ObservableCollection<TensionBarItem> TensionBars { get; } = [];
    public ObservableCollection<PracticeRowItem> Practices { get; } = [];
    public ObservableCollection<EmotionRowItem> Emotions { get; } = [];
    public ObservableCollection<InsightItem> Insights { get; } = [];
    public ObservableCollection<InsightItem> Abilities { get; } = [];

    public int Chats => _profile.Chats;
    public int Messages => _profile.UserMessages;
    public int Days => _profile.Days;
    public int Streak => _profile.StreakDays;

    public double TrustProgress => _profile.Trust.Progress;
    public string TrustName => ChatProfileContent.TrustName(_profile.Trust.Index, _language.IsEnglish);
    public string TrustHint => ChatProfileContent.TrustHint(_profile.Trust, _language.IsEnglish);
    public IReadOnlyList<int> TrustSteps { get; } = Enumerable.Range(0, ChatProfileContent.TrustLevels).ToArray();
    public int TrustLevel => _profile.Trust.Index;

    public bool HasName => _profile.UserName is not null;
    public string NameText => _profile.UserName ?? AppStrings.ChatProfileNameEmpty;

    public string Since => ChatProfileContent.Since(_profile.SinceUtc, _language.IsEnglish);
    public string Tagline => ChatProfileContent.Tagline(_language.IsEnglish);
    public string PrivacyLine => ChatProfileContent.PrivacyLine(_language.IsEnglish);
    public string Disclaimer => ChatProfileContent.Disclaimer(_language.IsEnglish);

    public bool HasTension => TensionBars.Count > 0;
    public string TensionCaption => ChatProfileContent.TensionCaption(_profile, _language.IsEnglish);
    public bool HasPractices => Practices.Count > 0;
    public bool HasNoPractices => Practices.Count == 0 && _profile.HasHistory;
    public bool HasEmotions => Emotions.Count > 0;
    public bool HasHistory => _profile.HasHistory;

    public string Title => AppStrings.ChatProfileTitle;
    public string OnlineText => AppStrings.ChatProfileOnline;
    public string OfflineText => AppStrings.ChatProfileOffline;
    public string StatChatsLabel => AppStrings.ChatProfileStatChats;
    public string StatMessagesLabel => AppStrings.ChatProfileStatMessages;
    public string StatDaysLabel => AppStrings.ChatProfileStatDays;
    public string StatStreakLabel => AppStrings.ChatProfileStatStreak;
    public string TrustTitle => AppStrings.ChatProfileTrustTitle;
    public string NameTitle => AppStrings.ChatProfileNameTitle;
    public string NameHint => AppStrings.ChatProfileNameHint;
    public string NameEditText => AppStrings.ChatProfileNameEdit;
    public string InsightsTitle => AppStrings.ChatProfileInsightsTitle;
    public string TensionTitle => AppStrings.ChatProfileTensionTitle;
    public string TensionStartText => AppStrings.ChatProfileTensionStart;
    public string TensionEndText => AppStrings.ChatProfileTensionEnd;
    public string PracticesTitle => AppStrings.ChatProfilePracticesTitle;
    public string PracticesEmptyText => AppStrings.ChatProfilePracticesEmpty;
    public string EmotionsTitle => AppStrings.ChatProfileEmotionsTitle;
    public string AboutTitle => AppStrings.ChatProfileAboutTitle;
    public string ActionsTitle => AppStrings.ChatProfileActionsTitle;
    public string NewChatText => AppStrings.ChatNew;
    public string AllChatsText => AppStrings.ChatProfileAllChats;
    public string ForgetText => AppStrings.ChatProfileForget;
    public string DeleteAllText => AppStrings.ChatProfileDeleteAll;

    /// <summary>Loads the profile. Safe to call on every appearing: the page animates only when something changed.</summary>
    public async Task RefreshAsync()
    {
        CompanionProfile fresh = await _chat.GetProfileAsync();
        bool changed = !_loaded || !ReferenceEquals(fresh, _profile) && !SameNumbers(fresh, _profile);
        _profile = fresh;

        Fill(TensionBars, fresh.Tension.Select(t => new TensionBarItem(t.AtUtc.ToLocalTime().ToString(DateFormat, CultureInfo.InvariantCulture), t.Before, t.After)));
        Fill(Practices, fresh.Practices.Select((p, i) => ToPracticeRow(p, isBest: i == 0 && p.Helped > 0)));
        Fill(Emotions, fresh.Emotions.Select(e => new EmotionRowItem(
            Capitalize(CompanionContent.EmotionName(e.Emotion, _language.IsEnglish)),
            $"{Math.Round(e.Share * 100):0}%",
            e.Share)));
        Fill(Insights, ChatProfileContent.Insights(fresh, _language.IsEnglish).Select(t => new InsightItem(t)));
        Fill(Abilities, ChatProfileContent.Abilities(_language.IsEnglish).Select(t => new InsightItem(t)));

        NotifyAll();
        _loaded = true;
        if (changed)
        {
            ProfileLoaded?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override void RefreshLocalizedProperties() => NotifyAll();

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
        nameof(Chats), nameof(Messages), nameof(Days), nameof(Streak),
        nameof(TrustProgress), nameof(TrustName), nameof(TrustHint), nameof(TrustLevel),
        nameof(HasName), nameof(NameText), nameof(Since), nameof(Tagline), nameof(PrivacyLine), nameof(Disclaimer),
        nameof(HasTension), nameof(TensionCaption), nameof(HasPractices), nameof(HasNoPractices), nameof(HasEmotions), nameof(HasHistory),
        nameof(Title), nameof(OnlineText), nameof(OfflineText), nameof(StatChatsLabel), nameof(StatMessagesLabel), nameof(StatDaysLabel),
        nameof(StatStreakLabel), nameof(TrustTitle), nameof(NameTitle), nameof(NameHint), nameof(NameEditText), nameof(InsightsTitle),
        nameof(TensionTitle), nameof(TensionStartText), nameof(TensionEndText), nameof(PracticesTitle), nameof(PracticesEmptyText),
        nameof(EmotionsTitle), nameof(AboutTitle), nameof(ActionsTitle), nameof(NewChatText), nameof(AllChatsText), nameof(ForgetText),
        nameof(DeleteAllText));

    private static bool SameNumbers(CompanionProfile a, CompanionProfile b) =>
        a.Chats == b.Chats && a.UserMessages == b.UserMessages && a.Days == b.Days && a.StreakDays == b.StreakDays
        && a.Trust.Index == b.Trust.Index && a.PracticesHelped == b.PracticesHelped;

    private static void Fill<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (T item in items)
        {
            target.Add(item);
        }
    }

    private static string Capitalize(string value) => value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];
}
