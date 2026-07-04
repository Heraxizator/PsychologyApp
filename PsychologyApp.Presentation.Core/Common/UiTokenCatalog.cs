namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Canonical UI token and style keys. Keep in sync with Resources/Styles/*.xaml.
/// </summary>
public static class UiTokenCatalog
{
    public static IReadOnlyList<string> RequiredColorKeys { get; } =
    [
        "Primary",
        "PrimaryTint",
        "SurfaceHeroLight",
        "SurfaceHeroDark",
        "BorderSubtleLight",
        "BorderSubtleDark",
        "OverlayScrim",
        "DisabledSurfaceLight",
        "DisabledSurfaceDark",
    ];

    public static IReadOnlyList<string> RequiredTypographyKeys { get; } =
    [
        "QuoteDisplayFontSize",
        "QuoteHeroFontSize",
        "DisabledOpacity",
        "HeroAccentBarWidth",
    ];

    public static IReadOnlyList<string> RequiredStyleKeys { get; } =
    [
        "BrandCardStyle",
        "ListCardItemStyle",
        "HeroCardStyle",
        "HeroQuoteStyle",
        "HeroCaptionStyle",
        "HeroAccentBarStyle",
        "MetaPillStyle",
        "EmptyStateActionPillStyle",
        "SubtleDividerStyle",
        "PrimaryActionBorderStyle",
        "FilterChipSelectedStyle",
    ];
}
