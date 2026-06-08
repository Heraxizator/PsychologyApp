namespace PsychologyApp.Presentation.Infrastructure;

public sealed class PressFeedbackOptions
{
    public Func<double> PressScale { get; init; } = () => UiAnimations.PressScalePrimary;

    public bool HapticOnRelease { get; init; }
}
