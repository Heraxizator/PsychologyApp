using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "White spot": the words of the memory hang in a dark night sky. Over forty seconds a soft white light grows out of the middle,
/// the words lose their edges and fade, and at the end the whole picture is an even, quiet white: the details that trigger the feeling
/// are gone and the nervous system gets a pause.
/// </summary>
public sealed class WhiteSpotStageView : PracticeStageView
{
    private const int DurationSeconds = 40;
    private readonly Label _words = CreateWordsLabel();
    private readonly Border _card;
    private readonly LightDrawable _light = new();
    private readonly GraphicsView _lightView;
    private readonly Label _countdown;
    private CancellationTokenSource? _running;

    public WhiteSpotStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StageWhiteSpotTitle, AppStrings.StageWhiteSpotHint)
    {
        (Brush night, View[] stars) = StageScenery.Night();
        SetScene(night, darkScene: true, stars);

        _card = CreateWordsCard(_words, onDarkScene: true);
        _lightView = new GraphicsView { Drawable = _light, InputTransparent = true, BackgroundColor = Colors.Transparent };
        _countdown = new Label
        {
            Style = AppStyle("CaptionStyle"),
            FontFamily = "InterMedium",
            TextColor = Color.FromRgba(255, 255, 255, 0.8),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 14),
            IsVisible = false
        };

        Canvas.Children.Add(_card);
        Canvas.Children.Add(_lightView);
        Canvas.Children.Add(_countdown);
        Canvas.Children.Add(EmptyLabel);
        SetStartAction();
        OnSourceTextChanged();
        Unloaded += (_, _) => _running?.Cancel();
    }

    protected override void OnSourceTextChanged()
    {
        _words.Text = SourceText;
        _card.IsVisible = HasSource;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void SetStartAction() =>
        SetActions(CreateButton(AppStrings.StageWhiteSpotAction, () => RunAsync().FireAndForget()));

    private async Task RunAsync()
    {
        if (!HasSource || _running is not null)
        {
            return;
        }

        _running = new CancellationTokenSource();
        CancellationToken token = _running.Token;
        ShowStatus(string.Empty);
        SetActions(CreateButton(AppStrings.StageWhiteSpotStop, Stop));
        _countdown.IsVisible = true;
        UiHaptics.Tick();

        DateTime started = DateTime.UtcNow;
        try
        {
            while (!token.IsCancellationRequested)
            {
                double elapsed = (DateTime.UtcNow - started).TotalSeconds;
                double progress = Math.Clamp(elapsed / DurationSeconds, 0, 1);
                Draw(progress);
                _countdown.Text = AppStrings.StageWhiteSpotSeconds((int)Math.Ceiling(DurationSeconds - elapsed));
                if (progress >= 1)
                {
                    break;
                }

                await Task.Delay(ReduceMotion.IsEnabled ? 1000 : 50, token);
            }

            _countdown.IsVisible = false;
            UiHaptics.Tick();
            ShowStatus(AppStrings.StageWhiteSpotDone);
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

    /// <summary>
    /// The light grows over the whole time; the words blur away in the first half; in the last fifth the night itself turns
    /// to soft white, so the end is a calm, even light rather than an empty card.
    /// </summary>
    private void Draw(double progress)
    {
        double eased = Easing.CubicOut.Ease(progress);
        double wordsFade = Math.Clamp(progress / 0.55, 0, 1);
        _words.Opacity = 1 - wordsFade;
        _card.Opacity = 1 - Math.Clamp((progress - 0.25) / 0.5, 0, 1);
        _card.Scale = 1 + 0.06 * progress;
        _light.Intensity = (float)Math.Clamp(progress * 2.4, 0, 1);
        _light.Radius = (float)(0.06 + 0.62 * eased);
        _light.Wash = (float)Math.Clamp((progress - 0.7) / 0.3, 0, 1);
        _lightView.Invalidate();
        _countdown.TextColor = progress > 0.75 ? Color.FromRgba(40, 50, 80, 0.7) : Color.FromRgba(255, 255, 255, 0.8);
    }

    private void Stop() => _running?.Cancel();

    private void Reset()
    {
        _countdown.IsVisible = false;
        ShowStatus(string.Empty);
        Draw(0);
        SetStartAction();
    }
}
