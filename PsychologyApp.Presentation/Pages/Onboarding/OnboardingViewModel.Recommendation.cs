using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
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
        _techniqueCatalog.Get(_techniqueRecommendationService.ResolveFromOnboardingConcern(ResolveConcernForRecommendation()));

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
