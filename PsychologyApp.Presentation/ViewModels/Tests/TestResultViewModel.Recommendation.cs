using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestResultViewModel
{
    public string RecommendedTechniqueTitle { get; private set; } = string.Empty;
    public string RecommendationHint { get; private set; } = string.Empty;

    private void RefreshRecommendationCopy()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId)
        {
            RecommendedTechniqueTitle = string.Empty;
            RecommendationHint = string.Empty;
            return;
        }

        string title = TechniqueCatalog.Get(techniqueId).ListTitle;
        RecommendedTechniqueTitle = AppStrings.TestRecommendationFor(title);
        string? reason = TestScoreAnalyzers.ResolveRecommendationReason(_analyzerId, Score);
        RecommendationHint = string.IsNullOrWhiteSpace(reason)
            ? AppStrings.TestsResultRecommendationHint
            : AppStrings.TestRecommendationReason(reason);
    }
}
