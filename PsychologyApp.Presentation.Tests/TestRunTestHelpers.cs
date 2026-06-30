using Moq;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Tests;

internal static class TestRunTestHelpers
{
    public static TestRunCoordinator CreateCoordinator(ITestCatalogService? catalog = null)
    {
        ITestCatalogService catalogService = catalog ?? new Mock<ITestCatalogService>().Object;
        IQuestionnaireResultDetailService detailService = new QuestionnaireResultDetailService();
        IQuestionnaireScoringService scoringService = new QuestionnaireScoringService();

        return new TestRunCoordinator(
            new QuestionnaireSubmissionService(scoringService),
            new QuestionnaireDetailBuilder(detailService, catalogService));
    }

    public static TestRetakeOperations CreateRetakeOperations(ITestCatalogService? catalog = null) =>
        new(CreateCoordinator(catalog));

    public static TestHistoryLoader CreateHistoryLoader(ITestCatalogService? catalog = null)
    {
        ITestCatalogService catalogService = catalog ?? new Mock<ITestCatalogService>().Object;
        ITestTrendService trendService = new TestTrendService(catalogService);
        IQuestionnaireResultDetailService detailService = new QuestionnaireResultDetailService();

        return new TestHistoryLoader(
            new TestTrendResolver(trendService),
            new QuestionnaireDetailReader(detailService));
    }

    public static TestsListLoader CreateTestsListLoader(
        IUserProgressService progress,
        ITestCatalogService catalog) =>
        new(progress, catalog, CreateCoordinator(catalog));
}
