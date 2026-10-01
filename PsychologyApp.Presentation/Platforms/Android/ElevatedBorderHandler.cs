using Android.Graphics;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Handlers;
using AView = Android.Views.View;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// Draws a Border's Shadow as native Android elevation. MAUI renders each Shadow into a software bitmap with a
/// blur mask inside an extra wrapper view, regenerated whenever the card is laid out at a new size; with a shadow
/// on every card that was a large share of the cost of showing a list. Elevation is drawn by the GPU.
/// The style's blur radius becomes the elevation and its colour tints the shadow (Android 9+); Android's own
/// light model decides the softness, so the look is close to, not identical with, the blurred shadow.
/// </summary>
public sealed class ElevatedBorderHandler : BorderHandler
{
    // A blur radius of 10 (cards) gives 4 dp, 28 (toasts) about 11 dp.
    private const float ElevationPerBlurUnit = 0.4f;

    public static readonly IPropertyMapper<IBorderView, IBorderHandler> ElevatedMapper =
        new PropertyMapper<IBorderView, IBorderHandler>(Mapper)
        {
            [nameof(IView.Shadow)] = MapElevation
        };

    static ElevatedBorderHandler()
    {
        ElevatedMapper.AppendToMapping(nameof(IBorderStroke.Shape), (handler, _) => handler.PlatformView?.InvalidateOutline());
    }

    public ElevatedBorderHandler()
        : base(ElevatedMapper)
    {
    }

    // The shadow alone no longer needs MAUI's wrapper view; a clip still does (the app gives no Border an image background).
    public override bool NeedsContainer =>
        VirtualView?.Shadow is null
            ? base.NeedsContainer
            : VirtualView.Clip is not null;

    private static void MapElevation(IBorderHandler handler, IBorderView border)
    {
        if (handler.PlatformView is not AView view)
        {
            return;
        }

        if (border.Shadow is not { } shadow || shadow.Opacity <= 0)
        {
            view.Elevation = 0;
            view.OutlineProvider = global::Android.Views.ViewOutlineProvider.Background;
            return;
        }

        float density = view.Resources?.DisplayMetrics?.Density ?? 1f;
        view.Elevation = shadow.Radius * ElevationPerBlurUnit * density;
        view.OutlineProvider = new BorderOutlineProvider(border);

        if (OperatingSystem.IsAndroidVersionAtLeast(28) && shadow.Paint is SolidPaint { Color: { } color })
        {
            // Opaque: Android already scales shadow colours by the theme's ambient and spot alphas.
            global::Android.Graphics.Color tint = global::Android.Graphics.Color.Rgb(
                (int)(color.Red * 255),
                (int)(color.Green * 255),
                (int)(color.Blue * 255));
            view.SetOutlineAmbientShadowColor(tint);
            view.SetOutlineSpotShadowColor(tint);
        }
    }

    private sealed class BorderOutlineProvider(IBorderView border) : global::Android.Views.ViewOutlineProvider
    {
        public override void GetOutline(AView? view, Outline? outline)
        {
            if (view is null || outline is null)
            {
                return;
            }

            float density = view.Resources?.DisplayMetrics?.Density ?? 1f;
            float radius = border.Shape is RoundRectangle roundRectangle
                ? (float)roundRectangle.CornerRadius.TopLeft * density
                : 0f;
            outline.SetRoundRect(0, 0, view.Width, view.Height, radius);
            outline.Alpha = 1f;
        }
    }
}
