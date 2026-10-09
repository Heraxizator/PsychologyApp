using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.BreathingImmersive;

/// <summary>
/// Breathing on the whole screen: only the circle, with the page behind it glowing and fading in step with the breath. The screen stays on
/// while the exercise runs. Opened over the practice page; closing returns to it.
/// </summary>
public partial class BreathingImmersivePage : ContentPage
{
    private const double GlowMin = 0.08;
    private const double GlowMax = 0.38;

    public BreathingImmersivePage()
    {
        InitializeComponent();
        SemanticProperties.SetDescription(CloseIcon, AppStrings.DialogueClose);
        Pacer.Rendered += OnPacerRendered;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeviceDisplay.Current.KeepScreenOn = true;
    }

    protected override void OnDisappearing()
    {
        DeviceDisplay.Current.KeepScreenOn = false;
        base.OnDisappearing();
    }

    private void OnPacerRendered(double scale, Color color)
    {
        double size = Math.Clamp(
            (scale - BreathingPattern.SmallScale) / (BreathingPattern.LargeScale - BreathingPattern.SmallScale), 0, 1);
        Glow.Color = color;
        Glow.Opacity = ReduceMotion.IsEnabled ? GlowMin : GlowMin + (GlowMax - GlowMin) * size;
    }

    private async void OnCloseTapped(object? sender, TappedEventArgs e) => await Navigation.PopModalAsync();
}
