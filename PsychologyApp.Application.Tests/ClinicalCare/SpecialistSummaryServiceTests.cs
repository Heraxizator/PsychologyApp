using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.ClinicalCare;
using Xunit;

namespace PsychologyApp.Application.Tests.ClinicalCare;

public sealed class SpecialistSummaryServiceTests
{
    private static SpecialistSummaryService Create()
    {
        DateTime recent = DateTime.UtcNow.AddDays(-2);
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetMoodsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new MoodEntryDTO { MoodLevel = 2, RecordedAt = recent }, new MoodEntryDTO { MoodLevel = 4, RecordedAt = recent }]);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetAllTestResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new TestResultDTO { TestId = "phq9", Score = 14, Summary = "moderate", CompletedAt = recent }]);

        Mock<IClinicalCareService> care = new();
        care.Setup(c => c.GetLatestRiskAssessmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RiskAssessmentDTO { RiskLevel = RiskLevel.Amber, AssessedAt = recent });
        care.Setup(c => c.GetSafetyPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SafetyPlanDTO { WarningSigns = ["sign"], CopingStrategies = ["walk"] });

        Mock<IChatRepository> chats = new();
        chats.Setup(c => c.GetMemoryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new Dictionary<string, string>());

        return new SpecialistSummaryService(progress.Object, care.Object, chats.Object);
    }

    // A placeholder that does not exist in one language's wording would crash the export the person needs for an appointment.
    [Fact]
    public async Task The_summary_is_built_in_every_language_with_its_numbers()
    {
        SpecialistSummaryService service = Create();
        List<string> summaries = [];
        foreach (string language in new[] { "ru", "en", "fr", "de", "es", "pt", "zh" })
        {
            string summary = await service.BuildSummaryAsync(language);

            Assert.Contains("phq9", summary);
            Assert.Contains("14", summary);
            Assert.Contains("moderate", summary);
            Assert.DoesNotContain("{", summary);
            summaries.Add(summary);
        }

        Assert.Equal(summaries.Count, summaries.Distinct().Count());
        Assert.Equal(await service.BuildSummaryAsync("en"), await service.BuildSummaryAsync("it"));
    }
}
