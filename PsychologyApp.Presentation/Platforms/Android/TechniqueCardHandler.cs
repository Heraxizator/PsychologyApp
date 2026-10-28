using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using MauiIcons.Material;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Widgets.TechniqueListCard;
using AView = Android.Views.View;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>Draws <see cref="TechniqueCardNativeView"/> with one plain Android view tree (see <see cref="TechniqueCardPlatformView"/>).</summary>
public sealed class TechniqueCardHandler : ViewHandler<TechniqueCardNativeView, TechniqueCardPlatformView>
{
    public static readonly IPropertyMapper<TechniqueCardNativeView, TechniqueCardHandler> Mapper =
        new PropertyMapper<TechniqueCardNativeView, TechniqueCardHandler>(ViewMapper)
        {
            [nameof(TechniqueCardNativeView.Title)] = (handler, view) => handler.PlatformView.SetTitle(view.Title),
            [nameof(TechniqueCardNativeView.Subtitle)] = (handler, view) => handler.PlatformView.SetSubtitle(view.Subtitle),
            [nameof(TechniqueCardNativeView.MetaText)] = (handler, view) => handler.PlatformView.SetMeta(view.MetaText),
            [nameof(TechniqueCardNativeView.Date)] = (handler, view) => handler.PlatformView.SetDate(view.Date),
            [nameof(TechniqueCardNativeView.Author)] = (handler, view) => handler.PlatformView.SetAuthor(view.Author),
            [nameof(TechniqueCardNativeView.IconName)] = MapPicture,
            [nameof(TechniqueCardNativeView.Image)] = MapPicture,
            [nameof(TechniqueCardNativeView.Flavor)] = (handler, view) => handler.PlatformView.SetFlavor(view.Flavor)
        };

    public TechniqueCardHandler() : base(Mapper)
    {
    }

    protected override TechniqueCardPlatformView CreatePlatformView() =>
        new(Context, MauiContext!.Services.GetRequiredService<IFontManager>());

    protected override void ConnectHandler(TechniqueCardPlatformView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Click += OnClick;
        if (Microsoft.Maui.Controls.Application.Current is { } app)
        {
            app.RequestedThemeChanged += OnThemeChanged;
        }
    }

    protected override void DisconnectHandler(TechniqueCardPlatformView platformView)
    {
        platformView.Click -= OnClick;
        if (Microsoft.Maui.Controls.Application.Current is { } app)
        {
            app.RequestedThemeChanged -= OnThemeChanged;
        }

        base.DisconnectHandler(platformView);
    }

    private void OnClick(object? sender, EventArgs e)
    {
        if (VirtualView?.TapCommand is { } command && command.CanExecute(null))
        {
            command.Execute(null);
        }
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => PlatformView?.ApplyAppearance();

    private static void MapPicture(TechniqueCardHandler handler, TechniqueCardNativeView view) =>
        handler.PlatformView.SetPicture(view.IconName, view.Image);
}

/// <summary>
/// The practice card as plain Android views: a rounded surface with a tile (icon or picture), the title and subtitle, a chevron, the
/// group line and the date and author line. Colours, fonts and sizes come from the app resources, so the theme, the accent colour, the
/// text size and the bold-text setting look as in the XAML card; they are applied again whenever the card is attached to a window,
/// which is how a recycled card picks up a setting changed meanwhile.
/// </summary>
public sealed class TechniqueCardPlatformView : LinearLayout
{
    private const float TileDp = 52;
    private const float TileCornerDp = 12;
    private const float IconDp = 28;
    private const float PictureDp = 40;

    private static readonly ConcurrentDictionary<string, string?> Glyphs = new(StringComparer.Ordinal);

