using System.ComponentModel;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.Widgets.Onboarding;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingPage : ContentPage
{
    private readonly OnboardingViewModel _viewModel;
    private int _currentStep = -1;
    private CancellationTokenSource? _stepMotionCts;

    public OnboardingPage(IOnboardingViewModelFactory factory, Func<TechniqueId?, Task> onCompleted)
    {
        InitializeComponent();
        _viewModel = factory.Create(Navigation, onCompleted);
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        StepScrollView.SizeChanged += OnStepScrollSizeChanged;
        HideAllSteps();
    }

    private void OnStepScrollSizeChanged(object? sender, EventArgs e)
    {
        double height = StepScrollView.Height;
        if (height <= 0)
        {
            return;
        }

        WelcomeStep.MinimumHeightRequest = height;
        OverviewStep.MinimumHeightRequest = height;
        ConcernStep.MinimumHeightRequest = height;
        FinishStep.MinimumHeightRequest = height;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_currentStep < 0)
        {
            _currentStep = _viewModel.Step;
            RevealStepAsync(GetStepView(_currentStep), _currentStep).FireAndForget();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(OnboardingViewModel.Step))
        {
            return;
        }

        AnimateStepChangeAsync(_currentStep, _viewModel.Step).FireAndForget();
        _currentStep = _viewModel.Step;
    }

    private void HideAllSteps()
    {
        WelcomeStep.IsVisible = false;
        OverviewStep.IsVisible = false;
        ConcernStep.IsVisible = false;
        FinishStep.IsVisible = false;
    }

    private async Task AnimateStepChangeAsync(int previousStep, int newStep)
    {
        CancelStepMotion();

        VisualElement? previousView = GetStepView(previousStep);
        VisualElement? nextView = GetStepView(newStep);

        if (previousView is not null && previousView.IsVisible)
        {
            if (UiAnimations.ShouldAnimate(previousView))
            {
                double baseY = previousView.TranslationY;
                await Task.WhenAll(
                    previousView.FadeToAsync(0, UiAnimations.ExitRevealDuration, UiAnimations.ExitEasing),
                    previousView.TranslateToAsync(0, baseY - 8, UiAnimations.ExitRevealDuration, UiAnimations.ExitEasing));
            }

            previousView.IsVisible = false;
            UiAnimations.ResetVisualState(previousView);
        }

        if (nextView is not null)
        {
            await RevealStepAsync(nextView, newStep);
        }
    }

    private async Task RevealStepAsync(VisualElement? stepView, int step = -1)
    {
        if (stepView is null)
        {
            return;
        }

        stepView.IsVisible = true;

        if (!UiAnimations.ShouldAnimate(stepView))
        {
            stepView.Opacity = 1;
            await RunStepMotionAsync(step < 0 ? _viewModel.Step : step);
            return;
        }

        if (stepView is Layout layout)
        {
            foreach (IView child in layout.Children)
            {
                if (child is VisualElement element)
                {
                    UiAnimations.PrepareForPremiumReveal(element);
                }
            }

            await UiAnimations.RevealChildrenStaggeredAsync(layout, allowHidden: true);
            await RunStepMotionAsync(step < 0 ? _viewModel.Step : step);
            return;
        }

        await UiAnimations.SafeRevealPremiumAsync(stepView, allowHidden: true);
        await RunStepMotionAsync(step < 0 ? _viewModel.Step : step);
    }

    private async Task RunStepMotionAsync(int step)
    {
        CancelStepMotion();
        _stepMotionCts = new CancellationTokenSource();
        CancellationToken token = _stepMotionCts.Token;

        try
        {
            switch (step)
            {
                case 0:
                    await WelcomeHero.PulseLogoAsync();
                    break;
                case 1:
                    await OverviewBanner.PlayIconSequenceAsync(token);
                    break;
                case 3:
                    await FinishHeader.PulseCheckCircleAsync();
                    await UiAnimations.SafePulseAsync(RecommendationPreview, token);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // Step changed before motion finished.
        }
    }

    private void CancelStepMotion()
    {
        if (_stepMotionCts is null)
        {
            return;
        }

        _stepMotionCts.Cancel();
        _stepMotionCts.Dispose();
        _stepMotionCts = null;
    }

    private VisualElement? GetStepView(int step) =>
        step switch
        {
            0 => WelcomeStep,
            1 => OverviewStep,
            2 => ConcernStep,
            3 => FinishStep,
            _ => null
        };
}
