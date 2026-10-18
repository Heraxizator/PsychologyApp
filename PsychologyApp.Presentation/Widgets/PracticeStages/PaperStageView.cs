using Microsoft.Maui.Controls.Shapes;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "Sheet of paper": the thought is written on a sheet that can be crumpled into a ball, torn in two, or thrown away with a swipe up.
/// The ritual of getting rid of the sheet, done with the hands instead of only imagined.
/// </summary>
public sealed class PaperStageView : PracticeStageView
{
    public static readonly BindableProperty KeepCommandProperty = BindableProperty.Create(
        nameof(KeepCommand), typeof(System.Windows.Input.ICommand), typeof(PaperStageView));

    private const double SheetWidth = 230;
    private const double SheetHeight = 170;

    /// <summary>Warm paper; under the evening lamp a shade dimmer, so a white sheet does not flash in a dark room.</summary>
    private static Color PaperColour => Color.FromArgb(IsDark ? "#E6DFCE" : "#FFFDF6");

    private readonly Grid _sheet;
    private readonly Label _words;
    private readonly Grid _halves;
    private bool _crumpled;
    private bool _gone;

    public PaperStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StagePaperTitle, AppStrings.StagePaperHint)
    {
        _words = CreatePaperWords();
        _sheet = CreateSheet(_words);
        _halves = new Grid { IsVisible = false, InputTransparent = true };
        SetScene(StageScenery.Desk(IsDark), darkScene: IsDark);

        SwipeGestureRecognizer swipeUp = new() { Direction = SwipeDirection.Up };
        swipeUp.Swiped += (_, _) => ThrowAsync().FireAndForget();
        _sheet.GestureRecognizers.Add(swipeUp);

        Canvas.Children.Add(_sheet);
        Canvas.Children.Add(_halves);
        Canvas.Children.Add(EmptyLabel);
        SetSheetActions();
        OnSourceTextChanged();
    }

    /// <summary>Records the thought in the practice before the sheet is destroyed, so letting it go never loses what was written.</summary>
    public System.Windows.Input.ICommand? KeepCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(KeepCommandProperty);
        set => SetValue(KeepCommandProperty, value);
    }

    protected override void OnSourceTextChanged()
    {
        if (!_gone && !_crumpled)
        {
            _words.Text = SourceText;
        }

        _sheet.IsVisible = HasSource && !_gone;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void SetSheetActions() => SetActions(
        CreateTile("Compress", AppStrings.StagePaperCrumple, () => CrumpleAsync().FireAndForget()),
        CreateTile("ContentCut", AppStrings.StagePaperTear, () => TearAsync().FireAndForget()),
        CreateTile("ArrowUpward", AppStrings.StagePaperThrow, () => ThrowAsync().FireAndForget()));

    /// <summary>The sheet folds in on itself: smaller, turned, rounder, the words blur into creases.</summary>
    private async Task CrumpleAsync()
    {
        if (!HasSource || _crumpled || _gone)
        {
            return;
        }

        _crumpled = true;
        UiHaptics.Tick();
        if (UiAnimations.ShouldAnimate(_sheet))
        {
            for (int squeeze = 0; squeeze < 3; squeeze++)
            {
                await Task.WhenAll(
                    _sheet.ScaleToAsync(0.85 - squeeze * 0.15, 140, Easing.CubicIn),
                    _sheet.RotateToAsync(-12 + squeeze * 14, 140, Easing.CubicIn));
                UiHaptics.Tick();
            }
        }

        // A ball of paper: round (the width squeezed to the height), lit in the middle and shaded at the edges, the ruling gone.
        _sheet.Scale = 0.5;
        _sheet.ScaleX = SheetHeight / SheetWidth;
        _words.Opacity = 0.2;
        if (_sheet.Children[0] is Border paper)
        {
            paper.StrokeShape = new RoundRectangle { CornerRadius = SheetHeight / 2 };
            paper.Background = new RadialGradientBrush(
                [new GradientStop(PaperColour, 0f), new GradientStop(Color.FromArgb("#EDE6D6"), 0.6f), new GradientStop(Color.FromArgb("#C9BFAA"), 1f)],
                new Point(0.4, 0.35),
                0.65);
            if (paper.Content is Grid layers && layers.Children.FirstOrDefault() is Grid ruling)
            {
                ruling.Opacity = 0;
            }
        }

        SetActions(CreateButton(AppStrings.StagePaperThrow, () => ThrowAsync().FireAndForget()));
    }

    /// <summary>Two halves of the same sheet pull apart and fall away.</summary>
    private async Task TearAsync()
    {
        if (!HasSource || _gone)
        {
            return;
        }

        _gone = true;
        _halves.Children.Clear();
        Grid left = CreateHalf(isLeft: true);
        Grid right = CreateHalf(isLeft: false);
        _halves.Children.Add(left);
        _halves.Children.Add(right);
        _halves.IsVisible = true;
        _sheet.IsVisible = false;
        UiHaptics.Tick();

        if (UiAnimations.ShouldAnimate(_halves))
        {
            await Task.WhenAll(
                left.TranslateToAsync(-70, 60, 700, Easing.CubicIn),
                left.RotateToAsync(-28, 700, Easing.CubicIn),
                left.FadeToAsync(0, 700, Easing.CubicIn),
                right.TranslateToAsync(70, 60, 700, Easing.CubicIn),
                right.RotateToAsync(28, 700, Easing.CubicIn),
                right.FadeToAsync(0, 700, Easing.CubicIn));
        }

        Finish();
    }

    /// <summary>Flies up and away with a turn; a swipe up does the same.</summary>
    private async Task ThrowAsync()
    {
        if (!HasSource || _gone)
        {
            return;
        }

        _gone = true;
        UiHaptics.Tick();
        if (UiAnimations.ShouldAnimate(_sheet))
        {
            await Task.WhenAll(
                _sheet.TranslateToAsync(40, -CanvasHeight, 650, Easing.CubicIn),
                _sheet.RotateToAsync(_sheet.Rotation + 160, 650, Easing.CubicIn),
                _sheet.ScaleToAsync(_sheet.Scale * 0.6, 650, Easing.CubicIn),
                _sheet.FadeToAsync(0, 650, Easing.CubicIn));
        }

        _sheet.IsVisible = false;
        Finish();
    }

    private void Finish()
    {
        if (KeepCommand?.CanExecute(null) == true)
        {
            KeepCommand.Execute(null);
        }

        _halves.IsVisible = false;
        ShowStatus(AppStrings.StagePaperDone);
        SetActions(CreateButton(AppStrings.StageAgain, Reset));
    }

    private void Reset()
    {
        _gone = false;
        _crumpled = false;
        _sheet.Scale = 1;
        _sheet.ScaleX = 1;
        _sheet.Rotation = -2;
        _sheet.TranslationX = 0;
        _sheet.TranslationY = 0;
        _sheet.Opacity = 1;
        _words.Opacity = 1;
        if (_sheet.Children[0] is Border paper)
        {
            paper.StrokeShape = new RoundRectangle { CornerRadius = 4 };
            paper.Background = new SolidColorBrush(PaperColour);
            if (paper.Content is Grid layers && layers.Children.FirstOrDefault() is Grid ruling)
            {
                ruling.Opacity = 1;
            }
        }

        ShowStatus(string.Empty);
        OnSourceTextChanged();
        SetSheetActions();
        if (UiAnimations.ShouldAnimate(_sheet))
        {
            _sheet.Opacity = 0;
            _sheet.FadeToAsync(1, UiAnimations.MediumDuration, Easing.CubicOut).FireAndForget();
        }
    }

    private Grid CreateHalf(bool isLeft)
    {
        Label words = CreatePaperWords();
        words.Text = _words.Text;
        Grid half = CreateSheet(words);
        half.Rotation = _sheet.Rotation;
        half.Scale = _sheet.Scale;
        half.Clip = new RectangleGeometry(isLeft
            ? new Rect(0, 0, SheetWidth / 2, SheetHeight)
            : new Rect(SheetWidth / 2, 0, SheetWidth / 2, SheetHeight));
        return half;
    }

    private static Label CreatePaperWords() => new()
    {
        FontSize = 17,
        TextColor = Color.FromArgb("#2B2B2B"),
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalTextAlignment = TextAlignment.Center,
        LineBreakMode = LineBreakMode.WordWrap,
        MaxLines = 5
    };

    /// <summary>A sheet of warm paper with faint ruled lines.</summary>
    private static Grid CreateSheet(Label words)
    {
        Grid lines = new() { RowSpacing = 22, Padding = new Thickness(14, 30, 14, 0), InputTransparent = true };
        for (int i = 0; i < 6; i++)
        {
            lines.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            BoxView line = new() { HeightRequest = 1, Color = Color.FromArgb("#D9E3EF") };
            Grid.SetRow(line, i);
            lines.Children.Add(line);
        }

        Border paper = new()
        {
            WidthRequest = SheetWidth,
            HeightRequest = SheetHeight,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 4 },
            BackgroundColor = PaperColour,
            Shadow = new Shadow { Brush = new SolidColorBrush(Colors.Black), Opacity = 0.22f, Radius = 12, Offset = new Point(0, 6) },
            Content = new Grid { Children = { lines, new ContentView { Padding = new Thickness(16, 12), Content = words } } }
        };

        return new Grid
        {
            WidthRequest = SheetWidth,
            HeightRequest = SheetHeight,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Rotation = -2,
            Children = { paper }
        };
    }
}
