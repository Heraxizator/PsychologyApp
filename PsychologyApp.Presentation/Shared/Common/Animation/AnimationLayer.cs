namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Puts a view on a GPU layer while its opacity animates. On Android a view with alpha below 1 is otherwise
/// redrawn into an offscreen buffer on every frame, which is what made reveals of whole lists drop frames.
/// </summary>
internal static class AnimationLayer
{
    public static void Enter(VisualElement view)
    {
#if ANDROID
        if (NativeView(view) is { LayerType: Android.Views.LayerType.None } native)
        {
            native.SetLayerType(Android.Views.LayerType.Hardware, null);
        }
#endif
    }

    public static void Exit(VisualElement view)
    {
#if ANDROID
        if (NativeView(view) is { LayerType: Android.Views.LayerType.Hardware } native)
        {
            native.SetLayerType(Android.Views.LayerType.None, null);
        }
#endif
    }

    /// <summary>FadeTo on a GPU layer; the layer is dropped once the view is fully shown or hidden.</summary>
    public static async Task<bool> FadeLayeredAsync(this VisualElement view, double opacity, uint length, Easing? easing = null)
    {
        Enter(view);
        bool finished = await view.FadeToAsync(opacity, length, easing);
        if (view.Opacity is 0 or 1)
        {
            Exit(view);
        }

        return finished;
    }

#if ANDROID
    // The container (when a shadow or clip wraps the view) is what Android actually fades.
    private static Android.Views.View? NativeView(VisualElement view) =>
        view.Handler is IViewHandler handler
            ? (handler.ContainerView ?? handler.PlatformView) as Android.Views.View
            : null;
#endif
}
