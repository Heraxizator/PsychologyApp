using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class TodayRecommendationResult
{
    public required TechniqueId TechniqueId { get; init; }
    public required TechniqueItem Item { get; init; }
    public required string ReasonText { get; init; }
}

public sealed class TodayRecommendationResolver(
    TechniqueCatalogGateway techniqueCatalog,
    ITechniqueRecommendationService techniqueRecommendationService)
{
    public async Task<TodayRecommendationResult> ResolveAsync(
        TodayRecommendationContext context,
        string streakDisplay,
        bool hasStreak,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        TodayRecommendationDecision decision = techniqueRecommendationService.ResolveTodayTechnique(context);
        TechniqueId techniqueId = decision.TechniqueId;
        TechniqueDefinition definition = await techniqueCatalog.GetAsync(techniqueId, cancellationToken);
        string durationText = AppStrings.TechniqueDuration(definition.ListDurationMinutes);

        return new TodayRecommendationResult
        {
            TechniqueId = techniqueId,
            ReasonText = ResolveReasonText(decision, context),
            Item = new TechniqueItem
            {
                Number = definition.ListNumber,
                Date = hasStreak ? streakDisplay : definition.ListDate,
                IconName = definition.ListIcon,
                DurationText = durationText,
                MetaText = AppStrings.TechniqueMetaLine(durationText, definition.Theme),
                Title = definition.ListTitle,
                Subtitle = definition.ListSubtitle,
                Theme = definition.Theme,
                Author = definition.Author,
                Active = true,
                TapCommand = new AsyncCommand(() => navigationService.GoToTechniqueAsync(techniqueId))
            }
        };
    }

    public async Task ApplyCatalogDateAsync(
        TechniqueItem todayItem,
        TechniqueId techniqueId,
        IEnumerable<TechniqueItem> staticItems,
        bool hasStreak,
        CancellationToken cancellationToken = default)
    {
        if (hasStreak)
        {
            return;
        }

        IReadOnlyList<TechniqueListEntry> entries =
            await techniqueCatalog.GetBuiltInListEntriesAsync(cancellationToken);
        TechniqueListEntry entry = entries.First(e => e.TechniqueId == techniqueId);
        TechniqueItem? match = staticItems.FirstOrDefault(item => item.Number == entry.Number);
        if (match is not null)
        {
            todayItem.Date = match.Date;
        }
    }

    private static string ResolveReasonText(TodayRecommendationDecision decision, TodayRecommendationContext context) =>
        decision.Source switch
        {
            TodayRecommendationSource.RecentTest =>
                AppStrings.TodayRecommendationReasonFromTest(decision.TestId ?? context.RecentTestResult?.TestId ?? string.Empty),
            TodayRecommendationSource.LowMood => AppStrings.TodayRecommendationReasonLowMood(),
            _ => AppStrings.TodayRecommendationReason(context.Concern)
        };
}
