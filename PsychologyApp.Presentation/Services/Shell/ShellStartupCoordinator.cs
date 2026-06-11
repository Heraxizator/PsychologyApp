using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Views.Onboarding;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Services.Shell;

public sealed class ShellStartupCoordinator(
    IAppStartupService startupService,
    IOnboardingViewModelFactory onboardingViewModelFactory,
    ILogger<ShellStartupCoordinator> logger) : IShellStartupCoordinator
{
    public async Task InitializeAsync()
    {
        try
        {
            await startupService.InitializeAsync();
            AppReadiness.SignalDatabaseReady();
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

            throw;
        }
    }

    public Task ShowOnboardingIfNeededAsync(
        INavigation navigation,
        Func<TechniqueId?, Task> onTechniqueSelected)
    {
        if (UserPreferences.Load().HasCompletedOnboarding)
        {
            return Task.CompletedTask;
        }

        return ShowOnboardingAsync(navigation, onTechniqueSelected);
    }

    public async Task ShowOnboardingAsync(
        INavigation navigation,
        Func<TechniqueId?, Task> onTechniqueSelected)
    {
        try
        {
            await Task.Delay(300);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                OnboardingPage onboardingPage = new(onboardingViewModelFactory, async techniqueId =>
                {
                    await navigation.PopModalAsync(true);
                    await onTechniqueSelected(techniqueId);
                });

                PressFeedbackHost.AttachToPage(onboardingPage);
                await navigation.PushModalAsync(onboardingPage, true);
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Onboarding presentation skipped.");
        }
    }
}
