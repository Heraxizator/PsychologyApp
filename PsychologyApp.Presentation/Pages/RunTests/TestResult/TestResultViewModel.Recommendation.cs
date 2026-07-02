using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel
{
    public string RecommendationHint { get; private set; } = string.Empty;
    public string RecommendationTitle { get; private set; } = string.Empty;

    private void RefreshRecommendationCopy()
    {
        RecommendationHint = HasRecommendation ? AppStrings.TestsResultRecommendationHint : string.Empty;
        RecommendationTitle = string.Empty;

        if (_result.RecommendedTechnique is TechniqueId techniqueId)
        {
            try
            {
                RecommendationTitle = AppStrings.TestRecommendationFor(
                    _techniqueCatalog.Get(techniqueId).ListTitle);
            }
            catch (Exception)
            {
                HasRecommendation = false;
                RecommendationHint = string.Empty;
            }
        }

        Notify(nameof(RecommendationHint), nameof(RecommendationTitle), nameof(HasRecommendation));
    }
}
