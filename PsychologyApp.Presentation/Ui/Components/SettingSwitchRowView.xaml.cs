using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.UI.Components;

public partial class SettingSwitchRowView : ContentView
{
    public SettingSwitchRowView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsToggled))
        {
            UiAnimations.SafePulseAsync(this).FireAndForget();
        }
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
