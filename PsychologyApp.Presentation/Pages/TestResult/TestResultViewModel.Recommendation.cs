using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.TestResult;

public partial class TestResultViewModel
{
    public string RecommendationHint { get; private set; } = string.Empty;
    public string RecommendationTitle { get; private set; } = string.Empty;

    private void RefreshRecommendationCopy()
    {
        RecommendationHint = HasRecommendation ? AppStrings.TestsResultRecommendationHint : string.Empty;
        RecommendationTitle = _result.RecommendedTechnique is TechniqueId techniqueId
            ? AppStrings.TestRecommendationFor(_techniqueCatalog.Get(techniqueId).ListTitle)
            : string.Empty;

        Notify(nameof(RecommendationHint), nameof(RecommendationTitle));
    }
}
