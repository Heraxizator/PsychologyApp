using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Common.Diagnostics;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;
using PsychologyApp.Presentation.App.Routes;

namespace PsychologyApp.Presentation.App;

public partial class AppShell : Shell
{
    public ShellContent PracticeShellTab => PracticeTab;
    public ShellContent TestsTab => DetectorTab;
    public ShellContent ChatShellTab => ChatTab;
    public ShellContent JournalShellTab => JournalTab;

    private static readonly string[] TabRoutes =
    [
        "PracticeTab",
        "ChatTab",
        "TestsTab",
        "JournalTab"
    ];

    private readonly IPageFactory _pageFactory;
    private readonly IUserProgressService _progress;
    private readonly IDatabaseReadySignal _databaseReady;
    private int _streakDays;
    private readonly IShellStartupCoordinator _startupCoordinator;
    private readonly IPracticeReminderCoordinator _practiceReminderCoordinator;
    private readonly IQuoteReminderCoordinator _quoteReminderCoordinator;
    private readonly IMoodReminderCoordinator _moodReminderCoordinator;
    private readonly IChatReminderCoordinator _chatReminderCoordinator;
    private readonly ILogger<AppShell> _logger;
    private readonly IUserPreferencesStore _preferences;
    private readonly Func<INavigationService> _navigationServiceFactory;
    private readonly PsychologyApp.Presentation.Features.ManageProfile.AppLockCoordinator _appLock;
    private readonly bool[] _tabsMaterialized = new bool[4];
    private bool _lazyTabsReady;

    public AppShell(
        IPageFactory pageFactory,
        IShellStartupCoordinator startupCoordinator,
        IPracticeReminderCoordinator practiceReminderCoordinator,
        IQuoteReminderCoordinator quoteReminderCoordinator,
        IMoodReminderCoordinator moodReminderCoordinator,
        IChatReminderCoordinator chatReminderCoordinator,
        ILogger<AppShell> logger,
        IUserPreferencesStore preferences,
        Func<INavigationService> navigationServiceFactory,
        PsychologyApp.Presentation.Features.ManageProfile.AppLockCoordinator appLock,
        IUserProgressService progress,
        IDatabaseReadySignal databaseReady)
    {
        _progress = progress;
        _databaseReady = databaseReady;
        _navigationServiceFactory = navigationServiceFactory;
        _appLock = appLock;
        _preferences = preferences;
        _pageFactory = pageFactory;
        _startupCoordinator = startupCoordinator;
        _practiceReminderCoordinator = practiceReminderCoordinator;
        _quoteReminderCoordinator = quoteReminderCoordinator;
        _moodReminderCoordinator = moodReminderCoordinator;
        _chatReminderCoordinator = chatReminderCoordinator;
        _logger = logger;
        InitializeComponent();
        ApplyLocalization();
        UpdateTabIcons();
        EnsureLazyTabsReady();
        _preferences.Changed += OnPreferencesChanged;
        if (Microsoft.Maui.Controls.Application.Current is { } app)
        {
            app.RequestedThemeChanged += OnRequestedThemeChanged;
        }
        HandlerChanged += OnShellHandlerChanged;
        PerfTrace.StartStallWatchdog(Dispatcher);
        Navigating += OnShellNavigating;
        Navigated += OnShellNavigated;
        // While the app lock is due, nothing of the app is drawn: the lock screen is a modal page above this, unaffected.
        _appLock.CoveringChanged += (_, _) => Opacity = _appLock.IsCovering ? 0 : 1;
        _ = InitializeAppAsync();
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyTabBarChrome();
        ApplyStatusBarChrome(e.RequestedTheme == AppTheme.Dark);
        // The menu tiles take their colours per theme.
        BuildMenu();
    }

    private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        PerfTrace.Mark($"Navigating {e.Target?.Location}");
        if (TryResolveTabIndex(e.Target, out int index))
        {
            EnsureTabMaterialized(index);
        }
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        PerfTrace.Mark($"Navigated {e.Current?.Location}");
        UpdateTabIcons();
        EnsureCurrentTabMaterialized();

