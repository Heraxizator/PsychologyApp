using CommunityToolkit.Maui.Animations;

namespace PsychologyApp.Presentation.Infrastructure.Animations;

public sealed class PressScaleAnimation : BaseAnimation
{
    public static readonly BindableProperty ScaleProperty =
        BindableProperty.Create(nameof(Scale), typeof(double), typeof(PressScaleAnimation), UiAnimations.PressScale);

    public double Scale
    {
        get => (double)GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public PressScaleAnimation()
    {
        Length = UiAnimations.MicroDuration;
        Easing = UiAnimations.StandardEasing;
    }

    public override Task Animate(VisualElement view, CancellationToken token = default) =>
        UiAnimations.PressScaleAsync(view, Scale, Length);
}
