using PsychologyApp.Presentation.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class ButtonView : ContentView
{
    public ButtonView()
    {
        InitializeComponent();
        TemplatePressFeedback.Attach(this);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(ButtonView), string.Empty, BindingMode.TwoWay);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ButtonView), default, BindingMode.TwoWay);

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
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
