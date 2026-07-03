using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel
{
    public string RecommendationHint { get; private set; } = string.Empty;
    public string RecommendationTitle { get; private set; } = string.Empty;
    public string RecommendationSubtitle { get; private set; } = string.Empty;
    public string RecommendationTheme { get; private set; } = string.Empty;
    public string RecommendationIconName { get; private set; } = string.Empty;

    private void RefreshRecommendationCopy()
    {
        RecommendationHint = HasRecommendation ? AppStrings.TestsResultRecommendationHint : string.Empty;
        RecommendationTitle = string.Empty;
        RecommendationSubtitle = string.Empty;
        RecommendationTheme = string.Empty;
        RecommendationIconName = string.Empty;

        if (_result.RecommendedTechnique is TechniqueId techniqueId)
        {
            try
            {
                TechniqueDefinition definition = _techniqueCatalog.Get(techniqueId);
                RecommendationTitle = definition.ListTitle;
                RecommendationSubtitle = definition.ListSubtitle;
                RecommendationTheme = definition.Theme;
                RecommendationIconName = definition.ListIcon;
            }
            catch (Exception)
            {
                HasRecommendation = false;
                RecommendationHint = string.Empty;
            }
        }

        Notify(
            nameof(RecommendationHint),
            nameof(RecommendationTitle),
            nameof(RecommendationSubtitle),
            nameof(RecommendationTheme),
            nameof(RecommendationIconName),
            nameof(HasRecommendation));
    }
}
