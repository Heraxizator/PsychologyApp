namespace PsychologyApp.Domain.Exposure;

public enum ExposureScreeningResult
{
    NotScreened,

    /// <summary>Thoughts of suicide or self-harm: the crisis screen, not exposure.</summary>
    Crisis,

    /// <summary>Trauma, heavy rituals, dissociation or psychosis, substance use: exposure only with a specialist.</summary>
    Specialist,

    /// <summary>Exposure in situations is fine, provoking body sensations is not (heart, epilepsy, asthma, pregnancy...).</summary>
    AllowedWithoutBodyExercises,

    Allowed
}

/// <param name="SelfHarmThoughts">Thoughts of suicide or self-harm now.</param>
/// <param name="TraumaRelated">The fear comes from a lived trauma, with flashbacks or nightmares (PTSD needs a supervised protocol).</param>
/// <param name="HeavyRituals">Rituals or checking take more than an hour a day (OCD needs ERP with a specialist).</param>
/// <param name="DissociationOrPsychosis">Episodes of unreality or "blanking out", voices or visions.</param>
/// <param name="SubstanceUse">Anxiety is regularly numbed with alcohol or other substances.</param>
/// <param name="BodyRisk">Heart disease, epilepsy, asthma, pregnancy, a recent operation or injury.</param>
public readonly record struct ExposureScreeningAnswers(
    bool SelfHarmThoughts,
    bool TraumaRelated,
    bool HeavyRituals,
    bool DissociationOrPsychosis,
    bool SubstanceUse,
    bool BodyRisk);

/// <summary>
/// Who may do exposure on their own. Exposure without a therapist is safe and effective for social, panic and specific fears,
/// but not for trauma, OCD, dissociation, psychosis or addiction, and not while there is a risk of self-harm.
/// </summary>
public static class ExposureScreening
{
    /// <summary>The screening is asked again after this time: the person's state can change.</summary>
    public static readonly TimeSpan ValidFor = TimeSpan.FromDays(28);

    public static ExposureScreeningResult Decide(ExposureScreeningAnswers answers)
    {
        if (answers.SelfHarmThoughts)
        {
            return ExposureScreeningResult.Crisis;
        }

        if (answers.TraumaRelated || answers.HeavyRituals || answers.DissociationOrPsychosis || answers.SubstanceUse)
        {
            return ExposureScreeningResult.Specialist;
        }

        return answers.BodyRisk ? ExposureScreeningResult.AllowedWithoutBodyExercises : ExposureScreeningResult.Allowed;
    }

    public static bool AllowsExposure(ExposureScreeningResult result) =>
        result is ExposureScreeningResult.Allowed or ExposureScreeningResult.AllowedWithoutBodyExercises;

    /// <summary>True when the plan may be worked on now: screened, allowed, and the screening is not older than <see cref="ValidFor"/>.</summary>
    public static bool IsCurrent(ExposurePlan plan, DateTime nowUtc) =>
        AllowsExposure(plan.Screening) && plan.ScreenedAt is { } screenedAt && nowUtc - screenedAt < ValidFor;
}
