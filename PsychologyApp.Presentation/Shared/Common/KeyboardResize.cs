namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// The window pans by default, which only nudges the page until the cursor line shows and leaves the send button half under the keyboard.
/// A page with a message bar switches the window to resizing while it is open, so the whole bar sits just above the keyboard; the previous mode
/// comes back when the page goes. One instance per page.
/// </summary>
public sealed class KeyboardResize
{
#if ANDROID
    private Android.Views.SoftInput? _previous;
#endif

    public void Enable()
    {
#if ANDROID
        if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window is { } window)
        {
            _previous ??= window.Attributes?.SoftInputMode;
            window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
        }
#endif
    }

    public void Restore()
    {
#if ANDROID
        if (_previous is { } previous && Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window is { } window)
        {
            window.SetSoftInputMode(previous);
            _previous = null;
        }
#endif
    }
}
