using PsychologyApp.Application.Body;
using PsychologyApp.Presentation.Body;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Body;

[Collection("Localization")]
public sealed class BodyPresentationTests
{
    // A view twice as tall as wide: the 100 × 200 box fills it at scale 3.
    private const double Width = 300;
    private const double Height = 600;

    [Theory]
    [InlineData(50, 6, BodyArea.Head)]
    [InlineData(50, 15, BodyArea.Head)]
    [InlineData(50, 24, BodyArea.Face)]
    [InlineData(50, 34, BodyArea.Throat)]
    [InlineData(30, 42, BodyArea.Shoulders)]
    [InlineData(50, 58, BodyArea.Chest)]
    [InlineData(50, 85, BodyArea.Stomach)]
    [InlineData(18, 70, BodyArea.Arms)]
    [InlineData(82, 70, BodyArea.Arms)]
    [InlineData(40, 150, BodyArea.Legs)]
    public void A_tap_on_the_figure_picks_the_place_under_the_finger(double boxX, double boxY, BodyArea expected) =>
        Assert.Equal(expected, BodyMapLayout.AreaAt(boxX * 3, boxY * 3, Width, Height));

    [Theory]
    [InlineData(2, 2)]
    [InlineData(37.5, 3)]
    [InlineData(50, 199.5)]
    public void A_tap_beside_the_figure_picks_nothing(double boxX, double boxY) =>
        Assert.Null(BodyMapLayout.AreaAt(boxX * 3, boxY * 3, Width, Height));

    [Fact]
    public void A_wide_view_centres_the_figure_and_still_maps_taps()
    {
        // 600 × 600: scale 3, the box starts 150 from the left.
        Assert.Equal(BodyArea.Chest, BodyMapLayout.AreaAt(150 + 50 * 3, 58 * 3, 600, 600));
        Assert.Null(BodyMapLayout.AreaAt(50, 58 * 3, 600, 600));
    }

    [Fact]
    public void Every_place_sensation_feeling_and_time_has_its_own_name_in_both_languages()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            foreach (string language in new[] { "ru", "en" })
            {
                AppStrings.LanguageOverride = language;
                AssertDistinct(Enum.GetValues<BodyArea>().Select(BodyLabels.Area));
                AssertDistinct(Enum.GetValues<BodySensation>().Select(BodyLabels.Sensation));
                AssertDistinct(Enum.GetValues<BodyFeeling>().Select(BodyLabels.Feeling));
                AssertDistinct(Enum.GetValues<PartOfDay>().Select(BodyLabels.Time));
            }
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }

    private static void AssertDistinct(IEnumerable<string> names)
    {
        List<string> list = names.ToList();
        Assert.All(list, name => Assert.False(string.IsNullOrWhiteSpace(name)));
        Assert.Equal(list.Count, list.Distinct().Count());
    }
}
