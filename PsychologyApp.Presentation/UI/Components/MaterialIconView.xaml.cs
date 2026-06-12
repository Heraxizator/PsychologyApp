using MauiIcons.Material;

namespace PsychologyApp.Presentation.UI.Components;

public partial class MaterialIconView : ContentView
{
    public MaterialIconView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(
            nameof(IconName),
            typeof(string),
            typeof(MaterialIconView),
            string.Empty,
            propertyChanged: OnIconNameChanged);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    private static void OnIconNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (MaterialIconView)bindable;
        view.ApplyIcon((string)newValue);
    }

    private void ApplyIcon(string iconName)
    {
        if (string.IsNullOrWhiteSpace(iconName) || !Enum.TryParse(iconName, out MaterialIcons icon))
        {
            IsVisible = false;
            return;
        }

        Icon.Icon = icon;
        IsVisible = true;
    }
}
