namespace PsychologyApp.Presentation.Shared.Common;

public static class InputFocusHelper
{
    public static void ApplyFocusedBorder(Border border) =>
        ApplyFocusedBorderInstant(border);

    public static void ApplyDefaultBorder(Border border) =>
        ApplyDefaultBorderInstant(border);

    public static Task ApplyFocusedBorderAsync(Border border) =>
        UiAnimations.SafeInputFocusAsync(border, focused: true);

    public static Task ApplyDefaultBorderAsync(Border border) =>
        UiAnimations.SafeInputFocusAsync(border, focused: false);

    internal static void ApplyFocusedBorderInstant(Border border)
    {
        border.Stroke = GetColor("Primary");
        border.StrokeThickness = 1.5;
    }

    internal static void ApplyDefaultBorderInstant(Border border)
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark
            ? "InputBorderDark"
            : "NeutralBorder";
        border.Stroke = GetColor(key);
        border.StrokeThickness = 1;
    }

    private static Color GetColor(string key) =>
        Microsoft.Maui.Controls.Application.Current?.Resources[key] is Color color
            ? color
            : Colors.Transparent;
}
