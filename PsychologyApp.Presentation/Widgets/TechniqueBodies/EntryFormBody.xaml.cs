using System.ComponentModel;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Widgets.TechniqueBodies;

public partial class EntryFormBody : ContentView
{
    private const double SlideDistance = 36;

    private int _shownStep;

    public EntryFormBody()
    {
        InitializeComponent();
        BindingContextChanged += OnBindingContextChanged;
    }

    /// <summary>
    /// Takes "Back / Next" off the card so the page can pin them to the bottom of the screen, under the thumb.
    /// The controls keep their bindings; the caller gives them the same binding context.
    /// </summary>
    public View DetachNavigation()
    {
        if (WizardNav.Parent is Layout card)
        {
            card.Children.Remove(WizardNav);
        }

        return WizardNav;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is TechniqueSessionViewModel viewModel)
        {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _shownStep = viewModel.CurrentStepIndex;
            BuildDots(viewModel);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TechniqueSessionViewModel viewModel)
        {
            return;
        }

        if (e.PropertyName == nameof(TechniqueSessionViewModel.CurrentStepIndex))
        {
            int direction = Math.Sign(viewModel.CurrentStepIndex - _shownStep);
            _shownStep = viewModel.CurrentStepIndex;
            SlideInAsync(direction).FireAndForget();
            BuildDots(viewModel);
        }
        else if (e.PropertyName is nameof(TechniqueSessionViewModel.Entries) or nameof(TechniqueSessionViewModel.IsWizardMode)
                 or nameof(TechniqueSessionViewModel.NextStepText))
        {
            BuildDots(viewModel);
        }
    }

    /// <summary>The next question comes in from the right, the previous one from the left, like turning a page.</summary>
    private async Task SlideInAsync(int direction)
    {
        if (direction == 0 || !UiAnimations.ShouldAnimate(FieldHost))
        {
            await UiAnimations.SafeRevealLiteAsync(FieldHost, allowHidden: true);
            return;
        }

        FieldHost.CancelAnimations();
        FieldHost.TranslationX = direction * SlideDistance;
        FieldHost.Opacity = 0;
        await Task.WhenAll(
            FieldHost.TranslateToAsync(0, 0, UiAnimations.MediumDuration, Easing.CubicOut),
            FieldHost.FadeToAsync(1, UiAnimations.MediumDuration, Easing.CubicOut));
    }

    private void BuildDots(TechniqueSessionViewModel viewModel)
    {
        int count = viewModel.Entries.Count;
        if (StepDots.Children.Count != count)
        {
            StepDots.Children.Clear();
            for (int i = 0; i < count; i++)
            {
                StepDots.Children.Add(new BoxView { HeightRequest = 6, WidthRequest = 6, CornerRadius = 3, VerticalOptions = LayoutOptions.Center });
            }
        }

        Color accent = ThemeColor("Primary", "PrimaryTextDark");
        Color ahead = ThemeColor("NeutralBorder", "InputBorderDark");
        for (int i = 0; i < StepDots.Children.Count; i++)
        {
            BoxView dot = (BoxView)StepDots.Children[i];
            bool current = i == viewModel.CurrentStepIndex;
            bool answered = !string.IsNullOrWhiteSpace(viewModel.Entries[i].Text);
            dot.Color = current || answered ? accent : ahead;
            dot.Opacity = current ? 1 : answered ? 0.55 : 1;
            double width = current ? 22 : 6;
            if (Math.Abs(dot.WidthRequest - width) > 0.1)
            {
                if (UiAnimations.ShouldAnimate(dot))
                {
                    double from = dot.WidthRequest;
                    dot.Animate("dot", v => dot.WidthRequest = v, from, width, length: UiAnimations.MicroDuration, easing: Easing.CubicOut);
                }
                else
                {
                    dot.WidthRequest = width;
                }
            }
        }
    }

    private static Color ThemeColor(string lightKey, string darkKey)
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark ? darkKey : lightKey;
        return Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true && value is Color color
            ? color
            : Colors.Gray;
    }
}
