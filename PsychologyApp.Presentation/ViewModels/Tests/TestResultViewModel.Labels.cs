using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestResultViewModel
{
    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string ScoreTitle => AppStrings.TestsResultTitle(Score);
    public string FinishButtonText => AppStrings.TestsFinishButton;
    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;
    public string RetakeButtonText => AppStrings.TestsRestart;

    protected override void RefreshLocalizedProperties()
    {
        RefreshRecommendationCopy();
        Notify(
            nameof(PageTitle),
            nameof(ScoreTitle),
            nameof(FinishButtonText),
            nameof(TryTechniqueButtonText),
            nameof(RetakeButtonText),
            nameof(RecommendedTechniqueTitle),
            nameof(RecommendationHint));
    }
}
