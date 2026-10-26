using PsychologyApp.Presentation.Core.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.BalloonPop;

/// <summary>
/// The balloon practice: balloons rise across the field and pop at a tap. A round starts only on request and stops by itself
/// (<see cref="BalloonRound"/>), so nothing animates while the person rates their tension. With reduced motion the balloons stand still.
/// </summary>
public partial class BalloonPopView : ContentView
{
    private static readonly Color[] Colors =
    [
        Color.FromArgb("#EF5350"), Color.FromArgb("#FFB74D"), Color.FromArgb("#FFEE58"),
        Color.FromArgb("#66BB6A"), Color.FromArgb("#42A5F5"), Color.FromArgb("#AB47BC")
    ];

    private readonly BalloonDrawable _drawable = new();
    private IDispatcherTimer? _timer;
    private DateTime _lastTick;
    private bool _still;

    public BalloonPopView()
    {
        InitializeComponent();
        Field.Drawable = _drawable;
        SemanticProperties.SetDescription(Field, AppStrings.BalloonsFieldDescription);
        Field.StartInteraction += OnFieldTouched;
        StartButton.TapCommand = new Command(StartRound);
        PopButton.BodyText = AppStrings.BalloonsPopOne;
        PopButton.TapCommand = new Command(PopNext);
        Unloaded += (_, _) => StopTimer();
        ShowIdle();
    }

    private void ShowIdle()
    {
        StartButton.BodyText = AppStrings.BalloonsStart;
        StartButton.IsVisible = true;
        PopButton.IsVisible = false;
        CountLabel.Text = AppStrings.BalloonsIntro;
    }

    private void StartRound()
    {
        _still = ReduceMotion.IsEnabled;
        _drawable.Round = new BalloonRound(reduceMotion: _still);
        StartButton.IsVisible = false;
        PopButton.IsVisible = true;
        UpdateCount();
        Field.Invalidate();
        if (!_still)
        {
            StartTimer();
        }
    }

    // With reduced motion nothing moves: the timer runs only for the short burst after a pop, not while waiting for a tap.
    private void StartTimer()
    {
        _lastTick = DateTime.UtcNow;
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(16);
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_drawable.Round is not { } round)
        {
            StopTimer();
            return;
        }

        DateTime now = DateTime.UtcNow;
        // A pause (screen off, app in the background) must not fast-forward the round on return.
        double seconds = Math.Min((now - _lastTick).TotalSeconds, 0.1);
        _lastTick = now;
        round.Update(seconds);
        Field.Invalidate();
        UpdateCount();
        if (round.IsOver)
        {
            FinishRound(round);
        }
        else if (_still && !round.Balloons.Any(balloon => balloon.State == BalloonState.Popped && balloon.BurstAge < BalloonRound.BurstSeconds))
        {
            StopTimer();
        }
    }

    private void OnFieldTouched(object? sender, TouchEventArgs e)
    {
        if (_drawable.Round is not { } round || e.Touches.Length == 0)
        {
            return;
        }

        PointF point = e.Touches[0];
        if (round.TryPop(point.X, point.Y, Field.Width, Field.Height))
        {
            Popped();
        }
    }

    private void PopNext()
    {
        if (_drawable.Round?.PopNext() == true)
        {
            Popped();
        }
    }

    private void Popped()
    {
        UiHaptics.Tick();
        UpdateCount();
        Field.Invalidate();
        if (_still && _timer?.IsRunning != true)
        {
            StartTimer();
        }
    }

    private void UpdateCount()
    {
        if (_drawable.Round is { } round)
        {
            CountLabel.Text = AppStrings.BalloonsCount(round.Popped, round.Total);
        }
    }

    private void FinishRound(BalloonRound round)
    {
        StopTimer();
        CountLabel.Text = AppStrings.BalloonsDone(round.Popped, round.Total);
        SemanticScreenReader.Announce(CountLabel.Text);
        StartButton.BodyText = AppStrings.BalloonsAgain;
        StartButton.IsVisible = true;
        PopButton.IsVisible = false;
    }

    private void StopTimer()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
        }
    }

    private sealed class BalloonDrawable : IDrawable
    {
        public BalloonRound? Round { get; set; }

        // Before the first round: a few balloons standing still, so the field does not look empty.
        private static readonly (float X, float Y, int Color)[] Preview = [(0.25f, 0.55f, 0), (0.45f, 0.4f, 4), (0.65f, 0.6f, 2), (0.8f, 0.42f, 5)];

        public void Draw(ICanvas canvas, RectF rect)
        {
            // Balloons enter from below and leave above: without the clip they were drawn over the labels and buttons.
            canvas.ClipRectangle(rect);
            float radius = (float)(BalloonRound.RadiusShare * rect.Width);
            if (Round is null)
            {
                foreach ((float x, float y, int color) in Preview)
                {
                    DrawBalloon(canvas, x * rect.Width, y * rect.Height, radius, Colors[color]);
                }

                return;
            }

            foreach (Balloon balloon in Round.Balloons)
            {
                float x = (float)(balloon.X * rect.Width);
                float y = (float)(balloon.Y * rect.Height);
                Color color = Colors[balloon.ColorIndex % Colors.Length];
                if (balloon.State == BalloonState.Floating)
                {
                    DrawBalloon(canvas, x, y, radius, color);
                }
                else if (balloon.State == BalloonState.Popped && balloon.BurstAge < BalloonRound.BurstSeconds)
                {
                    DrawBurst(canvas, x, y, radius, color, (float)(balloon.BurstAge / BalloonRound.BurstSeconds));
                }
            }
        }

        private static void DrawBalloon(ICanvas canvas, float x, float y, float radius, Color color)
        {
            canvas.StrokeColor = color.WithAlpha(0.7f);
            canvas.StrokeSize = 1.5f;
            canvas.DrawLine(x, y + radius * 1.15f, x + radius * 0.15f, y + radius * 2.2f);
            canvas.FillColor = color;
            canvas.FillEllipse(x - radius, y - radius * 1.15f, radius * 2, radius * 2.3f);
            PathF knot = new();
            knot.MoveTo(x, y + radius * 1.1f);
            knot.LineTo(x - radius * 0.14f, y + radius * 1.3f);
            knot.LineTo(x + radius * 0.14f, y + radius * 1.3f);
            knot.Close();
            canvas.FillPath(knot);
            canvas.FillColor = Microsoft.Maui.Graphics.Colors.White.WithAlpha(0.35f);
            canvas.FillEllipse(x - radius * 0.55f, y - radius * 0.8f, radius * 0.45f, radius * 0.6f);
        }

        private static void DrawBurst(ICanvas canvas, float x, float y, float radius, Color color, float progress)
        {
            canvas.StrokeColor = color.WithAlpha(1 - progress);
            canvas.StrokeSize = 3;
            float inner = radius * (0.4f + progress);
            float outer = radius * (0.8f + progress * 1.2f);
            for (int i = 0; i < 8; i++)
            {
                double angle = i * Math.PI / 4;
                float cos = (float)Math.Cos(angle);
                float sin = (float)Math.Sin(angle);
                canvas.DrawLine(x + cos * inner, y + sin * inner, x + cos * outer, y + sin * outer);
            }
        }
    }
}
