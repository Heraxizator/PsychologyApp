using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class SettingSwitchRowView : ContentView
{
    public SettingSwitchRowView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(SettingSwitchRowView), string.Empty);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SettingSwitchRowView), false, BindingMode.TwoWay);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }
}
