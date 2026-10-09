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

    private readonly BreathingPattern _pattern = BreathingPattern.Square;
    private BreathPhaseKind? _lastPhase;
    private bool _running;

    public BreathingPacerView()
    {
        InitializeComponent();
        SemanticProperties.SetDescription(CircleHost, AppStrings.BreathCircleLabel);
        ToggleButton.TapCommand = new Command(Toggle);
        Unloaded += (_, _) => Stop();
        ShowReady();
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
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        (byte r, byte g, byte b) = BreathingColors.At(calm, dark);
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
