using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.App.Providers;

using PsychologyApp.Presentation.Shared.Services.Toasts;

namespace PsychologyApp.Presentation.App;

public sealed class ShellStartupCoordinator(
    IAppStartupService startupService,
    IOnboardingViewModelFactory onboardingViewModelFactory,
    IOptions<AppSettings> settings,
    IDatabaseReadySignal databaseReadySignal,
    IToastService toastService,
    ILogger<ShellStartupCoordinator> logger) : IShellStartupCoordinator
{
    public async Task InitializeAsync()
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateLargeTimeoutSource(settings);
            await startupService.InitializeAsync(timeoutSource.Token);
            databaseReadySignal.SignalReady();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application startup failed.");
            databaseReadySignal.SignalFailed(ex);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                toastService.ShortToast(AppStrings.StartupErrorMessage);
                return Task.CompletedTask;
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
                    await Task.Yield();
                    await navigation.PopModalAsync(false);
                    await onTechniqueSelected(techniqueId);
                });

                PressFeedbackHost.AttachToPage(onboardingPage);
                await navigation.PushModalAsync(onboardingPage, false);
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Onboarding presentation skipped.");
        }
    }
}
