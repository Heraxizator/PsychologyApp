using CommunityToolkit.Maui.Animations;

namespace PsychologyApp.Presentation.Infrastructure.Animations;

public sealed class FadeInAnimation : BaseAnimation
{
    public static readonly BindableProperty FromOpacityProperty =
        BindableProperty.Create(nameof(FromOpacity), typeof(double), typeof(FadeInAnimation), 0d);

    public double FromOpacity
    {
        get => (double)GetValue(FromOpacityProperty);
        set => SetValue(FromOpacityProperty, value);
    }

    public FadeInAnimation()
    {
        Length = UiAnimations.MicroDuration;
        Easing = UiAnimations.StandardEasing;
    }

    public override Task Animate(VisualElement view, CancellationToken token = default) =>
        UiAnimations.FadeInAsync(view, FromOpacity, Length, cancellationToken: token);
}
