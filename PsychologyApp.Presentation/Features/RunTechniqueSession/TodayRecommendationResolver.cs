using PsychologyApp.Application.Models;
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

public static class TodayRecommendationResolver
{
    public static TodayRecommendationResult Resolve(
        string onboardingConcern,
        string streakDisplay,
        bool hasStreak,
        INavigationService navigationService)
    {
        TechniqueId techniqueId = OnboardingRecommendation.ResolveTechnique(onboardingConcern);
        TechniqueDefinition definition = TechniqueCatalog.Get(techniqueId);
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

    public static void ApplyCatalogDate(TechniqueItem todayItem, TechniqueId techniqueId, IEnumerable<TechniqueItem> staticItems, bool hasStreak)
    {
        if (hasStreak)
        {
            return;
        }

        TechniqueListEntry entry = TechniqueListCatalog.GetBuiltIn().First(e => e.TechniqueId == techniqueId);
        TechniqueItem? match = staticItems.FirstOrDefault(item => item.Number == entry.Number);
        if (match is not null)
        {
            todayItem.Date = match.Date;
        }
    }
}