        if (e.Source != ShellNavigationSource.ShellItemChanged)
        {
            return;
        }

        if (!ReduceMotion.IsEnabled)
        {
            TryPerformTabHaptic();
            TryRefreshTabContentMotion();
        }
    }

    private void TryRefreshTabContentMotion()
    {
        if (GetCurrentTabIndex() is not int index)
        {
            return;
        }

        ContentPage? tabPage = GetTabRootPage(GetTabShellContent(index));
        TabContentMotion.RefreshAsync(tabPage).FireAndForget();
    }

    private static void TryPerformTabHaptic()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Tab haptic skipped: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void OnShellHandlerChanged(object? sender, EventArgs e)
    {
        if (Handler is null)
        {
            return;
        }

        ApplyTabBarChrome();
        ApplyStatusBarChrome(IsDarkRequestedTheme());
        EnsureCurrentTabMaterialized();
    }

    private void OnPreferencesChanged()
    {
        ApplyLocalization();
        RefreshShellTabPageTitles();
    }

    public void ApplyLocalization()
    {
        PracticeTab.Title = AppStrings.ShellTabPracticeShort;
        ChatTab.Title = AppStrings.ShellTabChatShort;
        DetectorTab.Title = AppStrings.ShellTabDetectorShort;
        JournalTab.Title = AppStrings.ShellTabJournalShort;
        BuildMenu();
        ApplyTabBarChrome();
        ApplyStatusBarChrome(IsDarkRequestedTheme());
    }

    private void RefreshShellTabPageTitles()
    {
        for (int index = 0; index < _tabsMaterialized.Length; index++)
        {
            UpdateTabPageTitle(GetTabShellContent(index));
        }
    }

    private static ContentPage? GetTabRootPage(ShellContent tab) =>
        tab.Content as ContentPage;

    private void UpdateTabPageTitle(ShellContent tab)
    {
        if (GetTabRootPage(tab) is not ContentPage page)
        {
            return;
        }

        string? title = page.BindingContext switch
        {
            TechniquesViewModel techniques => techniques.PageTitle,
            TestsListViewModel tests => tests.PageTitle,
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(title))
        {
            page.Title = title;
        }
    }

    private void ApplyTabBarChrome()
    {
        ResourceDictionary? resources = Microsoft.Maui.Controls.Application.Current?.Resources;
        Color primary = ResolveColor(resources, "Primary", Color.FromArgb("#0085FF"));
        Color unselected = ResolveColor(resources, "Gray400", Color.FromArgb("#919191"));
        Color tabBarBackground = ResolveColor(resources, "White", Colors.White);

        if (Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            tabBarBackground = ResolveColor(resources, "Gray950", Color.FromArgb("#141414"));
        }

        SetValue(Shell.TabBarBackgroundColorProperty, tabBarBackground);
        SetValue(Shell.TabBarForegroundColorProperty, primary);
        SetValue(Shell.TabBarTitleColorProperty, primary);
        SetValue(Shell.TabBarUnselectedColorProperty, unselected);
    }

    private void ApplyStatusBarChrome(bool isDark)
    {
        if (Handler is null)
        {
            return;
        }

        try
        {
            StatusBarBehavior? statusBar = Behaviors.OfType<StatusBarBehavior>().FirstOrDefault();
            if (statusBar is null)
            {
                return;
            }

            ResourceDictionary? resources = Microsoft.Maui.Controls.Application.Current?.Resources;
            statusBar.StatusBarColor = isDark
                ? ResolveColor(resources, "Gray950", Color.FromArgb("#141414"))
                : ResolveColor(resources, "White", Colors.White);
            statusBar.StatusBarStyle = isDark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Status bar chrome update skipped.");
        }
    }

    private static bool IsDarkRequestedTheme() =>
        Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;

    private static Color ResolveColor(ResourceDictionary? resources, string key, Color fallback) =>
        resources?.TryGetValue(key, out object? value) == true && value is Color color ? color : fallback;

    private void EnsureLazyTabsReady()
    {
        if (_lazyTabsReady)
        {
            return;
        }

        for (int index = 0; index < _tabsMaterialized.Length; index++)
        {
            AssignPlaceholderIfEmpty(index);
        }

        EnsureTabMaterialized(0);
        _lazyTabsReady = true;
    }

    private void AssignPlaceholderIfEmpty(int index)
    {
        if (_tabsMaterialized[index])
        {
            return;
        }

        ShellContent shellContent = GetTabShellContent(index);
        if (shellContent.Content is not null)
        {
            return;
        }

        shellContent.Content = new ContentPage
        {
            BackgroundColor = Colors.Transparent,
            Title = shellContent.Title
        };
    }

    private void EnsureCurrentTabMaterialized()
    {
        if (GetCurrentTabIndex() is int index)
        {
            EnsureTabMaterialized(index);
        }
    }

    private int? GetCurrentTabIndex()
    {
        ShellContent? activeContent = GetActiveShellContent();
        if (activeContent is null)
        {
            return null;
        }

        int index = GetTabIndex(activeContent);
        return index >= 0 ? index : null;
    }

    private ShellContent? GetActiveShellContent()
    {
        BaseShellItem? current = CurrentItem is TabBar tabBar ? tabBar.CurrentItem : CurrentItem;
        return ShellNavigationResolver.ResolveShellContent(current);
    }

    private static bool TryResolveTabIndex(ShellNavigationState? target, out int index)
    {
        string location = target?.Location.ToString() ?? string.Empty;
        for (int i = 0; i < TabRoutes.Length; i++)
        {
            if (location.Contains(TabRoutes[i], StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    /// <summary>The bottom tabs and their icon names; the selected one is drawn filled, the others outlined (see <see cref="TabIcons"/>).</summary>
    private (BaseShellItem Item, string Filled, string Outlined)[] BottomTabs =>
    [
        (PracticeTab, "Spa", "Spa"),
        (ChatTab, "Forum", "Forum"),
        (DetectorTab, "Psychology", "Psychology"),
        (JournalTab, "AutoStories", "AutoStories")
    ];

    private void UpdateTabIcons()
    {
        ShellSection? current = (CurrentItem ?? Items.FirstOrDefault())?.CurrentItem;
        foreach ((BaseShellItem item, string filled, string outlined) in BottomTabs)
        {
            // A ShellContent placed straight in the tab bar sits in an implicit section.
            bool selected = ReferenceEquals(item.Parent, current);
            item.Icon = selected ? TabIcons.Get(filled, filled: true) : TabIcons.Get(outlined, filled: false);
        }
    }

    private ShellContent[] TabContents => [PracticeTab, ChatTab, DetectorTab, JournalTab];

    private int GetTabIndex(ShellContent shellContent) => Array.FindIndex(TabContents, tab => ReferenceEquals(tab, shellContent));

    private ShellContent GetTabShellContent(int index) => index >= 0 && index < TabContents.Length ? TabContents[index] : PracticeTab;

    /// <summary>Shows a bottom tab: the content is selected in its section and the section in the tab bar.</summary>
    public void ShowTab(ShellContent shellContent)
    {
        EnsureTabMaterialized(GetTabIndex(shellContent));
        if (shellContent.Parent is ShellSection section)
        {
            section.CurrentItem = shellContent;
            if (section.Parent is ShellItem item)
            {
                item.CurrentItem = section;
                CurrentItem = item;
            }
        }
    }

    private void EnsureTabMaterialized(int index)
    {
        if (index < 0 || index >= _tabsMaterialized.Length || _tabsMaterialized[index])
        {
            return;
        }

        ShellContent shellContent = GetTabShellContent(index);
        long started = System.Diagnostics.Stopwatch.GetTimestamp();
        shellContent.Content = CreateTabPage(index);
        PerfTrace.Measure($"Tab {index} created", started);
        _tabsMaterialized[index] = true;
        UpdateTabPageTitle(shellContent);

        if (index == 0)
        {
            OpenPendingTechniqueIfNeeded();
        }
    }

    private ContentPage CreateTabPage(int index) => index switch
    {
        0 => _pageFactory.CreateTechniquesPage(),
        1 => _pageFactory.CreateChatListPage(null),
        2 => _pageFactory.CreateTestsListPage(),
        3 => _pageFactory.CreateJournalPage(),
        _ => _pageFactory.CreateTechniquesPage()
    };

    /// <summary>
    /// Fills the burger menu. Its pages open over the current tab, so the tabs stay where they were and Back returns to them; whatever
    /// was open over the tab is closed first, so the menu never stacks one section on another.
    /// </summary>
    private void BuildMenu()
    {
        bool dark = IsDarkRequestedTheme();
        AppMenuEntry profile = MenuEntry("Person", AppStrings.ProfileTitle, AppMenuTone.Mine, navigation => navigation.GoToUserProfileAsync(), dark);
        AppMenuEntry[] mine =
        [
            MenuEntry("Insights", AppStrings.InsightsTitle, AppMenuTone.Mine, navigation => navigation.GoToInsightsAsync(), dark),
            MenuEntry("AccessibilityNew", AppStrings.BodyTitle, AppMenuTone.Mine, navigation => navigation.GoToBodyAsync(), dark)
        ];
        AppMenuEntry[] read =
        [
            MenuEntry("AutoAwesome", AppStrings.ArticlesForYou, AppMenuTone.Read, navigation => navigation.GoToForYouAsync(), dark),
            MenuEntry("School", AppStrings.ArticlesCourses, AppMenuTone.Read, navigation => navigation.GoToCoursesAsync(), dark),
            MenuEntry("Article", AppStrings.ShellTabArticlesShort, AppMenuTone.Read, navigation => navigation.GoToArticlesAsync(), dark)
        ];
        AppMenuEntry[] support =
        [
            MenuEntry("Anchor", AppStrings.AnchorThoughtsTitle, AppMenuTone.Support, navigation => navigation.GoToAnchorThoughtsAsync(), dark),
            MenuEntry("FormatQuote", AppStrings.ShellTabMotivatorShort, AppMenuTone.Support, navigation => navigation.GoToQuotesAsync(), dark),
            MenuEntry("Headphones", AppStrings.ShellTabMusicShort, AppMenuTone.Support, navigation => navigation.GoToPrayersAsync(), dark)
        ];
        AppMenuEntry[] app =
        [
            MenuEntry("HealthAndSafety", AppStrings.CrisisHubTitle, AppMenuTone.Help, navigation => navigation.GoToCrisisHubAsync(), dark),
            MenuEntry("Settings", AppStrings.OptionsTitle, AppMenuTone.App, navigation => navigation.GoToOptionsAsync(), dark)
        ];

        MenuProfileCard.BindingContext = profile;
        ShowProfileSubtitle();
        MenuFooterPrivacy.Text = AppStrings.MenuFooterPrivacy;
        MenuFooterVersion.Text = AppStrings.InfoAppVersion(AppInfo.Current.VersionString);
        MenuMineTitle.Text = AppStrings.MenuGroupMine;
        MenuReadTitle.Text = AppStrings.MenuGroupRead;
        MenuSupportTitle.Text = AppStrings.MenuGroupSupport;
        BindableLayout.SetItemsSource(MenuMine, mine);
        BindableLayout.SetItemsSource(MenuRead, read);
        BindableLayout.SetItemsSource(MenuSupport, support);
        BindableLayout.SetItemsSource(MenuApp, app);
    }

    /// <summary>The days in a row with a practice when there are some, the general line otherwise; refreshed each time the menu opens.</summary>
    private void ShowProfileSubtitle() =>
        MenuProfileSubtitle.Text = _streakDays > 0 ? AppStrings.MenuProfileStreak(_streakDays) : AppStrings.MenuProfileSubtitle;

    private async Task RefreshProfileStreakAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            int days = await _progress.GetStreakDaysAsync();
            await UiThread.RunAsync(() =>
            {
                _streakDays = days;
                ShowProfileSubtitle();
            });
        }
        catch (Exception error)
        {
            // Without the count the card keeps its general line; the menu works the same.
            BestEffort.Report(error);
        }
    }

    protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(FlyoutIsPresented) && FlyoutIsPresented)
        {
            RefreshProfileStreakAsync().FireAndForget();
        }
    }

    /// <summary>Back with the menu open closes the menu, as on any Android app; Shell left it to the system, which closed the app.</summary>
    protected override bool OnBackButtonPressed()
    {
        if (FlyoutIsPresented)
        {
            FlyoutIsPresented = false;
            return true;
        }

        return base.OnBackButtonPressed();
    }

    private AppMenuEntry MenuEntry(
        string icon, string title, AppMenuTone tone, Func<INavigationService, Task> open, bool dark) =>
        new(icon, title, new AsyncCommand(async () =>
        {
            FlyoutIsPresented = false;
            INavigationService navigation = _navigationServiceFactory();
            await navigation.GoToRootAsync();
            await open(navigation);
        }), tone, dark);

    public void OpenPendingTechniqueIfNeeded()
    {
        if (GetTabRootPage(PracticeTab) is not ContentPage { BindingContext: TechniquesViewModel viewModel })
        {
            return;
        }

        viewModel.TryOpenPendingTechniqueAsync().FireAndForget();
    }

    public void OpenPendingJournalIfNeeded()
    {
        if (GetTabRootPage(PracticeTab) is not ContentPage { BindingContext: TechniquesViewModel viewModel })
        {
            return;
        }

        viewModel.TryOpenPendingJournalAsync().FireAndForget();
    }

    private readonly TaskCompletionSource _startupCompleted = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Done once onboarding and the safety gate (risk check or crisis hub) have been shown, if they were due.</summary>
    internal Task StartupCompleted => _startupCompleted.Task;

    /// <summary>The safety gate is on screen: a tap on a reminder or the widget must not move the person away from it.</summary>
    internal static bool IsSafetyPageShown =>
        Current?.CurrentPage is Pages.ClinicalCare.CrisisHub.CrisisHubPage or Pages.ClinicalCare.RiskCheck.RiskCheckPage;

    private async Task InitializeAppAsync()
    {
        try
        {
            // The lock comes first: nothing of the person, not even the onboarding or the safety gate, before the phone's unlock.
            await _appLock.GuardAsync();
            await _startupCoordinator.InitializeAsync();
            await ShowOnboardingIfNeededAsync();
            await ShowClinicalGateIfNeededAsync();
            SyncRemindersFireAndForget();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Shell startup completed with errors.");
        }
        finally
        {
            _startupCompleted.TrySetResult();
        }
    }

    private void SyncRemindersFireAndForget()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.WhenAll(
                    _practiceReminderCoordinator.SyncAsync(),
                    _quoteReminderCoordinator.SyncAsync(),
                    _moodReminderCoordinator.SyncAsync(),
                    _chatReminderCoordinator.SyncAsync()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reminder sync failed during startup.");
            }
        });
    }

    private async Task ShowOnboardingIfNeededAsync()
    {
        Page? currentPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
        if (currentPage?.Navigation is not INavigation navigation)
        {
            return;
        }

        await _startupCoordinator.ShowOnboardingIfNeededAsync(navigation, async techniqueId =>
        {
            if (techniqueId is TechniqueId id)
            {
                _preferences.SetPendingTechnique(id);
                OpenPendingTechniqueIfNeeded();
            }

            await Task.CompletedTask;
        });
    }

    private async Task ShowClinicalGateIfNeededAsync()
    {
        if (!_preferences.Load().HasCompletedOnboarding)
        {
            return;
        }

        Page? currentPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
        if (currentPage?.Navigation is not INavigation navigation)
        {
            return;
        }

        await _startupCoordinator.ShowClinicalGateIfNeededAsync(navigation);
    }
}
