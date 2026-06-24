using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string RecommendedIconName => GetRecommendationDefinition().ListIcon;
    public string RecommendedTitle => GetRecommendationDefinition().ListTitle;
    public string RecommendedSubtitle => GetRecommendationDefinition().ListSubtitle;
    public string RecommendedReason => AppStrings.TodayRecommendationReason(ResolveConcernForRecommendation());

    private string ResolveConcernForRecommendation() =>
        string.IsNullOrEmpty(SelectedConcern) ? OnboardingConcernKeys.Explore : SelectedConcern;

    private TechniqueDefinition GetRecommendationDefinition() =>
        TechniqueCatalog.Get(OnboardingRecommendation.ResolveTechnique(ResolveConcernForRecommendation()));

    private void NotifyRecommendation()
    {
        Notify(
            nameof(RecommendedIconName),
            nameof(RecommendedTitle),
            nameof(RecommendedSubtitle),
            nameof(RecommendedReason),
            nameof(FinishSubtitle));
    }
}
