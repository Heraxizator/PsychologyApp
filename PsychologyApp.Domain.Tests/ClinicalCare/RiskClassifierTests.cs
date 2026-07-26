using PsychologyApp.Domain.ClinicalCare;
using Xunit;

namespace PsychologyApp.Domain.Tests.ClinicalCare;

public sealed class RiskClassifierTests
{
    [Theory]
    [InlineData(true, false, false, false, RiskLevel.Red)]
    [InlineData(false, true, false, false, RiskLevel.Red)]
    [InlineData(false, false, true, false, RiskLevel.Red)]
    [InlineData(false, false, false, true, RiskLevel.Amber)]
    [InlineData(false, false, false, false, RiskLevel.Green)]
    public void Classify_ReturnsExpectedLevel(
        bool selfHarm,
        bool disorientation,
        bool substance,
        bool insomnia,
        RiskLevel expected)
    {
        RiskLevel actual = RiskClassifier.Classify(new RiskAssessmentSignals(
            selfHarm,
            disorientation,
            substance,
            insomnia));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1.5, 5, RiskLevel.Amber)]
    [InlineData(2.0, 5, RiskLevel.Amber)]
    [InlineData(2.5, 1, RiskLevel.Amber)]
    [InlineData(2.5, 2, RiskLevel.Green)]
    [InlineData(3.0, 0, RiskLevel.Green)]
    [InlineData(0, 0, RiskLevel.Green)]
    public void DeriveFromMoodPracticeSignals_ReturnsExpectedLevel(
        double averageMood,
        int practiceCount,
        RiskLevel expected)
    {
        Assert.Equal(expected, RiskClassifier.DeriveFromMoodPracticeSignals(averageMood, practiceCount));
    }
}
