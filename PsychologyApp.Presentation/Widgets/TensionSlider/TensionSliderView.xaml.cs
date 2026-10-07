using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TensionSlider;

/// <summary>
/// The 0..10 tension question as one bar to slide along instead of eleven buttons: the number grows and changes colour from calm to strong
/// under the finger, a light tap marks each step, and after a practice it shows "was 8 → now 5" as the value moves.
/// </summary>
public partial class TensionSliderView : ContentView
{
    public static readonly BindableProperty ReferenceValueProperty = BindableProperty.Create(
        nameof(ReferenceValue), typeof(int?), typeof(TensionSliderView), null, propertyChanged: (b, _, _) => ((TensionSliderView)b).Refresh());

    private int _value = -1;

    public TensionSliderView()
    {
        InitializeComponent();
        HintLabel.Text = AppStrings.TensionPickHint;
        ChooseButton.BodyText = AppStrings.TensionPickConfirm;
        ChooseButton.TapCommand = new Command(() => Chosen?.Invoke(this, _value));
        Show(5, announce: false);
    }

    /// <summary>The value picked the last time, shown as "was N" next to the current one; null on the first rating of a chat.</summary>
    public int? ReferenceValue
    {
        get => (int?)GetValue(ReferenceValueProperty);
        set => SetValue(ReferenceValueProperty, value);
    }

    /// <summary>The person confirmed a value.</summary>
    public event EventHandler<int>? Chosen;

    private void OnValueChanged(object? sender, ValueChangedEventArgs e)
    {
        int snapped = TensionScale.Snap(e.NewValue);
        if (Math.Abs(TensionBar.Value - snapped) > 0.001)
        {
            // Stay on whole steps: the thumb jumps to the nearest one, which also calls this handler again with the snapped value.
            TensionBar.Value = snapped;
            return;
        }

        Show(snapped, announce: true);
    }

    private void Refresh() => Show(_value < 0 ? 5 : _value, announce: false);

    private void Show(int value, bool announce)
    {
        bool changed = value != _value;
        _value = value;

        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        Color color = Color.FromArgb(TensionScale.HexAt(value, dark));
        NumberLabel.Text = value.ToString();
        NumberLabel.TextColor = color;
        TensionBar.ThumbColor = color;
        TensionBar.MinimumTrackColor = color;
        WordLabel.Text = TensionScale.WordAt(value) switch
        {
            TensionWord.Calm => AppStrings.TensionCalmWord,
            TensionWord.Strong => AppStrings.TensionStrongWord,
            _ => string.Empty
        };

        ChangeLabel.IsVisible = ReferenceValue is not null;
        if (ReferenceValue is { } before)
        {
            ChangeLabel.Text = string.Format(AppStrings.TensionChangeFormat, before, value);
        }

        SemanticProperties.SetDescription(TensionBar, value.ToString());

        if (announce && changed)
        {
            UiHaptics.Tick();
            if (UiAnimations.ShouldAnimate(NumberLabel))
            {
                NumberLabel.Scale = 1.18;
                NumberLabel.ScaleToAsync(1, 140, Easing.CubicOut).FireAndForget();
            }
        }
    }
}
