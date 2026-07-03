using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.Onboarding;

public sealed class OnboardingRecommendationResult
{
    public required TechniqueId TechniqueId { get; init; }
    public required string Concern { get; init; }
    public required string IconName { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string ReasonText { get; init; }
}

public sealed class OnboardingRecommendationResolver(
    ITechniqueCatalogService techniqueCatalogService,
    ITechniqueRecommendationService techniqueRecommendationService)
{
    public OnboardingRecommendationResult Resolve(string? selectedConcern)
    {
        string concern = string.IsNullOrEmpty(selectedConcern) ? OnboardingConcernKeys.Explore : selectedConcern;
        TechniqueId techniqueId = techniqueRecommendationService.ResolveFromOnboardingConcern(concern);
        BuiltInTechniqueDefinition definition = techniqueCatalogService.Get(techniqueId);

        return new OnboardingRecommendationResult
        {
            TechniqueId = techniqueId,
            Concern = concern,
            IconName = definition.ListIcon,
            Title = definition.ListTitle,
            Subtitle = definition.ListSubtitle,
            ReasonText = AppStrings.TodayRecommendationReason(concern)
        };
    }
}
