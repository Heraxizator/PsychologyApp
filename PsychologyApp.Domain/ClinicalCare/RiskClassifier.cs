namespace PsychologyApp.Domain.ClinicalCare;

public readonly record struct RiskAssessmentSignals(
    bool HasSelfHarmThoughts,
    bool HasSevereDisorientation,
    bool HasSubstanceRisk,
    bool HasSevereInsomnia);

public static class RiskClassifier
{
    public static RiskLevel Classify(RiskAssessmentSignals signals)
    {
        if (signals.HasSelfHarmThoughts
            || signals.HasSevereDisorientation
            || signals.HasSubstanceRisk)
        {
            return RiskLevel.Red;
        }

        if (signals.HasSevereInsomnia)
        {
            return RiskLevel.Amber;
        }

        return RiskLevel.Green;
    }

    /// <summary>
    /// Soft risk from weekly mood/practice signals when no explicit assessment exists.
    /// </summary>
    public static RiskLevel DeriveFromMoodPracticeSignals(double averageMood, int practiceCount)
    {
        if (averageMood > 0 && averageMood <= 2.0)
        {
            return RiskLevel.Amber;
        }

        if (averageMood > 0 && averageMood <= 2.6 && practiceCount <= 1)
        {
            return RiskLevel.Amber;
        }

        return RiskLevel.Green;
    }
}
