using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Shell;
using PsychologyApp.Presentation.ViewModels.Practice;
using PsychologyApp.Presentation.ViewModels.Clean;
using PsychologyApp.Presentation.ViewModels.Motivator;
using PsychologyApp.Presentation.ViewModels.Physics;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation;

public partial class AppShell : Shell
{
    private readonly IPageFactory _pageFactory;
    private readonly IShellStartupCoordinator _startupCoordinator;
    private readonly ILogger<AppShell> _logger;
    private bool _tabsConfigured;

    public AppShell(
        IPageFactory pageFactory,
        IShellStartupCoordinator startupCoordinator,
        ILogger<AppShell> logger)
    {
        _pageFactory = pageFactory;
        _startupCoordinator = startupCoordinator;
        _logger = logger;
        InitializeComponent();
        ApplyLocalization();
        EnsureTabsConfigured();
        UserPreferences.Changed += OnPreferencesChanged;
        HandlerChanged += OnShellHandlerChanged;
        Navigated += OnShellNavigated;
        _ = InitializeAppAsync();
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        if (e.Source != ShellNavigationSource.ShellItemChanged)
        {
            return;
        }

        if (!ReduceMotion.IsEnabled)
        {
            TryPerformTabHaptic();
        }

        if (CurrentPage is ContentPage { Content: View root })
        {
            UiAnimations.SafePulseAsync(root).FireAndForget();
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
        ApplyStatusBarChrome(UserPreferences.IsDarkTheme(UserPreferences.Load().Theme));
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
        CleanerTab.Title = AppStrings.ShellTabCleanerShort;
        MotivatorTab.Title = AppStrings.ShellTabMotivatorShort;
        ApplyTabBarChrome();
        ApplyStatusBarChrome(UserPreferences.IsDarkTheme(UserPreferences.Load().Theme));
    }

    private void RefreshShellTabPageTitles()
    {
        UpdateTabPageTitle(PracticeTab);
        UpdateTabPageTitle(DetectorTab);
        UpdateTabPageTitle(SomaticTab);
        UpdateTabPageTitle(CleanerTab);
        UpdateTabPageTitle(MotivatorTab);
    }

    private static void UpdateTabPageTitle(ShellContent tab)
    {
        if (tab.Content is not ContentPage page)
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

            statusBar.StatusBarColor = isDark ? Colors.Black : Colors.White;
            statusBar.StatusBarStyle = isDark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Status bar chrome update skipped.");
        }
    }

    private static Color ResolveColor(ResourceDictionary? resources, string key, Color fallback) =>
        resources?.TryGetValue(key, out object? value) == true && value is Color color ? color : fallback;

    private void EnsureTabsConfigured()
    {
        if (_tabsConfigured || Items.FirstOrDefault() is not TabBar tabBar)
        {
            return;
        }

        ContentPage[] pages =
        [
            _pageFactory.CreateTechniquesPage(),
            _pageFactory.CreateTestsListPage(),
            _pageFactory.CreateStartPhysicsPage(),
            _pageFactory.CreateMusicPlayerPage(),
            _pageFactory.CreateQuotePage(),
        ];

        for (int index = 0; index < tabBar.Items.Count && index < pages.Length; index++)
        {
            if (tabBar.Items[index].CurrentItem is ShellContent shellContent)
            {
                shellContent.Content = pages[index];
            }
        }

        _tabsConfigured = true;
        OpenPendingTechniqueIfNeeded();
    }

    private void OpenPendingTechniqueIfNeeded()
    {
        if (PracticeTab.Content is not ContentPage { BindingContext: TechniquesViewModel viewModel })
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
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Shell startup completed with errors.");
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