    private readonly IFontManager _fonts;
    private readonly FrameLayout _tile;
    private readonly TextView _icon;
    private readonly TextView _title;
    private readonly TextView _subtitle;
    private readonly TextView _chevron;
    private readonly TextView _meta;
    private readonly TextView _date;
    private readonly TextView _author;
    private readonly LinearLayout _status;
    private readonly GradientDrawable _surface = new();
    private readonly GradientDrawable _tileBackground = new();
    private ImageView? _picture;
    private string _flavor = string.Empty;

    public TechniqueCardPlatformView(Context context, IFontManager fonts) : base(context)
    {
        _fonts = fonts;
        Orientation = global::Android.Widget.Orientation.Vertical;
        SetPadding(Px(16), Px(14), Px(16), Px(14));
        Background = _surface;
        Elevation = Px(2);
        if (OperatingSystem.IsAndroidVersionAtLeast(28))
        {
            // As soft as the cards drawn by MAUI (BrandCardShadowLight); the system default shadow is a hard grey edge.
            SetOutlineSpotShadowColor(global::Android.Graphics.Color.Argb(48, 21, 34, 56));
            SetOutlineAmbientShadowColor(global::Android.Graphics.Color.Argb(24, 21, 34, 56));
        }
        // The whole card is one item for screen readers: its description is the title (SemanticProperties on the MAUI view).
        DescendantFocusability = DescendantFocusability.BlockDescendants;
        // The card takes the tap itself (see TechniqueCardHandler.OnClick); clickable also makes TalkBack announce it as a button.
        Clickable = true;

        LinearLayout top = new(context) { Orientation = global::Android.Widget.Orientation.Horizontal };
        top.SetGravity(GravityFlags.CenterVertical);
        _tile = new FrameLayout(context) { Background = _tileBackground };
        _icon = Text(context, maxLines: 1);
        _icon.Gravity = GravityFlags.Center;
        _tile.AddView(_icon, new FrameLayout.LayoutParams(Px(IconDp), Px(IconDp), GravityFlags.Center));
        // The tile sits at the top of the text, as on the test cards, rather than floating in the middle of a tall card.
        top.AddView(_tile, new LayoutParams(Px(TileDp), Px(TileDp)) { Gravity = GravityFlags.Top });

        // Everything the card says is in one column with one left edge: title, a one-line subtitle, the length and group in grey,
        // and when the practice was last done, only once it was. Five lines with a blue one in each card made the list loud.
        LinearLayout texts = new(context) { Orientation = global::Android.Widget.Orientation.Vertical };
        _title = Text(context, maxLines: 2);
        _subtitle = Text(context, maxLines: 1);
        _meta = Text(context, maxLines: 1);
        texts.AddView(_title);
        texts.AddView(_subtitle, Margins(top: 2));
        texts.AddView(_meta, Margins(top: 4));

        LinearLayout status = new(context) { Orientation = global::Android.Widget.Orientation.Horizontal, Visibility = ViewStates.Gone };
        _status = status;
        _date = Text(context, maxLines: 1);
        _author = Text(context, maxLines: 1);
        _author.Gravity = GravityFlags.End;
        status.AddView(_date, new LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f));
        LayoutParams authorLayout = new(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
        authorLayout.SetMargins(Px(12), 0, 0, 0);
        status.AddView(_author, authorLayout);
        texts.AddView(status, Margins(top: 2));

        LayoutParams textsLayout = new(0, ViewGroup.LayoutParams.WrapContent, 1f);
        textsLayout.SetMargins(Px(12), 0, Px(12), 0);
        top.AddView(texts, textsLayout);

        _chevron = Text(context, maxLines: 1);
        top.AddView(_chevron);
        AddView(top, new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

        _chevron.Text = Glyph("KeyboardArrowRight");
        ApplyAppearance();
    }

    public void SetTitle(string text) => _title.Text = text;

    public void SetSubtitle(string text) => _subtitle.Text = text;

    public void SetMeta(string text) => _meta.Text = text;

    public void SetDate(string text)
    {
        _date.Text = text;
        _status.Visibility = string.IsNullOrWhiteSpace(text) ? ViewStates.Gone : ViewStates.Visible;
    }

    public void SetAuthor(string text)
    {
        _author.Text = text;
        _author.Visibility = string.IsNullOrWhiteSpace(text) ? ViewStates.Gone : ViewStates.Visible;
    }

    public void SetFlavor(string flavor)
    {
        _flavor = flavor;
        ApplyTileColours();
    }

    /// <summary>An icon when there is one; otherwise the picture of a practice the person made (its ImageView exists only then).</summary>
    public void SetPicture(string iconName, string image)
    {
        bool hasIcon = !string.IsNullOrWhiteSpace(iconName);
        _icon.Text = hasIcon ? Glyph(iconName) : string.Empty;
        _icon.Visibility = hasIcon ? ViewStates.Visible : ViewStates.Gone;

        Bitmap? bitmap = hasIcon ? null : Decode(image, Px(PictureDp));
        if (bitmap is null)
        {
            _picture?.SetImageDrawable(null);
            if (_picture is not null)
            {
                _picture.Visibility = ViewStates.Gone;
            }

            return;
        }

        if (_picture is null)
        {
            _picture = new ImageView(Context);
            _picture.SetScaleType(ImageView.ScaleType.FitCenter!);
            _tile.AddView(_picture, new FrameLayout.LayoutParams(Px(PictureDp), Px(PictureDp), GravityFlags.Center));
        }

        _picture.SetImageBitmap(bitmap);
        _picture.Visibility = ViewStates.Visible;
    }

    /// <summary>Colours, fonts and sizes from the current theme and settings.</summary>
    public void ApplyAppearance()
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        _surface.SetColor(Resource(dark ? "SurfaceElevatedDark" : "SurfaceElevatedLight", Colors.White).ToPlatform());
        _surface.SetCornerRadius(Px((float)Number("UiButtonCornerRadius", 8)));

        double body = Number("BodyFontSize", 16);
        double caption = Number("CaptionFontSize", 14);
        string bodyFamily = Resource<string>("BodyFontFamily") ?? "InterRegular";
        const string semiBold = "InterSemiBold";

        Style(_title, semiBold, body, Number("BodyLineHeight", 1.45), dark ? "TextPrimaryDark" : "TextPrimaryLight");
        Style(_subtitle, bodyFamily, caption, Number("CaptionLineHeight", 1.35), dark ? "TextSecondaryDark" : "TextSecondaryLight");
        Style(_meta, bodyFamily, caption, Number("CaptionLineHeight", 1.35), dark ? "TextSecondaryDark" : "TextSecondaryLight");
        Style(_date, bodyFamily, caption, Number("CaptionLineHeight", 1.35), dark ? "TextSecondaryDark" : "TextSecondaryLight");
        Style(_author, bodyFamily, caption, Number("CaptionLineHeight", 1.35), dark ? "TextSecondaryDark" : "TextSecondaryLight");
        Style(_chevron, "MaterialIcons", 14, 1, dark ? "TextSecondaryDark" : "TextTertiaryLight");
        _icon.Typeface = _fonts.GetTypeface(Microsoft.Maui.Font.OfSize("MaterialIcons", 24));
        _icon.SetTextSize(ComplexUnitType.Sp, 24);
        ApplyTileColours();
    }

