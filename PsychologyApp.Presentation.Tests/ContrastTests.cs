using System.Text.RegularExpressions;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Presentation.Tests;

/// <summary>Colours and text sizes that carry meaning stay readable in the light and the dark theme (WCAG contrast; nothing under 12 points).</summary>
public class ContrastTests(ITestOutputHelper output)
{
    private const string LightSurface = "#FFFFFF";
    private const string LightPage = "#F3F4F6";
    private const string DarkSurface = "#1E1E1E";
    private const string DarkPage = "#121212";

    [Fact]
    public void TheRatioIsTheWcagOne()
    {
        Assert.Equal(21, ColorContrast.Ratio("#000000", "#FFFFFF"), 1);
        Assert.Equal(1, ColorContrast.Ratio("#777777", "#777777"), 3);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheDayNumberIsReadableOnEveryMoodColour(bool dark)
    {
        foreach (int level in new[] { 1, 2, 3, 4, 5 })
        {
            double ratio = ColorContrast.Ratio(MoodPalette.Text(dark), MoodPalette.Fill(level, dark));
            Assert.True(ratio >= ColorContrast.BodyTextMinimum, $"mood {level}, dark={dark}: {ratio:F2}");
        }

        Assert.True(ColorContrast.Ratio(MoodPalette.Text(dark), MoodPalette.Fill(null, dark)) >= ColorContrast.BodyTextMinimum);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryMoodHasItsOwnColourAndAMoodColourIsNeverTheEmptyDay(bool dark)
    {
        string[] fills = [.. Enumerable.Range(1, 5).Select(l => MoodPalette.Fill(l, dark))];

        Assert.Equal(5, fills.Distinct().Count());
        Assert.DoesNotContain(MoodPalette.Fill(null, dark), fills);
    }

    [Theory]
    [InlineData(false, LightSurface, LightPage)]
    [InlineData(true, DarkSurface, DarkPage)]
    public void TheTensionNumberIsReadableAsLargeTextAtEveryStep(bool dark, string surface, string page)
    {
        for (int value = TensionScale.Min; value <= TensionScale.Max; value++)
        {
            string hex = TensionScale.HexAt(value, dark);
            Assert.True(ColorContrast.Ratio(hex, surface) >= ColorContrast.LargeTextMinimum, $"step {value} on surface, dark={dark}: {ColorContrast.Ratio(hex, surface):F2}");
            Assert.True(ColorContrast.Ratio(hex, page) >= ColorContrast.LargeTextMinimum, $"step {value} on page, dark={dark}: {ColorContrast.Ratio(hex, page):F2}");
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheBreathingWordsAreReadableOnTheCircleAtEveryMomentOfTheExercise(bool dark)
    {
        for (double calm = 0; calm <= 1.0001; calm += 0.1)
        {
            foreach (double scale in new[] { BreathingPattern.SmallScale, 0.75, BreathingPattern.LargeScale })
            {
                string seen = BreathingColors.Composite(BreathingColors.At(calm, dark), BreathingColors.AlphaAt(scale), dark);
                double ratio = ColorContrast.Ratio(BreathingColors.TextHex(dark), seen);
                Assert.True(ratio >= ColorContrast.BodyTextMinimum, $"calm {calm:F1}, scale {scale:F2}, dark={dark}: {ratio:F2}");
            }
        }
    }

    [Fact]
    public void TextSizesNeverGoUnderTwelveAndGrowWithTheSetting()
    {
        foreach (string key in new[] { "small", "medium", "large" })
        {
            (double pageTitle, double section, double body, double caption) = TypographyScale.For(key);
            Assert.True(caption >= TypographyScale.MinimumReadable, key);
            Assert.True(body >= caption && section >= body && pageTitle >= section, key);
        }

        Assert.True(TypographyScale.For("large").Body > TypographyScale.For("medium").Body);
        Assert.True(TypographyScale.For("medium").Body > TypographyScale.For("small").Body);
    }

    [Fact]
    public void TheDefaultTokensInTheStylesAreTheMediumSizes()
    {
        string typography = File.ReadAllText(StylePath("Typography.xaml"));
        (_, _, double body, double caption) = TypographyScale.For("medium");

        Assert.Contains($"x:Key=\"BodyFontSize\">{body}<", typography);
        Assert.Contains($"x:Key=\"CaptionFontSize\">{caption}<", typography);
    }

    [Fact]
    public void TheTextColoursOfTheThemesAreReadableOnTheirSurfaces()
    {
        Dictionary<string, string> colors = ReadColors();
        (string Text, string Background)[] pairs =
        [
            ("TextPrimaryLight", "SurfaceLight"), ("TextPrimaryLight", "PageBackground"),
            ("TextSecondaryLight", "SurfaceLight"), ("TextSecondaryLight", "PageBackground"),
            ("TextPrimaryDark", "SurfaceDark"), ("TextPrimaryDark", "PageBackgroundDark"),
            ("TextSecondaryDark", "SurfaceDark"), ("TextSecondaryDark", "PageBackgroundDark")
        ];

        foreach ((string text, string background) in pairs)
        {
            double ratio = ColorContrast.Ratio(colors[text], colors[background]);
            output.WriteLine($"{text} on {background}: {ratio:F2}");
            Assert.True(ratio >= ColorContrast.BodyTextMinimum, $"{text} on {background}: {ratio:F2}");
        }
    }

    [Fact]
    public void TheAccentIsReadableAsTextOnTheSurfaces()
    {
        Dictionary<string, string> colors = ReadColors();
        (string Accent, string Background)[] pairs =
        [
            ("PrimaryTextLight", "SurfaceLight"), ("PrimaryTextLight", "PageBackground"),
            ("PrimaryTextDark", "SurfaceDark"), ("PrimaryTextDark", "PageBackgroundDark")
        ];

        foreach ((string accent, string background) in pairs)
        {
            double ratio = ColorContrast.Ratio(colors[accent], colors[background]);
            output.WriteLine($"{accent} on {background}: {ratio:F2}");
            Assert.True(ratio >= ColorContrast.BodyTextMinimum, $"{accent} on {background}: {ratio:F2}");
        }
    }

    [Fact]
    public void TextOnTheOtherSurfacesOfBothThemesIsReadable()
    {
        Dictionary<string, string> colors = ReadColors();
        (string Text, string Background, double Minimum)[] pairs =
        [
            ("TextPrimaryDark", "InputBackgroundDark", 4.5), ("TextSecondaryDark", "InputBackgroundDark", 4.5),
            ("TextPrimaryDark", "SurfaceElevatedDark", 4.5), ("TextSecondaryDark", "SurfaceElevatedDark", 4.5),
            ("TextPrimaryDark", "SurfaceHeroDark", 4.5), ("TextSecondaryDark", "SurfaceHeroDark", 4.5),
            ("TextPrimaryDark", "PrimaryTintDark", 4.5), ("TextSecondaryDark", "PrimaryTintDark", 4.5),
            ("PrimaryTextDark", "PrimaryTintDark", 4.5), ("PrimaryTextDark", "InputFocusBackgroundDark", 4.5),
            ("PrimaryTextDark", "SurfaceHeroDark", 4.5), ("PrimaryTextDark", "InputBackgroundDark", 4.5),
            ("PrimaryTextDark", "SurfaceElevatedDark", 4.5),
            ("InputPlaceholderDark", "InputBackgroundDark", 4.5),
            ("DangerDark", "SurfaceDark", 4.5), ("DangerDark", "PageBackgroundDark", 4.5),
            ("SuccessDark", "SurfaceDark", 4.5), ("SuccessDark", "PageBackgroundDark", 4.5),
            ("TextSecondaryLight", "InputBackgroundLight", 4.5), ("TextSecondaryLight", "SurfaceHeroLight", 4.5),
            ("TextSecondaryLight", "PrimaryTint", 4.5), ("TextPrimaryLight", "PrimaryTint", 4.5),
            ("PrimaryTextLight", "PrimaryTint", 4.5), ("PrimaryTextLight", "InputFocusBackgroundLight", 4.5),
            ("PrimaryTextLight", "SurfaceHeroLight", 4.5), ("PrimaryTextLight", "InputBackgroundLight", 4.5),
            ("InputPlaceholderLight", "InputBackgroundLight", 4.5),
            ("Danger", "SurfaceLight", 4.5), ("Danger", "PageBackground", 4.5),
            ("Success", "SurfaceLight", 4.5), ("Success", "PageBackground", 4.5)
        ];

        List<string> failures = [];
        foreach ((string text, string background, double minimum) in pairs)
        {
            double ratio = ColorContrast.Ratio(colors[text], colors[background]);
            output.WriteLine($"{text} on {background}: {ratio:F2}");
            if (ratio < minimum)
            {
                failures.Add($"{text} on {background}: {ratio:F2}");
            }
        }

        Assert.True(failures.Count == 0, string.Join("; ", failures));
    }

    [Fact]
    public void TheBreathingWordsAreReadableOnTheFullScreenPageAtEveryMomentOfTheExercise()
    {
        for (double calm = 0; calm <= 1.0001; calm += 0.1)
        {
            for (double scale = BreathingPattern.SmallScale; scale <= BreathingPattern.LargeScale + 0.0001; scale += 0.05)
            {
                string seen = BreathingColors.Composite(BreathingColors.ImmersiveAt(calm), BreathingColors.AlphaAt(scale), dark: true);
                double ratio = ColorContrast.Ratio(BreathingColors.TextHex(dark: true), seen);
                Assert.True(ratio >= ColorContrast.BodyTextMinimum, $"calm {calm:F1}, scale {scale:F2}: {ratio:F2}");
            }
        }
    }

    private static string StylePath(string file) => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation", "Resources", "Styles", file));

    private static Dictionary<string, string> ReadColors() =>
        Regex.Matches(File.ReadAllText(StylePath("Colors.xaml")), "<Color x:Key=\"(\\w+)\">(#[0-9A-Fa-f]{6,8})</Color>")
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);

    [Theory]
    [InlineData("#E53935")]
    [InlineData("#F7B548")]
    [InlineData("#2E9E5B")]
    [InlineData("#0085FF")]
    [InlineData("#FFFF00")]
    [InlineData("#FFFFFF")]
    public void AnyAccentCanBeMadeReadableAsTextOnBothThemes(string accent)
    {
        Assert.True(ColorContrast.Ratio(ColorContrast.Readable(accent, "#F3F4F6"), "#F3F4F6") >= ColorContrast.BodyTextMinimum, accent);
        Assert.True(ColorContrast.Ratio(ColorContrast.Readable(accent, "#1E1E1E"), "#1E1E1E") >= ColorContrast.BodyTextMinimum, accent);
    }

    [Fact]
    public void ADeepEnoughAccentIsKeptAsItIs()
    {
        Assert.Equal("#006ACC", ColorContrast.Readable("#006ACC", "#F3F4F6"));
    }

    [Fact]
    public void TheDefaultBlueGetsItsReadableShadeForLightText()
    {
        Assert.Equal("#006ACC", ColorContrast.Readable("#0085FF", "#F3F4F6"));
        Assert.Equal("#0085FF", ColorContrast.Readable("#0085FF", "#1E1E1E"));
    }

    [Theory]
    [InlineData("#E53935")]
    [InlineData("#F7B548")]
    [InlineData("#2E9E5B")]
    [InlineData("#0085FF")]
    [InlineData("#FFFF00")]
    [InlineData("#003366")]
    public void TheTextOnAnAccentButtonIsReadableForAnyAccent(string accent)
    {
        (string fill, string onFill) = ColorContrast.ForFill(accent);

        Assert.True(ColorContrast.Ratio(onFill, fill) >= ColorContrast.BodyTextMinimum, $"{accent} -> {fill} / {onFill}: {ColorContrast.Ratio(onFill, fill):F2}");
    }

    [Fact]
    public void ALightAccentGetsDarkTextAndADeepOneKeepsWhite()
    {
        Assert.Equal("#262626", ColorContrast.ForFill("#F7B548").OnFill);
        Assert.Equal("#FFFFFF", ColorContrast.ForFill("#003366").OnFill);
        Assert.Equal("#003366", ColorContrast.ForFill("#003366").Fill);
    }

    [Fact]
    public void TheDefaultButtonTokensAreTheComputedOnes()
    {
        Dictionary<string, string> colors = ReadColors();
        (string fill, string onFill) = ColorContrast.ForFill(colors["Primary"]);

        Assert.Equal(fill, colors["PrimaryFill"].ToUpperInvariant());
        Assert.Equal(onFill, colors["OnPrimary"].ToUpperInvariant());
    }

    [Fact]
    public void TheSendIconIsVisibleOnTheEmptyAndOnTheReadyButton()
    {
        Dictionary<string, string> colors = ReadColors();

        // Empty bar: the accent icon on the soft accent tint. Ready: the text colour of the accent fill on the fill itself.
        Assert.True(ColorContrast.Ratio(colors["PrimaryTextLight"], colors["PrimaryTint"]) >= ColorContrast.LargeTextMinimum);
        Assert.True(ColorContrast.Ratio(colors["PrimaryTextDark"], colors["PrimaryTintDark"]) >= ColorContrast.LargeTextMinimum);
        Assert.True(ColorContrast.Ratio(colors["OnPrimary"], colors["PrimaryFill"]) >= ColorContrast.BodyTextMinimum);
    }
}
