using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.UI.Components;

public partial class RatingEntryView : ContentView
{
    private bool _suppressSliderCallback;

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

    private static void OnKindChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RatingEntryView)bindable;
        if ((EntryFieldKind)newValue == EntryFieldKind.RatingNeg10To10)
        {
            view.Minimum = -10;
            view.Maximum = 10;
        }
        else
        {
            view.Minimum = 0;
            view.Maximum = 10;
        }

        view.SyncFromBodyText();
    }

    private static void OnBodyTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (RatingEntryView)bindable;
        if (!view._suppressSliderCallback)
        {
            view.SyncFromBodyText();
        }
    }

    private void SyncFromBodyText()
    {
        if (double.TryParse(BodyText, out double parsed))
        {
            parsed = Math.Clamp(parsed, Minimum, Maximum);
            _suppressSliderCallback = true;
            SliderValue = parsed;
            _suppressSliderCallback = false;
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
        UpdateValueDisplay();
    }

    private void UpdateValueDisplay()
    {
        if (!int.TryParse(BodyText, out int value))
        {
            ValueDisplay = string.Empty;
            return;
        }

        ValueDisplay = Kind == EntryFieldKind.RatingNeg10To10
            ? AppStrings.TechniqueRatingNegValue(value)
            : AppStrings.TechniqueRatingValue(value);
    }
}