    /// <summary>
    /// The card always takes the whole width it is offered. MAUI measures it with an "at most" width and later stretches it without
    /// measuring the rows again, so with short texts (in Chinese, say) the chevron and the author stayed in the middle of the card.
    /// </summary>
    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        if (MeasureSpec.GetMode(widthMeasureSpec) == MeasureSpecMode.AtMost)
        {
            widthMeasureSpec = MeasureSpec.MakeMeasureSpec(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpecMode.Exactly);
        }

        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
    }

    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
        ApplyAppearance();
    }

    /// <summary>
    /// The press feedback of the app's other cards (VisualElementPressFeedback: scale 0.94, fade to 0.9, 1 dp down; 120 ms in, 200 ms
    /// out). Android sets the pressed state itself and clears it when the finger starts scrolling, so a scroll never looks like a tap.
    /// </summary>
    protected override void DispatchSetPressed(bool pressed)
    {
        base.DispatchSetPressed(pressed);
        if (PsychologyApp.Presentation.Shared.Common.ReduceMotion.IsEnabled)
        {
            return;
        }

        Animate()!
            .ScaleX(pressed ? 0.94f : 1f)
            .ScaleY(pressed ? 0.94f : 1f)
            .Alpha(pressed ? 0.9f : 1f)
            .TranslationY(pressed ? Px(1) : 0)
            .SetDuration(pressed ? 120 : 200)
            .Start();
    }

    private void ApplyTileColours()
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        _tileBackground.SetCornerRadius(Px(TileCornerDp));
        if (TechniqueFlavors.Colors(TechniqueFlavors.Parse(_flavor), dark) is { } colours)
        {
            _tileBackground.SetColor(MauiColor.FromArgb(colours.Tile).ToPlatform());
            _icon.SetTextColor(MauiColor.FromArgb(colours.Icon).ToPlatform());
            return;
        }

        _tileBackground.SetColor(Resource(dark ? "PrimaryTintDark" : "PrimaryTint", Colors.LightBlue).ToPlatform());
        _icon.SetTextColor(Resource("Primary", Colors.RoyalBlue).ToPlatform());
    }

    private void Style(TextView view, string family, double size, double lineHeight, string colourKey)
    {
        view.Typeface = _fonts.GetTypeface(Microsoft.Maui.Font.OfSize(family, size));
        // Labels in MAUI size their text in sp, so the system font scale applies here too.
        view.SetTextSize(ComplexUnitType.Sp, (float)size);
        view.SetLineSpacing(0, (float)lineHeight);
        view.SetTextColor(Resource(colourKey, Colors.Black).ToPlatform());
    }

    private static TextView Text(Context context, int maxLines)
    {
        TextView view = new(context) { ImportantForAccessibility = ImportantForAccessibility.No };
        view.SetMaxLines(maxLines);
        view.Ellipsize = TextUtils.TruncateAt.End;
        return view;
    }

    private LayoutParams Margins(int top)
    {
        LayoutParams layout = new(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        layout.SetMargins(0, Px(top), 0, 0);
        return layout;
    }

    private int Px(float dp) => (int)Math.Round(Context!.ToPixels(dp));

    // The icon library keeps each glyph in the enum member's [Description], as TabIcons reads it.
    private static string Glyph(string name) =>
        Glyphs.GetOrAdd(name, key => typeof(MaterialIcons).GetField(key, BindingFlags.Public | BindingFlags.Static)
            ?.GetCustomAttribute<DescriptionAttribute>()?.Description) ?? string.Empty;

    /// <summary>A picture file scaled down to about the tile size while decoding, so a phone photo never sits in memory full size.</summary>
    private static Bitmap? Decode(string? path, int sizePx)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        BitmapFactory.Options bounds = new() { InJustDecodeBounds = true };
        BitmapFactory.DecodeFile(path, bounds);
        int sample = 1;
        while (bounds.OutWidth / (sample * 2) >= sizePx && bounds.OutHeight / (sample * 2) >= sizePx)
        {
            sample *= 2;
        }

        return BitmapFactory.DecodeFile(path, new BitmapFactory.Options { InSampleSize = sample });
    }

    private static MauiColor Resource(string key, MauiColor fallback) => Resource<MauiColor>(key) ?? fallback;

    private static double Number(string key, double fallback) =>
        Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true && value is double number ? number : fallback;

    private static T? Resource<T>(string key) where T : class =>
        Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true ? value as T : null;
}
