using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class ButtonView : ContentView
{
    public ButtonView()
    {
        InitializeComponent();
        TemplatePressFeedback.Attach(this, new PressFeedbackOptions
        {
            PressScale = () => Variant == "Secondary"
                ? UiAnimations.PressScaleSecondary
                : UiAnimations.PressScalePrimary,
            HapticOnRelease = true
        });
        UpdateSemantics();
    }

    private static void OnAccessibilityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ButtonView view)
        {
            view.UpdateSemantics();
        }
    }

    private void UpdateSemantics()
    {
        string label = string.IsNullOrWhiteSpace(AccessibilityLabel) ? BodyText : AccessibilityLabel;
        SemanticProperties.SetDescription(this, label);
        SemanticProperties.SetHint(this, AccessibilityHint);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(
            nameof(BodyText),
            typeof(string),
            typeof(ButtonView),
            string.Empty,
            BindingMode.TwoWay,
            propertyChanged: OnAccessibilityChanged);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public static readonly BindableProperty AccessibilityHintProperty =
        BindableProperty.Create(
            nameof(AccessibilityHint),
            typeof(string),
            typeof(ButtonView),
            string.Empty,
            propertyChanged: OnAccessibilityChanged);

    public string AccessibilityHint
    {
        get => (string)GetValue(AccessibilityHintProperty);
        set => SetValue(AccessibilityHintProperty, value);
    }

    public static readonly BindableProperty AccessibilityLabelProperty =
        BindableProperty.Create(
            nameof(AccessibilityLabel),
            typeof(string),
            typeof(ButtonView),
            string.Empty,
            propertyChanged: OnAccessibilityChanged);

    public string AccessibilityLabel
    {
        get => (string)GetValue(AccessibilityLabelProperty);
        set => SetValue(AccessibilityLabelProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ButtonView), default, BindingMode.TwoWay);

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ButtonView));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(string), typeof(ButtonView), "Primary");

    public string Variant
    {
        get => (string)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public static readonly BindableProperty IsCompactProperty =
        BindableProperty.Create(nameof(IsCompact), typeof(bool), typeof(ButtonView), false);

    public bool IsCompact
    {
        get => (bool)GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }
}
