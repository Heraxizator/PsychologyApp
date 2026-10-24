using PsychologyApp.Application.Exposure;
using PsychologyApp.Domain.Exposure;
using Xunit;

namespace PsychologyApp.Application.Tests.Exposure;

/// <summary>Every language offers the same help: the same kinds, the same number of answers, the same ladder ratings and body flags.</summary>
public sealed class ExposureContentTests
{
    public static TheoryData<string> Languages => new() { "ru", "en", "fr", "de", "es", "pt", "zh" };

    public static TheoryData<string> Translations => new() { "ru", "fr", "de", "es", "pt", "zh" };

    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_kind_of_fear_has_content_in_every_language(string language) =>
        Assert.Equal(Enum.GetValues<FearKind>(), ExposureContent.Kinds(language).Select(content => content.Kind));

    // A body-risk flag that moved to another step would offer stairs and coffee to someone whose screening found a heart risk.
    [Theory]
    [MemberData(nameof(Translations))]
    public void Every_language_matches_english_one_to_one(string language)
    {
        foreach ((ExposureKindContent translated, ExposureKindContent en) in ExposureContent.Kinds(language).Zip(ExposureContent.Kinds("en")))
        {
            Assert.Equal(en.FearedOutcomes.Count, translated.FearedOutcomes.Count);
            Assert.Equal(en.AvoidedSituations.Count, translated.AvoidedSituations.Count);
            Assert.Equal(en.SafetyBehaviours.Count, translated.SafetyBehaviours.Count);
            Assert.Equal(
                en.Steps.Select(step => (step.ExpectedDistress, step.ProvokesBodySensations)),
                translated.Steps.Select(step => (step.ExpectedDistress, step.ProvokesBodySensations)));
            Assert.NotEqual(en.Title, translated.Title);
        }

        Assert.Equal(ExposureContent.Lessons("en").Count, ExposureContent.Lessons(language).Count);
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public void Example_ladders_climb_from_easy_to_hard_within_range(string language)
    {
        foreach (ExposureKindContent content in ExposureContent.Kinds(language))
        {
            List<int> ratings = content.Steps.Select(step => step.ExpectedDistress).ToList();
            Assert.All(ratings, rating => Assert.InRange(rating, 0, 100));
            Assert.Equal(ratings.Order(), ratings);
        }
    }

    [Fact]
    public void A_language_without_its_own_examples_gets_the_english_ones() =>
        Assert.Same(ExposureContent.Kinds("en"), ExposureContent.Kinds("it"));
}
