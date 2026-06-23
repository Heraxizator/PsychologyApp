using MauiIcons.Material;

namespace PsychologyApp.Presentation.Shared.UI.Components;

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

    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(
            nameof(IconColor),
            typeof(Color),
            typeof(MaterialIconView),
            null);

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(
            nameof(IconSize),
            typeof(double),
            typeof(MaterialIconView),
            24d);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
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
