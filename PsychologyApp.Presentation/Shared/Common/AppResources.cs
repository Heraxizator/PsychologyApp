namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Reads app-level resources from code. A missing key gives the caller's fallback instead of the
/// <see cref="KeyNotFoundException"/> the resource indexer throws, so a renamed style cannot crash a screen.
/// </summary>
public static class AppResources
{
    public static T? Find<T>(string key) where T : class =>
        Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true ? value as T : null;

    public static Color Color(string key, Color fallback) => Find<Color>(key) ?? fallback;

    /// <summary>The colour for the theme the app is drawn in now.</summary>
    public static Color ThemedColor(string lightKey, string darkKey, Color fallback) =>
        Color(Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark ? darkKey : lightKey, fallback);
}
