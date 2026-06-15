using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Preferences;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Services.Profile;

public sealed class ProfileFeaturedTechniquesBuilder(IUserPreferencesStore userPreferencesStore)
{
    public IReadOnlyList<TechniqueItem> Build(INavigationService navigationService)
    {
        string concern = userPreferencesStore.Load().OnboardingConcern;
        TechniqueId recommendedId = OnboardingRecommendation.ResolveTechnique(concern);
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

            TechniqueDefinition definition = TechniqueCatalog.Get(techniqueId);
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
