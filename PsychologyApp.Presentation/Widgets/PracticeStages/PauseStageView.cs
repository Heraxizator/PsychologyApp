using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "Check it": one minute without the looping thought. In a quiet teal evening the thought steps back to a faint line at the top, a soft
/// light in the middle breathes slowly (five seconds in, five out) and the remaining seconds count down in it; at the end a question
/// asks what changed.
/// </summary>
public sealed class PauseStageView : PracticeStageView
{
    private const int DurationSeconds = 60;
    private const double BreathSeconds = 5;

    private readonly Label _thought;
    private readonly LightDrawable _glow = new() { Core = Color.FromArgb("#9FE6DD"), Intensity = 0.75f, Radius = RestRadius };
    private readonly GraphicsView _light;
    private const float RestRadius = 0.2f;
    private const float BreathRadius = 0.16f;
    private readonly Label _seconds;
    private CancellationTokenSource? _running;

    public PauseStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StagePauseTitle, AppStrings.StagePauseHint)
    {
        SetScene(StageScenery.Twilight(), darkScene: true);
        _thought = new Label
        {
            Style = AppStyle("BodyStyle"),
            TextColor = Color.FromRgba(255, 255, 255, 0.85),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(28, 18, 28, 0),
            MaxLines = 2,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        _light = new GraphicsView { Drawable = _glow, InputTransparent = true, BackgroundColor = Colors.Transparent };
        _seconds = new Label
        {
            Style = AppStyle("SectionTitleStyle"),
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        Canvas.Children.Add(_light);
        Canvas.Children.Add(_thought);
        Canvas.Children.Add(_seconds);
        Canvas.Children.Add(EmptyLabel);
        SetStartAction();
        OnSourceTextChanged();
        Unloaded += (_, _) => _running?.Cancel();
    }

    protected override void OnSourceTextChanged()
    {
        _thought.Text = SourceText;
        _thought.IsVisible = HasSource;
        _light.IsVisible = HasSource;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void SetStartAction() =>
        SetActions(CreateButton(AppStrings.StagePauseStart, () => RunAsync().FireAndForget()));

    private async Task RunAsync()
    {
        if (!HasSource || _running is not null)
        {
            return;
        }

        _running = new CancellationTokenSource();
        CancellationToken token = _running.Token;
        ShowStatus(string.Empty);
        SetActions(CreateButton(AppStrings.StagePauseStop, () => _running?.Cancel()));
        _seconds.IsVisible = true;
        if (UiAnimations.ShouldAnimate(_thought))
        {
            _thought.FadeToAsync(0.3, 1500, Easing.CubicOut).FireAndForget();
        }

        UiHaptics.Tick();
        DateTime started = DateTime.UtcNow;
        try
        {
            while (!token.IsCancellationRequested)
            {
                double elapsed = (DateTime.UtcNow - started).TotalSeconds;
                _seconds.Text = Math.Max(0, (int)Math.Ceiling(DurationSeconds - elapsed)).ToString(System.Globalization.CultureInfo.CurrentCulture);
                if (!ReduceMotion.IsEnabled)
                {
                    // In for five seconds, out for five: a slow wave the eye can follow without counting.
                    double phase = elapsed % (BreathSeconds * 2) / (BreathSeconds * 2);
                    _glow.Radius = RestRadius + BreathRadius * (float)Math.Sin(phase * Math.PI);
                    _light.Invalidate();
                }

                if (elapsed >= DurationSeconds)
                {
                    break;
                }

                await Task.Delay(ReduceMotion.IsEnabled ? 1000 : 40, token);
            }

            _seconds.IsVisible = false;
            UiHaptics.Tick();
            ShowStatus(AppStrings.StagePauseDone);
            SetActions(CreateButton(AppStrings.StageAgain, Reset));
        }
        catch (OperationCanceledException)
        {
            Reset();
        }
        finally
        {
            _running?.Dispose();
            _running = null;
        }
    }

    private void Reset()
    {
        _seconds.IsVisible = false;
        ShowStatus(string.Empty);
        _glow.Radius = RestRadius;
        _light.Invalidate();
        _thought.Opacity = 1;
        SetStartAction();
    }
}
