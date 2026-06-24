using System.ComponentModel;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.Onboarding;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingPage : ContentPage
{
    private readonly OnboardingViewModel _viewModel;
    private int _currentStep = -1;

    public OnboardingPage(IOnboardingViewModelFactory factory, Func<TechniqueId?, Task> onCompleted)
    {
        InitializeComponent();
        _viewModel = factory.Create(Navigation, onCompleted);
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        HideAllSteps();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_currentStep < 0)
        {
            _currentStep = _viewModel.Step;
            RevealStepAsync(GetStepView(_currentStep)).FireAndForget();
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
            await RevealStepAsync(nextView);
        }
    }

    private static async Task RevealStepAsync(VisualElement? stepView)
    {
        if (stepView is null)
        {
            return;
        }

        stepView.IsVisible = true;

        if (!UiAnimations.ShouldAnimate(stepView))
        {
            stepView.Opacity = 1;
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
            return;
        }

        await UiAnimations.SafeRevealPremiumAsync(stepView, allowHidden: true);
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
