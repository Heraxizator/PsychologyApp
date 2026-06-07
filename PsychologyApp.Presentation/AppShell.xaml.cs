using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Onboarding;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Technique.Main;

namespace PsychologyApp.Presentation;

public partial class AppShell : Shell
{
    private readonly IPageFactory _pageFactory;
    private bool _tabsConfigured;

    public AppShell(IPageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        InitializeComponent();
        ApplyLocalization();
        EnsureTabsConfigured();
        UserPreferences.Changed += OnPreferencesChanged;
        HandlerChanged += OnShellHandlerChanged;
        _ = InitializeAppAsync();
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
            MauiServiceProvider.GetRequired<ILogger<AppShell>>()
                .LogDebug(ex, "Status bar chrome update skipped.");
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
        IAppStartupService startup = MauiServiceProvider.GetRequired<IAppStartupService>();
        ILogger<AppShell> logger = MauiServiceProvider.GetRequired<ILogger<AppShell>>();

        try
        {
            await startup.InitializeAsync();
            AppReadiness.SignalDatabaseReady();
            await ShowOnboardingIfNeededAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application startup failed.");
            AppReadiness.SignalDatabaseFailed(ex);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Microsoft.Maui.Controls.Application.Current?.Windows.Count > 0
                    && Microsoft.Maui.Controls.Application.Current.Windows[0].Page is not null)
                {
                    await Microsoft.Maui.Controls.Application.Current.Windows[0].Page!.DisplayAlert(
                        AppStrings.StartupErrorTitle,
                        AppStrings.StartupErrorMessage,
                        AppStrings.Ok);
                }
            });
        }
    }

    private async Task ShowOnboardingIfNeededAsync()
    {
        if (UserPreferences.Load().HasCompletedOnboarding)
        {
            return;
        }

        try
        {
            await Task.Delay(300);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                Page? currentPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
                if (currentPage?.Navigation is not INavigation navigation)
                {
                    return;
                }

                IOnboardingViewModelFactory factory = MauiServiceProvider.GetRequired<IOnboardingViewModelFactory>();

                OnboardingPage onboardingPage = new(factory, async techniqueId =>
                {
                    await navigation.PopModalAsync(true);
                    if (techniqueId is TechniqueId id)
                    {
                        UserPreferences.SetPendingTechnique(id);
                        OpenPendingTechniqueIfNeeded();
                    }
                });

                await navigation.PushModalAsync(onboardingPage, true);
            });
        }
        catch (Exception ex)
        {
            MauiServiceProvider.GetRequired<ILogger<AppShell>>()
                .LogWarning(ex, "Onboarding presentation skipped.");
        }
    }
}
