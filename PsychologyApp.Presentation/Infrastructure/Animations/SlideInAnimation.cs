using CommunityToolkit.Maui.Animations;

namespace PsychologyApp.Presentation.Infrastructure.Animations;

public sealed class SlideInAnimation : BaseAnimation
{
    public static readonly BindableProperty OffsetYProperty =
        BindableProperty.Create(nameof(OffsetY), typeof(double), typeof(SlideInAnimation), UiAnimations.SlideOffset);

    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public SlideInAnimation()
    {
        Length = UiAnimations.MicroDuration;
        Easing = UiAnimations.StandardEasing;
    }

    public override Task Animate(VisualElement view, CancellationToken token = default) =>
        UiAnimations.SlideInAsync(view, OffsetY, Length, cancellationToken: token);
}
