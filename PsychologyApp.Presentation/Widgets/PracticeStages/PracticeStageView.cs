using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.UI.Components;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// A card where an imagination practice happens on screen instead of only in the head: the problem written above becomes an object
/// to shrink, dissolve, crumple, repeat or look back at. Each stage sets its own scenery on <see cref="Canvas"/> (a night sky, a horizon,
/// a desk) and adds its buttons to <see cref="Actions"/>. The buttons are outlined: the one filled button of the page stays "Finish".
/// </summary>
public abstract class PracticeStageView : ContentView
{
    public static readonly BindableProperty SourceTextProperty = BindableProperty.Create(
        nameof(SourceText), typeof(string), typeof(PracticeStageView), string.Empty,
        propertyChanged: (b, _, _) => ((PracticeStageView)b).OnSourceTextChanged());

    protected const double CanvasHeight = 290;

    protected PracticeStageView(TechniqueFlavor flavor, string title, string hint)
    {
        Flavor = flavor;
        TitleLabel = new Label { Text = title, Style = AppResources.Find<Style>("SectionTitleStyle") };
        HintLabel = new Label { Text = hint, Style = AppResources.Find<Style>("CaptionStyle") };
        StatusLabel = new Label
        {
            Style = AppResources.Find<Style>("BodyStyle"),
            FontFamily = "InterMedium",
            HorizontalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };
        Canvas = new Grid { HeightRequest = CanvasHeight, Background = DefaultCanvasBrush() };
        Actions = new Grid { ColumnSpacing = 8 };
        EmptyLabel = new Label
        {
            Text = AppStrings.StageEmptyText,
            Style = AppResources.Find<Style>("BodyStyle"),
            TextColor = Color.FromArgb("#5C5C66"),
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(32, 0)
        };

        Border canvasFrame = new()
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            Content = Canvas
        };

