using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class MetricTileView : ContentView
{
    private bool _hasDisplayedValue;

    public MetricTileView()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateSemantics();
    }

    public static readonly BindableProperty ValueTextProperty =
        BindableProperty.Create(
            nameof(ValueText),
            typeof(string),
            typeof(MetricTileView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(
            nameof(LabelText),
            typeof(string),
            typeof(MetricTileView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(MetricTileView), null);

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private static void OnPresentationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not MetricTileView view)
        {
            return;
        }

        view.UpdateSemantics();

        if (oldValue is not string oldText || newValue is not string newText)
        {
            return;
        }

        view.TryPulseOnValueChange(oldText, newText);
    }

    private void TryPulseOnValueChange(string oldText, string newText)
    {
        if (!_hasDisplayedValue)
        {
            if (!string.IsNullOrWhiteSpace(newText))
            {
                _hasDisplayedValue = true;
            }

            return;
        }

        if (string.Equals(oldText, newText, StringComparison.Ordinal)
            || string.IsNullOrWhiteSpace(oldText)
            || string.IsNullOrWhiteSpace(newText))
        {
            return;
        }

        UiAnimations.SafePulseAsync(this).FireAndForget();
    }

    private void UpdateSemantics()
    {
        string description = string.IsNullOrWhiteSpace(LabelText)
            ? ValueText
            : $"{ValueText}, {LabelText}";
        SemanticProperties.SetDescription(this, description);
        SemanticProperties.SetHint(this, LabelText);
    }
}
