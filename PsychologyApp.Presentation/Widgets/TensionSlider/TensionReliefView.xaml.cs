using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TensionSlider;

/// <summary>
/// What a practice did, shown as a picture: a haze in the colour of the tension that thins out and shrinks while the number settles from
/// the value before the practice to the value after it. With reduced motion the end state appears at once. Hidden until both values are known.
/// </summary>
public partial class TensionReliefView : ContentView
{
    private const string AnimationName = "tension-relief";
    private const uint DurationMs = 1600;

    public static readonly BindableProperty BeforeProperty = BindableProperty.Create(
        nameof(Before), typeof(int), typeof(TensionReliefView), -1, propertyChanged: (b, _, _) => ((TensionReliefView)b).Refresh());

    public static readonly BindableProperty AfterProperty = BindableProperty.Create(
        nameof(After), typeof(int), typeof(TensionReliefView), -1, propertyChanged: (b, _, _) => ((TensionReliefView)b).Refresh());

    private double _shown = -1;

    public TensionReliefView()
    {
        InitializeComponent();
        Unloaded += (_, _) => this.AbortAnimation(AnimationName);
        Refresh();
    }

    public int Before
    {
        get => (int)GetValue(BeforeProperty);
        set => SetValue(BeforeProperty, value);
    }

    public int After
    {
        get => (int)GetValue(AfterProperty);
        set => SetValue(AfterProperty, value);
    }

    private void Refresh()
    {
        bool known = Before is >= TensionScale.Min and <= TensionScale.Max && After is >= TensionScale.Min and <= TensionScale.Max;
        if (!known)
        {
            this.AbortAnimation(AnimationName);
            _shown = -1;
            Opacity = 0;
            return;
        }

        Opacity = 1;
        CaptionLabel.Text = $"{Before} → {After}";
        SemanticProperties.SetDescription(this, AppStrings.ReliefDescription(Before, After));

        // The number first appears at the "before" value and settles; a change of the rating later continues from where it stands.
        double from = _shown < 0 ? Before : _shown;
        double to = After;
        this.AbortAnimation(AnimationName);
        if (ReduceMotion.IsEnabled || Math.Abs(from - to) < 0.01)
        {
            Show(to);
            return;
        }

        Show(from);
        new Animation(Show, from, to, Easing.SinInOut).Commit(this, AnimationName, 16, DurationMs);
    }

    private void Show(double value)
    {
        _shown = value;
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        double t = Math.Clamp(value / TensionScale.Max, 0, 1);
        Color color = Color.FromArgb(TensionScale.HexAt(value, dark));

        // High tension: a big, dense haze. Low tension: a small, pale one.
        Haze.Scale = 0.72 + 0.28 * t;
        Haze.BackgroundColor = color.WithAlpha((float)(0.14 + 0.30 * t));
        Core.Scale = 0.86 + 0.14 * t;
        Core.BackgroundColor = color.WithAlpha((float)(0.30 + 0.30 * t));
        ValueLabel.Text = ((int)Math.Round(value)).ToString();
        ValueLabel.TextColor = Color.FromArgb(MoodPalette.Text(dark));
    }
}