        Content = new Border
        {
            Style = AppResources.Find<Style>("BrandCardStyle"),
            Margin = 0,
            Padding = new Thickness(14, 16, 14, 14),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    new VerticalStackLayout { Spacing = 4, Padding = new Thickness(4, 0), Children = { TitleLabel, HintLabel } },
                    canvasFrame,
                    StatusLabel,
                    Actions
                }
            }
        };
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    protected TechniqueFlavor Flavor { get; }

    protected Label TitleLabel { get; }

    protected Label HintLabel { get; }

    protected Label StatusLabel { get; }

    protected Grid Canvas { get; }

    protected Grid Actions { get; }

    /// <summary>Shown on the canvas while nothing has been written yet.</summary>
    protected Label EmptyLabel { get; }

    protected bool HasSource => !string.IsNullOrWhiteSpace(SourceText);

    protected static bool IsDark => Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;

    protected abstract void OnSourceTextChanged();

    /// <summary>Paints the canvas: a background brush, optional scenery views behind everything else, and the colour of the empty hint.</summary>
    protected void SetScene(Brush background, bool darkScene, params View[] scenery)
    {
        Canvas.Background = background;
        EmptyLabel.TextColor = darkScene ? Color.FromRgba(255, 255, 255, 0.78) : Color.FromArgb("#4F4F5A");
        for (int i = scenery.Length - 1; i >= 0; i--)
        {
            scenery[i].InputTransparent = true;
            Canvas.Children.Insert(0, scenery[i]);
        }
    }

    protected void ShowStatus(string text)
    {
        StatusLabel.Text = text;
        StatusLabel.IsVisible = !string.IsNullOrWhiteSpace(text);
        if (StatusLabel.IsVisible && UiAnimations.ShouldAnimate(StatusLabel))
        {
            StatusLabel.Opacity = 0;
            StatusLabel.TranslationY = 6;
            StatusLabel.FadeToAsync(1, UiAnimations.MediumDuration, Easing.CubicOut).FireAndForget();
            StatusLabel.TranslateToAsync(0, 0, UiAnimations.MediumDuration, Easing.CubicOut).FireAndForget();
        }
    }

    protected void SetActions(params View[] buttons)
    {
        Actions.Children.Clear();
        Actions.ColumnDefinitions.Clear();
        for (int i = 0; i < buttons.Length; i++)
        {
            Actions.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            if (buttons.Length > 2 && buttons[i] is ButtonView button)
            {
                // Three choices share one row: the compact size keeps each word whole.
                button.IsCompact = true;
            }

            Grid.SetColumn(buttons[i], i);
            Actions.Children.Add(buttons[i]);
        }
    }

    protected static ButtonView CreateButton(string text, Action onTap) => new()
    {
        BodyText = text,
        Variant = "Secondary",
        TapCommand = new Command(onTap)
    };

    /// <summary>An outlined tile with an icon over a short caption: three of them fit one row at any text size.</summary>
    protected static View CreateTile(string icon, string text, Action onTap)
    {
        Color accent = AppResources.ThemedColor("PrimaryTextLight", "PrimaryTextDark", Colors.Gray);
        Border tile = new()
        {
            Padding = new Thickness(6, 10),
            StrokeThickness = 1.5,
            Stroke = new SolidColorBrush(AppResources.ThemedColor("Primary", "PrimaryTextDark", Colors.Gray)),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Colors.Transparent,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    new MaterialIconView { IconName = icon, IconSize = 24, IconColor = accent, HorizontalOptions = LayoutOptions.Center },
                    new Label
                    {
                        Text = text,
                        Style = AppResources.Find<Style>("CaptionStyle"),
                        FontFamily = "InterSemiBold",
                        TextColor = accent,
                        HorizontalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap
                    }
                }
            }
        };
        SemanticProperties.SetDescription(tile, text);
        tile.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() =>
        {
            UiHaptics.Tick();
            onTap();
        }) });
        return tile;
    }

    /// <summary>The problem as a card in the scene. On a dark scene it is a soft, translucent plate with light words.</summary>
    protected static Border CreateWordsCard(Label words, bool onDarkScene = false)
    {
        if (onDarkScene)
        {
            words.TextColor = Colors.White;
        }

        return new Border
        {
            Padding = new Thickness(20, 14),
            MaximumWidthRequest = 270,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            // A hairline instead of a drop shadow: Android draws the shadow of a rounded card as a grey box behind it.
            StrokeThickness = 1,
            Stroke = new SolidColorBrush(onDarkScene ? Color.FromRgba(255, 255, 255, 0.22) : Color.FromRgba(0, 0, 0, 0.08)),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
            BackgroundColor = onDarkScene ? Color.FromRgba(255, 255, 255, 0.12) : AppResources.ThemedColor("SurfaceLight", "SurfaceDark", Colors.Gray),
            Content = words
        };
    }

    protected static Label CreateWordsLabel() => new()
    {
        Style = AppResources.Find<Style>("EmphasisBodyStyle"),
        FontFamily = "InterSemiBold",
        HorizontalTextAlignment = TextAlignment.Center,
        LineBreakMode = LineBreakMode.WordWrap,
        MaxLines = 4
    };

    /// <summary>A soft round light: bright in the middle, fading to nothing at the edge. Centred in the canvas.</summary>
    protected static Border CreateGlow(Color colour, double size, double centreAlpha = 1) => new()
    {
        WidthRequest = size,
        HeightRequest = size,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
        StrokeThickness = 0,
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = size / 2 },
        InputTransparent = true,
        Background = new RadialGradientBrush(
            [
                new GradientStop(colour.WithAlpha((float)centreAlpha), 0f),
                new GradientStop(colour.WithAlpha((float)(centreAlpha * 0.55)), 0.45f),
                new GradientStop(colour.WithAlpha(0f), 1f)
            ],
            new Point(0.5, 0.5),
            0.5)
    };

    protected static LinearGradientBrush Vertical(params string[] hexStops)
    {
        GradientStopCollection stops = [];
        for (int i = 0; i < hexStops.Length; i++)
        {
            stops.Add(new GradientStop(Color.FromArgb(hexStops[i]), hexStops.Length == 1 ? 0f : i / (float)(hexStops.Length - 1)));
        }

        return new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
    }

    /// <summary>Without scenery of its own, a stage glows softly in the colour of the practice's group.</summary>
    private Brush DefaultCanvasBrush()
    {
        string tile = TechniqueFlavors.Colors(Flavor, IsDark)?.Tile ?? (IsDark ? "#1F3050" : "#DCE8FA");
        Color page = AppResources.ThemedColor("PageBackground", "PageBackgroundDark", Colors.Gray);
        return new RadialGradientBrush(
            [new GradientStop(Color.FromArgb(tile), 0f), new GradientStop(page, 1f)],
            new Point(0.5, 0.45),
            0.75);
    }
}
