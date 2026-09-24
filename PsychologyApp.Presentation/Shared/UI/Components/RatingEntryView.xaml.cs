using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class RatingEntryView : ContentView
{
    private bool _suppressSliderCallback;
    private int? _lastHapticStep;

    public RatingEntryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(RatingEntryView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(RatingEntryView), string.Empty, BindingMode.TwoWay, propertyChanged: OnBodyTextChanged);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public static readonly BindableProperty KindProperty =
        BindableProperty.Create(nameof(Kind), typeof(EntryFieldKind), typeof(RatingEntryView), EntryFieldKind.Rating0To10, propertyChanged: OnKindChanged);

    public EntryFieldKind Kind
    {
        get => (EntryFieldKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(RatingEntryView), 0d);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        private set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(RatingEntryView), 10d);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        private set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty MinimumCaptionProperty =
        BindableProperty.Create(nameof(MinimumCaption), typeof(string), typeof(RatingEntryView), "0");

    public string MinimumCaption
    {
        get => (string)GetValue(MinimumCaptionProperty);
        private set => SetValue(MinimumCaptionProperty, value);
    }

    public static readonly BindableProperty MaximumCaptionProperty =
        BindableProperty.Create(nameof(MaximumCaption), typeof(string), typeof(RatingEntryView), "10");

    public string MaximumCaption
    {
        get => (string)GetValue(MaximumCaptionProperty);
        private set => SetValue(MaximumCaptionProperty, value);
    }

    public static readonly BindableProperty SliderValueProperty =
        BindableProperty.Create(nameof(SliderValue), typeof(double), typeof(RatingEntryView), 0d, BindingMode.TwoWay);

    public double SliderValue
    {
        get => (double)GetValue(SliderValueProperty);
        set => SetValue(SliderValueProperty, value);
    }

    public static readonly BindableProperty ValueDisplayProperty =
        BindableProperty.Create(nameof(ValueDisplay), typeof(string), typeof(RatingEntryView), string.Empty);

    public string ValueDisplay
    {
        get => (string)GetValue(ValueDisplayProperty);
        private set => SetValue(ValueDisplayProperty, value);
    }

    public static readonly BindableProperty HasValueProperty =
        BindableProperty.Create(nameof(HasValue), typeof(bool), typeof(RatingEntryView), false);

    /// <summary>False while the question has not been answered yet — the pill and slider then show a neutral "unanswered" state.</summary>
    public bool HasValue
    {
        get => (bool)GetValue(HasValueProperty);
        private set => SetValue(HasValueProperty, value);
    }

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(RatingEntryView), Colors.Gray);

    /// <summary>Interpolated between "calm" and "intense" colors as the value moves, so the pill and track carry meaning, not just position.</summary>
    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        private set => SetValue(AccentColorProperty, value);
    }

    /// <summary>Re-syncs the visual state for a freshly bound entry, even when its text happens to equal the previous entry's text (e.g. both empty).</summary>
    public void ForceSync() => SyncFromBodyText(force: true);

    private static void OnKindChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RatingEntryView)bindable;
        var kind = (EntryFieldKind)newValue;
        if (kind == EntryFieldKind.RatingNeg10To10)
        {
            view.Minimum = -10;
            view.Maximum = 10;
            view.MinimumCaption = "-10";
            view.MaximumCaption = "10";
        }
        else
        {
            view.Minimum = 0;
            view.Maximum = 10;
            view.MinimumCaption = "0";
            view.MaximumCaption = "10";
        }

        view.SyncFromBodyText(force: true);
    }

    private static void OnBodyTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RatingEntryView)bindable;
        if (!view._suppressSliderCallback)
        {
            view.SyncFromBodyText();
        }
    }

    private void SyncFromBodyText(bool force = false)
    {
        _lastHapticStep = null;

        if (double.TryParse(BodyText, out double parsed))
        {
            parsed = Math.Clamp(parsed, Minimum, Maximum);
            _suppressSliderCallback = true;
            SliderValue = parsed;
            _suppressSliderCallback = false;
            HasValue = true;
        }
        else if (force)
        {
            // Nothing answered yet for this question: rest the thumb at the low end instead of
            // silently keeping whatever position the previous question left it in.
            _suppressSliderCallback = true;
            SliderValue = Minimum;
            _suppressSliderCallback = false;
            HasValue = false;
        }

        UpdateValueDisplay();
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_suppressSliderCallback)
        {
            return;
        }

        int rounded = (int)Math.Round(e.NewValue);
        _suppressSliderCallback = true;
        BodyText = rounded.ToString();
        _suppressSliderCallback = false;
        HasValue = true;
        UpdateValueDisplay();

        if (_lastHapticStep != rounded)
        {
            _lastHapticStep = rounded;
            TechniqueEntryFeedback.PlayAddFeedback();
        }
    }

    private void UpdateValueDisplay()
    {
        if (!HasValue || !int.TryParse(BodyText, out int value))
        {
            ValueDisplay = string.Empty;
            AccentColor = GetThemeColor("NeutralBorder", "Gray600");
            return;
        }

        ValueDisplay = Kind == EntryFieldKind.RatingNeg10To10
            ? AppStrings.TechniqueRatingNegValue(value)
            : AppStrings.TechniqueRatingValue(value);
        AccentColor = ResolveAccentColor(value);
    }

    private Color ResolveAccentColor(int value)
    {
        double range = Maximum - Minimum;
        double normalized = range <= 0 ? 0 : (value - Minimum) / range;

        Color calm = GetThemeColor("Success", "SuccessDark");
        Color intense = GetThemeColor("Danger", "DangerDark");

        if (Kind == EntryFieldKind.RatingNeg10To10)
        {
            // -10 (worst) through 0 (neutral) to +10 (best): a diverging scale, not a one-directional one.
            return normalized < 0.5
                ? Lerp(intense, GetThemeColor("Primary", "White"), normalized * 2)
                : Lerp(GetThemeColor("Primary", "White"), calm, (normalized - 0.5) * 2);
        }

        return Lerp(calm, intense, normalized);
    }

    private static Color Lerp(Color from, Color to, double progress)
    {
        double t = Math.Clamp(progress, 0, 1);
        return Color.FromRgba(
            from.Red + (to.Red - from.Red) * t,
            from.Green + (to.Green - from.Green) * t,
            from.Blue + (to.Blue - from.Blue) * t,
            from.Alpha + (to.Alpha - from.Alpha) * t);
    }

    private static Color GetThemeColor(string lightKey, string darkKey)
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark ? darkKey : lightKey;
        return Microsoft.Maui.Controls.Application.Current?.Resources[key] is Color color ? color : Colors.Gray;
    }
}
