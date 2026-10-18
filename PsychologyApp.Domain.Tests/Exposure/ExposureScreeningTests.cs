using PsychologyApp.Domain.Exposure;
using Xunit;

namespace PsychologyApp.Domain.Tests.Exposure;

public sealed class ExposureScreeningTests
{
    private static readonly ExposureScreeningAnswers None = new(false, false, false, false, false, false);

    [Fact]
    public void Self_harm_thoughts_lead_to_the_crisis_screen_whatever_else_is_answered()
    {
        Assert.Equal(ExposureScreeningResult.Crisis, ExposureScreening.Decide(
            new ExposureScreeningAnswers(true, true, true, true, true, true)));
    }

    [Theory]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, false, false, true)]
    public void Trauma_rituals_dissociation_or_substances_need_a_specialist(bool trauma, bool rituals, bool dissociation, bool substances)
    {
        ExposureScreeningResult result = ExposureScreening.Decide(None with
        {
            TraumaRelated = trauma,
            HeavyRituals = rituals,
            DissociationOrPsychosis = dissociation,
            SubstanceUse = substances
        });

        Assert.Equal(ExposureScreeningResult.Specialist, result);
        Assert.False(ExposureScreening.AllowsExposure(result));
    }

    [Fact]
    public void A_body_risk_allows_exposure_but_not_body_exercises()
    {
        ExposureScreeningResult result = ExposureScreening.Decide(None with { BodyRisk = true });

        Assert.Equal(ExposureScreeningResult.AllowedWithoutBodyExercises, result);
        Assert.True(ExposureScreening.AllowsExposure(result));
    }

    [Fact]
    public void No_risk_allows_exposure()
    {
        Assert.Equal(ExposureScreeningResult.Allowed, ExposureScreening.Decide(None));
    }

    [Fact]
    public void The_screening_has_to_be_repeated_after_four_weeks()
    {
        DateTime screened = new(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        ExposurePlan plan = new() { Screening = ExposureScreeningResult.Allowed, ScreenedAt = screened };

        Assert.True(ExposureScreening.IsCurrent(plan, screened.AddDays(27)));
        Assert.False(ExposureScreening.IsCurrent(plan, screened.AddDays(28)));
        Assert.False(ExposureScreening.IsCurrent(plan with { Screening = ExposureScreeningResult.Specialist }, screened.AddDays(1)));
        Assert.False(ExposureScreening.IsCurrent(new ExposurePlan(), screened));
    }
}
