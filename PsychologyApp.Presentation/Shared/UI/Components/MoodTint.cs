using PsychologyApp.Presentation.Core.Charts;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// Paints an element in the colour of a mood level (<see cref="MoodPalette"/>), for the light and the dark theme at once, so the
/// colour follows a theme switch like an AppThemeBinding; a converter would keep the theme it was first evaluated in. No level
/// gives the plain surface.
/// </summary>
public static class MoodTint
{
    public static readonly BindableProperty LevelProperty =
        BindableProperty.CreateAttached("Level", typeof(int?), typeof(MoodTint), null, propertyChanged: OnLevelChanged);

    public static int? GetLevel(BindableObject view) => (int?)view.GetValue(LevelProperty);

    public static void SetLevel(BindableObject view, int? value) => view.SetValue(LevelProperty, value);

    /// <summary>A small mark of a day's mood (the dot under a date in the week), in the stronger shade a mark that size needs.</summary>
    public static readonly BindableProperty DotLevelProperty =
        BindableProperty.CreateAttached("DotLevel", typeof(int?), typeof(MoodTint), null, propertyChanged: OnDotLevelChanged);

    public static int? GetDotLevel(BindableObject view) => (int?)view.GetValue(DotLevelProperty);

    public static void SetDotLevel(BindableObject view, int? value) => view.SetValue(DotLevelProperty, value);

    private static void OnDotLevelChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is VisualElement element && newValue is int level)
        {
            element.SetAppThemeColor(
                VisualElement.BackgroundColorProperty,
                Color.FromArgb(MoodPalette.Dot(level, dark: false)),
                Color.FromArgb(MoodPalette.Dot(level, dark: true)));
        }
    }

    /// <summary>The face on a mood tile: the shade of the tile's own hue, as the icons on the test and practice tiles.</summary>
    public static readonly BindableProperty IconLevelProperty =
        BindableProperty.CreateAttached("IconLevel", typeof(int), typeof(MoodTint), 0, propertyChanged: OnIconLevelChanged);

    public static int GetIconLevel(BindableObject view) => (int)view.GetValue(IconLevelProperty);

    public static void SetIconLevel(BindableObject view, int value) => view.SetValue(IconLevelProperty, value);

    private static void OnIconLevelChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is MaterialIconView icon && newValue is int level and >= 1)
        {
            icon.SetAppThemeColor(
                MaterialIconView.IconColorProperty,
                Color.FromArgb(MoodPalette.Icon(level, dark: false)),
                Color.FromArgb(MoodPalette.Icon(level, dark: true)));
        }
    }

    private static void OnLevelChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not VisualElement element)
        {
            return;
        }

        int? level = newValue as int?;
        element.SetAppThemeColor(
            VisualElement.BackgroundColorProperty,
            Color.FromArgb(MoodPalette.Fill(level, dark: false)),
            Color.FromArgb(MoodPalette.Fill(level, dark: true)));
    }
}
