using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class NextPracticeResult
{
    public required TechniqueId TechniqueId { get; init; }
    public required string Caption { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string ReasonText { get; init; }
    public required string IconName { get; init; }
    public required string ActionText { get; init; }
}

public sealed class NextPracticeResolver(
    IUserProgressService userProgressService,
    IUserPreferencesStore userPreferencesStore,
    ITechniqueRecommendationService techniqueRecommendationService,
    TechniqueCatalogGateway techniqueCatalog)
{
    public async Task<NextPracticeResult?> ResolveAsync(
        TechniqueId completedTechniqueId,
        CancellationToken cancellationToken = default)
    {
        string concern = userPreferencesStore.Load().OnboardingConcern;
        TodayRecommendationContext context = await TodayRecommendationContextBuilder.BuildAsync(
            userProgressService,
            concern,
            cancellationToken);

        TechniqueId nextId = techniqueRecommendationService.ResolveNextAfterCompletion(
            context,
            completedTechniqueId);

        TechniqueDefinition definition = await techniqueCatalog.GetAsync(nextId, cancellationToken);

        return new NextPracticeResult
        {
            TechniqueId = nextId,
            Caption = AppStrings.PracticeNextCaption,
            Title = definition.ListTitle,
            Subtitle = definition.ListSubtitle,
            ReasonText = AppStrings.PracticeNextReason,
            IconName = definition.ListIcon,
            ActionText = AppStrings.TodayStartPractice
        };
    }
}
