using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.CompanionAvatar;

/// <summary>
/// The companion's face: a calm gradient circle with two eyes, a small smile and cheeks. Drawn, not an image, so it is sharp at every size
/// and costs nothing to ship. While <see cref="IsAlive"/> it blinks now and then; with reduced motion it stays still.
/// </summary>
public sealed class CompanionAvatarView : GraphicsView
{
    public static readonly BindableProperty IsAliveProperty = BindableProperty.Create(
        nameof(IsAlive),
        typeof(bool),
        typeof(CompanionAvatarView),
        false,
        propertyChanged: (bindable, _, _) => ((CompanionAvatarView)bindable).RestartBlinking());

    public static readonly BindableProperty ShowStatusProperty = BindableProperty.Create(
        nameof(ShowStatus),
        typeof(bool),
        typeof(CompanionAvatarView),
        true,
        propertyChanged: (bindable, _, _) => ((CompanionAvatarView)bindable).Invalidate());

    private readonly AvatarDrawable _drawable = new();
    private CancellationTokenSource? _blinking;

    public CompanionAvatarView()
    {
        Drawable = _drawable;
        Unloaded += (_, _) => StopBlinking();
        Loaded += (_, _) => RestartBlinking();
    }

    /// <summary>Blinks from time to time. Turn on for a large avatar the person looks at; leave off in lists.</summary>
    public bool IsAlive
    {
        get => (bool)GetValue(IsAliveProperty);
        set => SetValue(IsAliveProperty, value);
    }

    /// <summary>The small green "online" dot in the corner.</summary>
    public bool ShowStatus
    {
        get => (bool)GetValue(ShowStatusProperty);
        set => SetValue(ShowStatusProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(ShowStatus))
        {
            _drawable.ShowStatus = ShowStatus;
        }
    }

    private void RestartBlinking()
    {
        StopBlinking();
        if (!IsAlive || ReduceMotion.IsEnabled || Handler is null)
        {
            return;
        }

        _blinking = new CancellationTokenSource();
        BlinkLoopAsync(_blinking.Token).FireAndForget();
    }

    private void StopBlinking()
    {
        _blinking?.Cancel();
        _blinking = null;
        if (_drawable.Blink != 0)
        {
            _drawable.Blink = 0;
            Invalidate();
        }
    }

    private async Task BlinkLoopAsync(CancellationToken cancellationToken)
    {
        const int Steps = 5;
        const int FrameMs = 18;
        Random random = new();
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(random.Next(2200, 5200), cancellationToken);
                for (int i = 1; i <= Steps; i++)
                {
                    SetBlink((float)i / Steps);
                    await Task.Delay(FrameMs, cancellationToken);
                }

                for (int i = Steps - 1; i >= 0; i--)
                {
                    SetBlink((float)i / Steps);
                    await Task.Delay(FrameMs, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The page went away; StopBlinking already reset the eyes.
        }
    }

    private void SetBlink(float value)
    {
        _drawable.Blink = value;
        Invalidate();
    }

    private sealed class AvatarDrawable : IDrawable
    {
        private static readonly Color Top = Color.FromArgb("#62B4FF");
        private static readonly Color Bottom = Color.FromArgb("#7A5CFF");

        public float Blink { get; set; }

        public bool ShowStatus { get; set; } = true;

        public void Draw(ICanvas canvas, RectF bounds)
        {
            float size = Math.Min(bounds.Width, bounds.Height);
            if (size <= 0)
            {
                return;
            }

            float cx = bounds.Center.X;
            float cy = bounds.Center.Y;
            float radius = size * (ShowStatus ? 0.46f : 0.5f);

            // A soft halo so the face sits on the page instead of being cut out of it.
            canvas.FillColor = Bottom.WithAlpha(0.14f);
            canvas.FillCircle(cx, cy, radius + (size * 0.03f));

            LinearGradientPaint paint = new() { StartColor = Top, EndColor = Bottom, StartPoint = new Point(0.15, 0), EndPoint = new Point(0.85, 1) };
            canvas.SetFillPaint(paint, new RectF(cx - radius, cy - radius, radius * 2, radius * 2));
            canvas.FillCircle(cx, cy, radius);

            // Light from above.
            canvas.FillColor = Colors.White.WithAlpha(0.16f);
            canvas.FillEllipse(cx - (radius * 0.62f), cy - (radius * 0.86f), radius * 1.0f, radius * 0.5f);

            float eyeY = cy - (radius * 0.10f);
            float eyeDx = radius * 0.36f;
            float eyeW = radius * 0.17f;
            float eyeH = radius * 0.27f * (1f - (Blink * 0.92f));
            canvas.FillColor = Colors.White;
            canvas.FillEllipse(cx - eyeDx - (eyeW / 2), eyeY - (eyeH / 2), eyeW, eyeH);
            canvas.FillEllipse(cx + eyeDx - (eyeW / 2), eyeY - (eyeH / 2), eyeW, eyeH);

            canvas.FillColor = Colors.White.WithAlpha(0.17f);
            float cheek = radius * 0.13f;
            canvas.FillCircle(cx - (radius * 0.56f), cy + (radius * 0.26f), cheek);
            canvas.FillCircle(cx + (radius * 0.56f), cy + (radius * 0.26f), cheek);

            PathF smile = new();
            float mouthY = cy + (radius * 0.24f);
            smile.MoveTo(cx - (radius * 0.28f), mouthY);
            smile.QuadTo(cx, mouthY + (radius * 0.30f), cx + (radius * 0.28f), mouthY);
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = Math.Max(1.5f, radius * 0.075f);
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawPath(smile);

            if (ShowStatus)
            {
                float dot = size * 0.085f;
                float dx = cx + (radius * 0.74f);
                float dy = cy + (radius * 0.74f);
                canvas.FillColor = Colors.White;
                canvas.FillCircle(dx, dy, dot + (size * 0.025f));
                canvas.FillColor = Color.FromArgb("#34C759");
                canvas.FillCircle(dx, dy, dot);
            }
        }
    }
}
