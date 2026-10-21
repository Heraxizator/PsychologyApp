using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "Say it again": the phrase sits in a large circle; after saying it out loud the person taps the circle. A ripple marks each repeat,
/// the count grows, and the words grow a little quieter on screen, as a worn-out phrase does. At 5, 7 and 10 repeats a question
/// invites to listen whether it still sounds as strong.
/// </summary>
public sealed class RepeatStageView : PracticeStageView
{
    public static readonly BindableProperty RepeatCommandProperty = BindableProperty.Create(
        nameof(RepeatCommand), typeof(ICommand), typeof(RepeatStageView));

    public static readonly BindableProperty RepeatCountProperty = BindableProperty.Create(
        nameof(RepeatCount), typeof(int), typeof(RepeatStageView), 0,
        propertyChanged: (b, o, n) => ((RepeatStageView)b).OnCountChanged((int)o, (int)n));

    private const double CircleSize = 196;
    private static readonly int[] CheckIns = [5, 7, 10];
    private const int IdleBreaths = 3;

    private readonly Label _words;
    private readonly Border _circle;
    private readonly Ellipse _ripple;
    private readonly Label _count;

    public RepeatStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StageRepeatTitle, AppStrings.StageRepeatHint)
    {
        string icon = TechniqueFlavors.Colors(flavor, IsDark)?.Icon ?? "#2656A8";
        _words = new Label
        {
            Style = AppResources.Find<Style>("EmphasisBodyStyle"),
            FontFamily = "InterSemiBold",
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.WordWrap,
            MaxLines = 5,
            Margin = new Thickness(22)
        };
        _circle = new Border
        {
            WidthRequest = CircleSize,
            HeightRequest = CircleSize,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Stroke = new SolidColorBrush(Color.FromArgb(icon)),
            StrokeShape = new RoundRectangle { CornerRadius = CircleSize / 2 },
            BackgroundColor = AppResources.ThemedColor("SurfaceLight", "SurfaceDark", Colors.Gray),
            // At night the halo is only a hint: a bright ring around the circle would glare in a dark room.
            Shadow = new Shadow { Brush = new SolidColorBrush(Color.FromArgb(icon)), Opacity = IsDark ? 0.1f : 0.25f, Radius = 18 },
            StrokeThickness = IsDark ? 2 : 3,
            Content = _words
        };
        _ripple = new Ellipse
        {
            WidthRequest = CircleSize,
            HeightRequest = CircleSize,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Stroke = new SolidColorBrush(Color.FromArgb(icon)),
            StrokeThickness = 2,
            Opacity = 0,
            InputTransparent = true
        };
        _count = new Label
        {
            Style = AppResources.Find<Style>("CaptionStyle"),
            FontFamily = "InterSemiBold",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 6)
        };
        _circle.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Repeat) });
        SemanticProperties.SetDescription(_circle, AppStrings.StageRepeatButton);

        Canvas.Children.Add(_ripple);
        Canvas.Children.Add(_circle);
        Canvas.Children.Add(_count);
        Canvas.Children.Add(EmptyLabel);
        SetActions(CreateButton(AppStrings.StageRepeatButton, Repeat));
        OnSourceTextChanged();
        OnCountChanged(0, RepeatCount);
        Loaded += (_, _) => IdlePulseAsync().FireAndForget();
    }

    public ICommand? RepeatCommand
    {
        get => (ICommand?)GetValue(RepeatCommandProperty);
        set => SetValue(RepeatCommandProperty, value);
    }

    public int RepeatCount
    {
        get => (int)GetValue(RepeatCountProperty);
        set => SetValue(RepeatCountProperty, value);
    }

    protected override void OnSourceTextChanged()
    {
        _words.Text = SourceText;
        _circle.IsVisible = HasSource;
        _count.IsVisible = HasSource;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void Repeat()
    {
        if (!HasSource)
        {
            return;
        }

        if (RepeatCommand?.CanExecute(null) == true)
        {
            RepeatCommand.Execute(null);
        }

        UiHaptics.Tick();
        if (UiAnimations.ShouldAnimate(_circle))
        {
            RippleAsync().FireAndForget();
        }
    }

    private void OnCountChanged(int before, int now)
    {
        _count.Text = AppStrings.StageRepeatCount(now);

        // A phrase said many times loses its weight: the words fade a little with every repeat, never below readable.
        _words.Opacity = Math.Max(0.45, 1 - now * 0.06);
        if (now > before && CheckIns.Contains(now))
        {
            ShowStatus(AppStrings.StageRepeatCheck);
        }
        else if (now == 0)
        {
            ShowStatus(string.Empty);
        }
    }

    private async Task RippleAsync()
    {
        _ripple.CancelAnimations();
        _ripple.Scale = 1;
        _ripple.Opacity = 0.7;
        await Task.WhenAll(
            _circle.ScaleToAsync(0.94, 90, Easing.CubicOut),
            _ripple.ScaleToAsync(1.35, 600, Easing.CubicOut),
            _ripple.FadeToAsync(0, 600, Easing.CubicOut));
        await _circle.ScaleToAsync(1, 260, Easing.SpringOut);
    }

    /// <summary>
    /// A few slow breaths of the circle while it waits for the next repeat. Deliberately not endless: a looping animation keeps the
    /// screen busy for the battery and for accessibility services, which wait for the interface to settle.
    /// </summary>
    private async Task IdlePulseAsync()
    {
        for (int breath = 0; breath < IdleBreaths && UiAnimations.ShouldAnimate(_circle); breath++)
        {
            await _circle.ScaleToAsync(1.03, 1600, Easing.SinInOut);
            await _circle.ScaleToAsync(1, 1600, Easing.SinInOut);
        }
    }
}
