namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class SettingSwitchRowView : ContentView
{
    public SettingSwitchRowView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(IsRowEnabled))
        {
            RowOpacity = IsRowEnabled ? 1d : (double)Microsoft.Maui.Controls.Application.Current!.Resources["DisabledOpacity"];
        }
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(SettingSwitchRowView), string.Empty);

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SettingSwitchRowView), false, BindingMode.TwoWay);

    public static readonly BindableProperty ShowDividerProperty =
        BindableProperty.Create(nameof(ShowDivider), typeof(bool), typeof(SettingSwitchRowView), true);

    public static readonly BindableProperty IsRowEnabledProperty =
        BindableProperty.Create(nameof(IsRowEnabled), typeof(bool), typeof(SettingSwitchRowView), true);

    public static readonly BindableProperty RowOpacityProperty =
        BindableProperty.Create(nameof(RowOpacity), typeof(double), typeof(SettingSwitchRowView), 1d);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public bool ShowDivider
    {
        get => (bool)GetValue(ShowDividerProperty);
        set => SetValue(ShowDividerProperty, value);
    }

    public bool IsRowEnabled
    {
        get => (bool)GetValue(IsRowEnabledProperty);
        set => SetValue(IsRowEnabledProperty, value);
    }

    public double RowOpacity
    {
        get => (double)GetValue(RowOpacityProperty);
        private set => SetValue(RowOpacityProperty, value);
    }
}
