using System.Windows.Input;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.BreathingPacer;

/// <summary>
/// A circle that breathes with the person: it grows while they breathe in, stays large while they hold, shrinks while they breathe out,
/// and its colour moves from warm to cool as the cycles pass. With reduced motion the circle stays still and only the words and counts change.
/// </summary>
public partial class BreathingPacerView : ContentView
{
    private const string AnimationName = "breathing";
    private const double HaloLagSeconds = 0.45;

    private const int ImmersiveStartDelayMs = 900;

    public static readonly BindableProperty FullscreenCommandProperty = BindableProperty.Create(
        nameof(FullscreenCommand), typeof(ICommand), typeof(BreathingPacerView), null,
        propertyChanged: (bindable, _, _) => ((BreathingPacerView)bindable).UpdateFullscreenButton());

    public static readonly BindableProperty IsImmersiveProperty = BindableProperty.Create(
        nameof(IsImmersive), typeof(bool), typeof(BreathingPacerView), false,
        propertyChanged: (bindable, _, newValue) => ((BreathingPacerView)bindable).ApplyImmersive((bool)newValue));

    private readonly BreathingPattern _pattern = BreathingPattern.Square;
    private BreathPhaseKind? _lastPhase;
    private bool _running;

    public BreathingPacerView()
    {
        InitializeComponent();
        SemanticProperties.SetDescription(CircleHost, AppStrings.BreathCircleLabel);
        ToggleButton.TapCommand = new Command(Toggle);
        FullscreenButton.BodyText = AppStrings.BreathFullscreen;
        Unloaded += (_, _) => Stop();
        ShowReady();
    }

    /// <summary>Asks the host to open the full-screen breathing page. The button shows only when the host gives a command.</summary>
    public ICommand? FullscreenCommand
    {
        get => (ICommand?)GetValue(FullscreenCommandProperty);
        set => SetValue(FullscreenCommandProperty, value);
    }

    /// <summary>The circle on its own: no card, a larger circle, the dark-theme colours on whatever dark page it sits on, and it starts by itself.</summary>
    public bool IsImmersive
    {
        get => (bool)GetValue(IsImmersiveProperty);
        set => SetValue(IsImmersiveProperty, value);
    }

    /// <summary>After every frame: how large the circle is (0.55 to 1) and its colour, so the page behind it can breathe too.</summary>
    public event Action<double, Color>? Rendered;

    private void UpdateFullscreenButton()
    {
        if (FullscreenCommand is not null)
        {
            FullscreenButton.TapCommand = FullscreenCommand;
        }

        FullscreenButton.IsVisible = FullscreenCommand is not null && !IsImmersive;
    }

    private void ApplyImmersive(bool immersive)
    {
        UpdateFullscreenButton();
        if (!immersive)
        {
            return;
        }

        Card.Style = new Style(typeof(Border));
        Card.BackgroundColor = Colors.Transparent;
        Card.StrokeThickness = 0;
        Card.Padding = 0;
        Card.Margin = 0;
        CircleHost.WidthRequest = 390;
        CircleHost.HeightRequest = 390;
        ToggleButton.Variant = "Secondary";
        Halo.WidthRequest = 300;
        Halo.HeightRequest = 300;
        Halo.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 150 };
        Circle.WidthRequest = 260;
        Circle.HeightRequest = 260;
        Circle.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 130 };
        CountLabel.FontSize = 48;
        CycleLabel.TextColor = Color.FromArgb("#CCFFFFFF");
        Render(BreathingPattern.SmallScale, 0);
        Loaded += async (_, _) =>
        {
            await Task.Delay(ImmersiveStartDelayMs);
            if (!_running && IsImmersive)
            {
                Start();
            }
        };
    }

    private void ShowReady()
    {
        PhaseLabel.Text = string.Empty;
        CountLabel.Text = string.Empty;
        CycleLabel.Text = AppStrings.BreathReady;
        ToggleButton.BodyText = AppStrings.BreathStart;
        Render(BreathingPattern.SmallScale, 0);
    }

    private void Toggle()
    {
        if (_running)
        {
            Stop();
            ShowReady();
            return;
        }

        Start();
    }

    private void Start()
    {
        _running = true;
        _lastPhase = null;
        ToggleButton.BodyText = AppStrings.BreathStop;
        double total = _pattern.TotalSeconds;

        Animation animation = new(t => Update(t * total), 0, 1, Easing.Linear);
        animation.Commit(this, AnimationName, 16, (uint)(total * 1000), Easing.Linear, (_, cancelled) =>
        {
            if (!cancelled)
            {
                Finish();
            }
        });
    }

    private void Stop()
    {
        this.AbortAnimation(AnimationName);
        _running = false;
    }

    private void Update(double elapsed)
    {
        BreathPosition position = _pattern.At(elapsed);
        double scale = BreathingPattern.ScaleAt(position.Phase, position.Progress);
        // The halo follows the circle a moment later, so the breath has depth instead of one flat disc.
        BreathPosition trailing = _pattern.At(Math.Max(0, elapsed - HaloLagSeconds));
        Render(scale, _pattern.CalmAt(elapsed), BreathingPattern.ScaleAt(trailing.Phase, trailing.Progress));
        CountLabel.Text = position.SecondsLeft.ToString();
        CycleLabel.Text = string.Format(AppStrings.BreathCycleFormat, position.Cycle, _pattern.Cycles);

        if (_lastPhase != position.Phase.Kind)
        {
            _lastPhase = position.Phase.Kind;
            string name = PhaseName(position.Phase.Kind);
            PhaseLabel.Text = name;
            SemanticScreenReader.Announce(name);
            Tick();
        }
    }

    private void Render(double scale, double calm, double? haloScale = null)
    {
        bool dark = IsImmersive || Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        (byte r, byte g, byte b) = IsImmersive ? BreathingColors.ImmersiveAt(calm) : BreathingColors.At(calm, dark);
        Color color = Color.FromRgb(r, g, b);
        double alpha = BreathingColors.AlphaAt(scale);
        Color text = Color.FromArgb(BreathingColors.TextHex(dark));
        PhaseLabel.TextColor = text;
        CountLabel.TextColor = text;

        // Reduced motion keeps the circle at a steady middle size; the words and counts still carry the rhythm.
        double shown = ReduceMotion.IsEnabled ? 0.8 : scale;
        Circle.Scale = shown;
        Circle.BackgroundColor = color.WithAlpha((float)alpha);
        Halo.Scale = (ReduceMotion.IsEnabled ? 0.8 : haloScale ?? scale) * 1.18;
        Halo.BackgroundColor = color.WithAlpha(0.22f);
        Rendered?.Invoke(scale, color);
    }

    private void Finish()
    {
        _running = false;
        PhaseLabel.Text = string.Empty;
        CountLabel.Text = string.Empty;
        CycleLabel.Text = AppStrings.BreathDone;
        ToggleButton.BodyText = AppStrings.BreathAgain;
        Render(BreathingPattern.SmallScale, 1);
        Tick();
    }

    private static string PhaseName(BreathPhaseKind kind) => kind switch
    {
        BreathPhaseKind.Inhale => AppStrings.BreathInhale,
        BreathPhaseKind.Exhale => AppStrings.BreathExhale,
        _ => AppStrings.BreathHold
    };

    private static void Tick() => UiHaptics.Tick();
}
