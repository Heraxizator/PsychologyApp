using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class MetricTileView : ContentView
{
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
        if (bindable is MetricTileView view)
        {
            view.UpdateSemantics();
        }
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
