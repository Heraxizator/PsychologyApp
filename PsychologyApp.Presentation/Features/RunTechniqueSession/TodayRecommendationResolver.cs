using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Practice;
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
    public TodayRecommendationResult Resolve(
        string onboardingConcern,
        string streakDisplay,
        bool hasStreak,
        INavigationService navigationService)
    {
        TechniqueId techniqueId = techniqueRecommendationService.ResolveFromOnboardingConcern(onboardingConcern);
        TechniqueDefinition definition = techniqueCatalog.Get(techniqueId);
        string durationText = AppStrings.TechniqueDuration(definition.ListDurationMinutes);

        return new TodayRecommendationResult
        {
            TechniqueId = techniqueId,
            ReasonText = AppStrings.TodayRecommendationReason(onboardingConcern),
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

    public void ApplyCatalogDate(TechniqueItem todayItem, TechniqueId techniqueId, IEnumerable<TechniqueItem> staticItems, bool hasStreak)
    {
        if (hasStreak)
        {
            return;
        }

        TechniqueListEntry entry = techniqueCatalog.GetBuiltInListEntries().First(e => e.TechniqueId == techniqueId);
        TechniqueItem? match = staticItems.FirstOrDefault(item => item.Number == entry.Number);
        if (match is not null)
        {
            todayItem.Date = match.Date;
        }
    }
}
