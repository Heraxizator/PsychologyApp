using CommunityToolkit.Maui;

namespace PsychologyApp.Presentation.Shared.UI.Overlays;

internal static class AppPopupOptions
{
    private static readonly Color DialogOverlay = Color.FromArgb("#66000000");

    public static PopupOptions Dialog { get; } = new()
    {
        PageOverlayColor = DialogOverlay,
        CanBeDismissedByTappingOutsideOfPopup = true,
        Shape = null,
        Shadow = null,
    };

    public static PopupOptions Toast { get; } = new()
    {
        PageOverlayColor = Colors.Transparent,
        CanBeDismissedByTappingOutsideOfPopup = false,
        Shape = null,
        Shadow = null,
    };

    public static PopupOptions OptionsSheet { get; } = new()
    {
        PageOverlayColor = DialogOverlay,
        CanBeDismissedByTappingOutsideOfPopup = true,
        Shape = null,
        Shadow = null,
    };
}
