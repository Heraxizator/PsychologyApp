using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "Fifty years later": the problem stands in the same open landscape, and a slider moves time from now to fifty years ahead.
/// With every mark the card drifts toward the horizon, shrinks and fades, and the day turns to a soft evening, so the person sees
/// how the weight of the problem changes with distance in time, and then rates it.
/// </summary>
public sealed class TimeTravelStageView : PracticeStageView
{
    private static readonly double ForegroundY = CanvasHeight * 0.14;
    private static readonly double HorizonY = CanvasHeight * StageScenery.HorizonShare - CanvasHeight / 2;

    private readonly Label _words = CreateWordsLabel();
    private readonly Border _card;
    private readonly Border _when;
    private readonly Label _whenLabel;
    private readonly Border _evening;
    private readonly Slider _time;
    private int _stop;

    public TimeTravelStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StageFutureTitle, AppStrings.StageFutureHint)
    {
        string accent = TechniqueFlavors.Colors(flavor, dark: false)?.Icon ?? "#2656A8";
        _evening = new Border
        {
            StrokeThickness = 0,
            Background = StageScenery.Gradient("#3E2F6B", "#C77B5A", "#33F2B36A", "#00F2B36A"),
            Opacity = 0
        };
        SetScene(StageScenery.LandscapeBase(IsDark), darkScene: IsDark, [.. StageScenery.Landscape(CanvasHeight, IsDark), _evening]);

        _card = CreateWordsCard(_words);
        _whenLabel = new Label
        {
            Style = AppResources.Find<Style>("CaptionStyle"),
            FontFamily = "InterSemiBold",
            TextColor = IsDark ? Color.FromArgb("#E8ECF6") : Color.FromArgb("#1F2A44")
        };
        _when = new Border
        {
            Padding = new Thickness(14, 6),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
            BackgroundColor = IsDark ? Color.FromRgba(16, 20, 34, 0.78) : Color.FromRgba(255, 255, 255, 0.85),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 14, 0, 0),
            Content = _whenLabel
        };
        _time = new Slider
        {
            Minimum = 0,
            Maximum = AppStrings.StageFutureStopCount - 1,
            Value = 0,
            ThumbColor = Color.FromArgb(accent),
            MinimumTrackColor = Color.FromArgb(accent)
        };
        _time.ValueChanged += OnTimeChanged;

        Canvas.Children.Add(_card);
        Canvas.Children.Add(_when);
        Canvas.Children.Add(EmptyLabel);
        Grid ends = new() { ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star)], Margin = new Thickness(6, -4, 6, 0) };
        ends.Add(new Label { Text = AppStrings.StageFutureStop(0), Style = AppResources.Find<Style>("CaptionStyle") }, 0);
        ends.Add(new Label { Text = AppStrings.StageFutureStop(AppStrings.StageFutureStopCount - 1), Style = AppResources.Find<Style>("CaptionStyle"), HorizontalTextAlignment = TextAlignment.End }, 1);
        Actions.Children.Add(new VerticalStackLayout { Children = { _time, ends } });
        Show(0, animate: false);
        OnSourceTextChanged();
    }

    protected override void OnSourceTextChanged()
    {
        _words.Text = SourceText;
        _card.IsVisible = HasSource;
        _when.IsVisible = HasSource;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void OnTimeChanged(object? sender, ValueChangedEventArgs e)
    {
        int stop = (int)Math.Round(e.NewValue);
        if (Math.Abs(_time.Value - stop) > 0.001)
        {
            _time.Value = stop;
            return;
        }

        if (stop != _stop)
        {
            UiHaptics.Tick();
            Show(stop, animate: true);
        }
    }

    private void Show(int stop, bool animate)
    {
        _stop = stop;
        _whenLabel.Text = AppStrings.StageFutureStop(stop);
        SemanticProperties.SetDescription(_time, _whenLabel.Text);

        double distance = stop / (double)(AppStrings.StageFutureStopCount - 1);
        double scale = 1 - 0.7 * distance;
        double y = ForegroundY + (HorizonY - ForegroundY) * distance;
        double opacity = 1 - 0.55 * distance;
        double evening = 0.42 * distance;
        if (animate && UiAnimations.ShouldAnimate(_card))
        {
            _card.ScaleToAsync(scale, UiAnimations.MediumDuration + 120, Easing.CubicOut).FireAndForget();
            _card.TranslateToAsync(0, y, UiAnimations.MediumDuration + 120, Easing.CubicOut).FireAndForget();
            _card.FadeToAsync(opacity, UiAnimations.MediumDuration + 120, Easing.CubicOut).FireAndForget();
            _evening.FadeToAsync(evening, UiAnimations.MediumDuration + 300, Easing.SinInOut).FireAndForget();
        }
        else
        {
            _card.Scale = scale;
            _card.TranslationY = y;
            _card.Opacity = opacity;
            _evening.Opacity = evening;
        }
    }
}
