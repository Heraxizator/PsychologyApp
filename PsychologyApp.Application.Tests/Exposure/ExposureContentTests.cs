using PsychologyApp.Application.Exposure;
using PsychologyApp.Domain.Exposure;
using Xunit;

namespace PsychologyApp.Application.Tests.Exposure;

/// <summary>Both languages offer the same help: the same kinds, the same number of answers, the same ladder ratings and body flags.</summary>
public sealed class ExposureContentTests
{
    [Fact]
    public void Every_kind_of_fear_has_content_in_both_languages()
    {
        foreach (bool english in new[] { false, true })
        {
            Assert.Equal(Enum.GetValues<FearKind>(), ExposureContent.Kinds(english).Select(content => content.Kind));
        }
    }

    [Fact]
    public void Russian_and_english_match_one_to_one()
    {
        foreach ((ExposureKindContent ru, ExposureKindContent en) in ExposureContent.Kinds(false).Zip(ExposureContent.Kinds(true)))
        {
            Assert.Equal(ru.FearedOutcomes.Count, en.FearedOutcomes.Count);
            Assert.Equal(ru.AvoidedSituations.Count, en.AvoidedSituations.Count);
            Assert.Equal(ru.SafetyBehaviours.Count, en.SafetyBehaviours.Count);
            Assert.Equal(
                ru.Steps.Select(step => (step.ExpectedDistress, step.ProvokesBodySensations)),
                en.Steps.Select(step => (step.ExpectedDistress, step.ProvokesBodySensations)));
        }

        Assert.Equal(ExposureContent.Lessons(false).Count, ExposureContent.Lessons(true).Count);
    }

    [Fact]
    public void Example_ladders_climb_from_easy_to_hard_within_range()
    {
        foreach (ExposureKindContent content in ExposureContent.Kinds(false).Concat(ExposureContent.Kinds(true)))
        {
            List<int> ratings = content.Steps.Select(step => step.ExpectedDistress).ToList();
            Assert.All(ratings, rating => Assert.InRange(rating, 0, 100));
            Assert.Equal(ratings.Order(), ratings);
        }
    }
}
