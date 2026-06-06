using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation;

public partial class AppShell : Shell
{
    public AppShell(IPageFactory pageFactory)
    {
        InitializeComponent();
        ConfigureTabContent(pageFactory);
        ApplyLocalization();
        UserPreferences.Changed += OnPreferencesChanged;
        _ = InitializeAppAsync();
    }

    public void ApplyChrome(bool isDark)
    {
        StatusBarBehavior? statusBar = Behaviors.OfType<StatusBarBehavior>().FirstOrDefault();
        if (statusBar is not null)
        {
            statusBar.StatusBarColor = isDark ? Colors.Black : Colors.White;
            statusBar.StatusBarStyle = isDark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
        }
    }

    private void OnPreferencesChanged()
    {
        ApplyLocalization();
    }

    public void ApplyLocalization()
    {
        PracticeTab.Title = AppStrings.ShellTabPractice;
        DetectorTab.Title = AppStrings.ShellTabDetector;
        SomaticTab.Title = AppStrings.ShellTabSomatic;
        CleanerTab.Title = AppStrings.ShellTabCleaner;
        MotivatorTab.Title = AppStrings.ShellTabMotivator;
        ApplyChrome(UserPreferences.IsDarkTheme(UserPreferences.Load().Theme));
    }

    private void ConfigureTabContent(IPageFactory pageFactory)
    {
        if (Items.FirstOrDefault() is not TabBar tabBar)
        {
            return;
        }

        ContentPage[] pages =
        [
            pageFactory.CreateTechniquesPage(),
            pageFactory.CreateTestsListPage(),
            pageFactory.CreateStartPhysicsPage(),
            pageFactory.CreateMusicPlayerPage(),
            pageFactory.CreateQuotePage(),
        ];

        for (int index = 0; index < tabBar.Items.Count && index < pages.Length; index++)
        {
            if (tabBar.Items[index].CurrentItem is ShellContent shellContent)
            {
                shellContent.Content = pages[index];
            }
        }
    }

    private static async Task InitializeAppAsync()
    {
        IAppStartupService startup = MauiServiceProvider.GetRequired<IAppStartupService>();
        ILogger<AppShell> logger = MauiServiceProvider.GetRequired<ILogger<AppShell>>();

        try
        {
            await startup.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application startup failed.");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Microsoft.Maui.Controls.Application.Current?.Windows.Count > 0
                    && Microsoft.Maui.Controls.Application.Current.Windows[0].Page is not null)
                {
                    await Microsoft.Maui.Controls.Application.Current.Windows[0].Page!.DisplayAlert(
                        AppStrings.StartupErrorTitle,
                        AppStrings.StartupErrorMessage,
                        "OK");
                }
            });
        }
    }
}
