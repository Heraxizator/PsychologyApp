using PsychologyApp.Application.Recommendations;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileFeaturedTechniquesBuilder(
    IUserPreferencesStore userPreferencesStore,
    TechniqueCatalogGateway techniqueCatalog,
    ITechniqueRecommendationService techniqueRecommendationService)
{
    public IReadOnlyList<TechniqueItem> Build(INavigationService navigationService)
    {
        string concern = userPreferencesStore.Load().OnboardingConcern;
        TechniqueId recommendedId = techniqueRecommendationService.ResolveFromOnboardingConcern(concern);
        TechniqueId[] featuredIds =
        [
            recommendedId,
            TechniqueId.Spin,
            TechniqueId.Paper,
            TechniqueId.Polarity
        ];

        List<TechniqueItem> items = [];
        HashSet<TechniqueId> added = [];

        foreach (TechniqueId techniqueId in featuredIds)
        {
            if (!added.Add(techniqueId))
            {
                continue;
            }

            TechniqueDefinition definition = techniqueCatalog.Get(techniqueId);
            string durationText = AppStrings.TechniqueDuration(definition.ListDurationMinutes);
            items.Add(new TechniqueItem
            {
                IconName = definition.ListIcon,
                DurationText = durationText,
                Title = definition.PageName,
                Subtitle = definition.ListSubtitle,
                Theme = definition.Theme,
                MetaText = AppStrings.TechniqueMetaLine(durationText, definition.Theme),
                Active = true,
                TapCommand = new AsyncCommand(() => navigationService.GoToTechniqueAsync(techniqueId))
            });
        }

        return items;
    }
}
