namespace PsychologyApp.Presentation.Common;

public sealed class PressFeedbackOptions
{
    public Func<double> PressScale { get; init; } = () => UiAnimations.PressScalePrimary;

    public bool HapticOnRelease { get; init; }

    public bool ScaleOnly { get; init; }
}
