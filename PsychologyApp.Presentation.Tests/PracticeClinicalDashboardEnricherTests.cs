using Microsoft.Extensions.Logging;
using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.ClinicalCare;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PracticeClinicalDashboardEnricherTests
{
    [Fact]
    public void FormatRiskBanner_MapsRedAndAmber()
    {
        Assert.Equal(AppStrings.ClinicalRedBanner, PracticeClinicalDashboardEnricher.FormatRiskBanner(RiskLevel.Red));
        Assert.Equal(AppStrings.ClinicalAmberBanner, PracticeClinicalDashboardEnricher.FormatRiskBanner(RiskLevel.Amber));
        Assert.Equal(string.Empty, PracticeClinicalDashboardEnricher.FormatRiskBanner(RiskLevel.Green));
    }

    [Fact]
    public void FormatProgramBanner_IncludesProgramNameAndWeek()
    {
        TherapyProgramStateDTO program = new()
        {
            ProgramType = TherapyProgramType.Anxiety,
            CurrentWeek = 2,
            IsActive = true,
            StartedAt = DateTime.UtcNow
        };

        string banner = PracticeClinicalDashboardEnricher.FormatProgramBanner(program);

        Assert.Contains(AppStrings.TherapyProgramAnxiety, banner, StringComparison.Ordinal);
        Assert.Contains("2", banner, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LoadAsync_WhenAdjustThrows_ReturnsSafetyFallbackBanner()
    {
        Mock<IClinicalCareService> clinical = new();
        clinical
            .Setup(c => c.AdjustProgramFromScorecardAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("scorecard failed"));

        PracticeClinicalDashboardEnricher enricher = new(
            clinical.Object,
            Mock.Of<ILogger<PracticeClinicalDashboardEnricher>>());

        PracticeClinicalDashboardSnapshot snapshot = await enricher.LoadAsync();

        Assert.Equal(string.Empty, snapshot.TherapyProgramBanner);
        Assert.Equal(AppStrings.ClinicalStatusUnavailableBanner, snapshot.ClinicalRiskBanner);
    }

    [Fact]
    public async Task LoadAsync_WhenProgramActive_BuildsTherapyBanner()
    {
        TherapyProgramStateDTO program = new()
        {
            ProgramType = TherapyProgramType.Mood,
            CurrentWeek = 1,
            IsActive = true,
            StartedAt = DateTime.UtcNow
        };

        Mock<IClinicalCareService> clinical = new();
        clinical
            .Setup(c => c.AdjustProgramFromScorecardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        clinical
            .Setup(c => c.GetActiveProgramAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);
        clinical
            .Setup(c => c.GetLatestRiskAssessmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((RiskAssessmentDTO?)null);
        clinical
            .Setup(c => c.GetActiveWeekAdherenceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((TherapyProgramAdherence?)null);

        PracticeClinicalDashboardEnricher enricher = new(
            clinical.Object,
            Mock.Of<ILogger<PracticeClinicalDashboardEnricher>>());

        PracticeClinicalDashboardSnapshot snapshot = await enricher.LoadAsync();

        Assert.Contains(AppStrings.TherapyProgramMood, snapshot.TherapyProgramBanner, StringComparison.Ordinal);
        Assert.Equal(string.Empty, snapshot.ClinicalRiskBanner);
        clinical.Verify(c => c.AdjustProgramFromScorecardAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
