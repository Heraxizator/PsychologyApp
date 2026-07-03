using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;

namespace PsychologyApp.Presentation.Tests;

internal static class TechniqueCatalogTestHelper
{
    public static TechniqueCatalogGateway CreateGateway(string languageKey = "ru") =>
        new(new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => languageKey)));

    public static ITechniqueRecommendationService CreateRecommendationService() =>
        new TechniqueRecommendationService();

    public static TodayRecommendationResolver CreateTodayRecommendationResolver(string languageKey = "ru") =>
        new(CreateGateway(languageKey), CreateRecommendationService());
}
