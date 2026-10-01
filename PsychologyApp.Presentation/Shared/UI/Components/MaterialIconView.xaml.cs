using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using MauiIcons.Material;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// A Material icon drawn as one glyph Label. The library's MauiIcon wraps its glyph in a ContentView, a stack
/// layout and a second label; inside list rows that was five native views per icon.
/// </summary>
public sealed class MaterialIconView : Label
{
    // Registered by UseMaterialMauiIcons().
    private const string FontAlias = "MaterialIcons";

    private static readonly ConcurrentDictionary<string, string?> Glyphs = new(StringComparer.Ordinal);

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(
            nameof(IconName),
            typeof(string),
            typeof(MaterialIconView),
            string.Empty,
            propertyChanged: (bindable, _, newValue) => ((MaterialIconView)bindable).ApplyIcon((string?)newValue));

    public static readonly BindableProperty IconColorProperty =
        BindableProperty.Create(
            nameof(IconColor),
            typeof(Color),
            typeof(MaterialIconView),
            null,
            propertyChanged: (bindable, _, newValue) => ((MaterialIconView)bindable).ApplyColor((Color?)newValue));

    public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(
            nameof(IconSize),
            typeof(double),
            typeof(MaterialIconView),
            24d,
            propertyChanged: (bindable, _, newValue) => ((MaterialIconView)bindable).FontSize = (double)newValue);

    public static readonly BindableProperty HasResolvedIconProperty =
        BindableProperty.Create(
            nameof(HasResolvedIcon),
            typeof(bool),
            typeof(MaterialIconView),
            false);

    public MaterialIconView()
    {
        FontFamily = FontAlias;
        FontSize = IconSize;
        HorizontalTextAlignment = TextAlignment.Center;
        VerticalTextAlignment = TextAlignment.Center;
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;
        ApplyColor(null);
        IsVisible = false;
    }

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

    public bool HasResolvedIcon
    {
        get => (bool)GetValue(HasResolvedIconProperty);
        private set => SetValue(HasResolvedIconProperty, value);
    }

    private void ApplyIcon(string? iconName)
    {
        string? glyph = string.IsNullOrWhiteSpace(iconName) ? null : Glyphs.GetOrAdd(iconName, ResolveGlyph);
        Text = glyph;
        HasResolvedIcon = glyph is not null;
        IsVisible = glyph is not null;
    }

    private void ApplyColor(Color? color)
    {
        if (color is null)
        {
            SetDynamicResource(TextColorProperty, "Primary");
            return;
        }

        TextColor = color;
    }

    // The library stores each glyph in the enum member's [Description]; read once per icon name, then cached.
    private static string? ResolveGlyph(string iconName)
    {
        if (!MaterialIconNames.TryResolve(iconName, out MaterialIcons icon))
        {
            return null;
        }

        return typeof(MaterialIcons)
            .GetField(icon.ToString(), BindingFlags.Public | BindingFlags.Static)?
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description;
    }
}
