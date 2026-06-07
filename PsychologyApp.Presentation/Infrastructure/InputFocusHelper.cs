namespace PsychologyApp.Presentation.Infrastructure;

public static class InputFocusHelper
{
    public static void ApplyFocusedBorder(Border border)
    {
        border.Stroke = GetColor("Primary");
        border.StrokeThickness = 1.5;
    }

    public static void ApplyDefaultBorder(Border border)
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
