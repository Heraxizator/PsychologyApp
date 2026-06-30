using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.Techniques;
using PsychologyApp.Presentation.Pages.MusicPlayer;
using PsychologyApp.Presentation.Pages.QuoteFeed;
using PsychologyApp.Presentation.Pages.PhysicsSearch;
using PsychologyApp.Presentation.Pages.TestsList;
using PsychologyApp.Presentation.Pages.StartPhysics;

namespace PsychologyApp.Presentation.App;

public partial class AppShell : Shell
{
    public ShellContent PracticeShellTab => PracticeTab;
    public ShellContent TestsTab => DetectorTab;
    public ShellContent QuotesShellTab => QuotesTab;

    private static readonly string[] TabRoutes =
    [
        "PracticeTab",
        "TestsTab",
        "SomaticTab",
        "CleanerTab",
        "QuotesTab"
    ];

    private readonly IPageFactory _pageFactory;
    private readonly IShellStartupCoordinator _startupCoordinator;
    private readonly IPracticeReminderCoordinator _practiceReminderCoordinator;
    private readonly ILogger<AppShell> _logger;
    private readonly bool[] _tabsMaterialized = new bool[5];
    private bool _lazyTabsReady;

    public AppShell(
        IPageFactory pageFactory,
        IShellStartupCoordinator startupCoordinator,
        IPracticeReminderCoordinator practiceReminderCoordinator,
        ILogger<AppShell> logger)
    {
        _pageFactory = pageFactory;
        _startupCoordinator = startupCoordinator;
        _practiceReminderCoordinator = practiceReminderCoordinator;
        _logger = logger;
        InitializeComponent();
        ApplyLocalization();
        EnsureLazyTabsReady();
        UserPreferences.Changed += OnPreferencesChanged;
        HandlerChanged += OnShellHandlerChanged;
        Navigating += OnShellNavigating;
        Navigated += OnShellNavigated;
        _ = InitializeAppAsync();
    }

    private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (TryResolveTabIndex(e.Target, out int index))
        {
            EnsureTabMaterialized(index);
        }
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        EnsureCurrentTabMaterialized();

        if (e.Source != ShellNavigationSource.ShellItemChanged)
        {
            return;
        }

        if (!ReduceMotion.IsEnabled)
        {
            TryPerformTabHaptic();
        }
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
        DetectorTab.Title = AppStrings.ShellTabDetectorShort;
        SomaticTab.Title = AppStrings.ShellTabSomaticShort;
        CleanerTab.Title = AppStrings.ShellTabMusicShort;
        QuotesTab.Title = AppStrings.ShellTabMotivatorShort;
        ApplyTabBarChrome();
        ApplyStatusBarChrome(IsDarkRequestedTheme());
    }

    private void RefreshShellTabPageTitles()
    {
        UpdateTabPageTitle(PracticeTab);
        UpdateTabPageTitle(DetectorTab);
        UpdateTabPageTitle(SomaticTab);
        UpdateTabPageTitle(CleanerTab);
        UpdateTabPageTitle(QuotesTab);
    }

    private static ContentPage? GetTabRootPage(ShellContent tab) =>
        tab.Content as ContentPage;

    private static void UpdateTabPageTitle(ShellContent tab)
    {
        if (GetTabRootPage(tab) is not ContentPage page)
        {
            return;
        }

        string? title = page.BindingContext switch
        {
            TechniquesViewModel techniques => techniques.PageTitle,
            TestsListViewModel tests => tests.PageTitle,
            StartPhysicsViewModel physics => physics.PageTitle,
            MusicPlayerViewModel music => music.PageTitle,
            QuoteViewModel quotes => quotes.PageTitle,
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

    private static bool TryResolveTabIndex(ShellNavigationState target, out int index)
    {
        string location = target.Location.ToString();
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

    private int GetTabIndex(ShellContent shellContent)
    {
        if (ReferenceEquals(shellContent, PracticeTab))
        {
            return 0;
        }

        if (ReferenceEquals(shellContent, DetectorTab))
        {
            return 1;
        }

        if (ReferenceEquals(shellContent, SomaticTab))
        {
            return 2;
        }

        if (ReferenceEquals(shellContent, CleanerTab))
        {
            return 3;
        }

        if (ReferenceEquals(shellContent, QuotesTab))
        {
            return 4;
        }

        return -1;
    }

    private ShellContent GetTabShellContent(int index) => index switch
    {
        0 => PracticeTab,
        1 => DetectorTab,
        2 => SomaticTab,
        3 => CleanerTab,
        4 => QuotesTab,
        _ => PracticeTab
    };

    public void MaterializeTab(ShellContent shellContent) =>
        EnsureTabMaterialized(GetTabIndex(shellContent));

    private void EnsureTabMaterialized(int index)
    {
        if (index < 0 || index >= _tabsMaterialized.Length || _tabsMaterialized[index])
        {
            return;
        }

        ShellContent shellContent = GetTabShellContent(index);
        shellContent.Content = CreateTabPage(index);
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
        1 => _pageFactory.CreateTestsListPage(),
        2 => _pageFactory.CreateStartPhysicsPage(),
        3 => _pageFactory.CreateMusicPlayerPage(),
        4 => _pageFactory.CreateQuotePage(),
        _ => _pageFactory.CreateTechniquesPage()
    };

    public void OpenPendingTechniqueIfNeeded()
    {
        if (GetTabRootPage(PracticeTab) is not ContentPage { BindingContext: TechniquesViewModel viewModel })
        {
            return;
        }

        viewModel.TryOpenPendingTechniqueAsync().FireAndForget();
    }

    private async Task InitializeAppAsync()
    {
        try
        {
            await _startupCoordinator.InitializeAsync();
            await ShowOnboardingIfNeededAsync();
            await _practiceReminderCoordinator.SyncAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Shell startup completed with errors.");
        }
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
                UserPreferences.SetPendingTechnique(id);
                OpenPendingTechniqueIfNeeded();
            }

            await Task.CompletedTask;
        });
    }
}
