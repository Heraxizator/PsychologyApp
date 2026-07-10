using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel
{
    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;
    public string ExplorePracticeButtonText => AppStrings.TestResultExplorePractice;
    public string RetakeButtonText => AppStrings.TestRetakeButton;
    public string BackToListButtonText => AppStrings.TestsBackToList;

    protected override void RefreshLocalizedProperties()
    {
        ApplyResult();
        Notify(
            nameof(PageTitle),
            nameof(TryTechniqueButtonText),
            nameof(ExplorePracticeButtonText),
            nameof(RetakeButtonText),
            nameof(BackToListButtonText),
            nameof(RecommendationHint),
            nameof(RecommendationTitle),
            nameof(RecommendationSubtitle),
            nameof(RecommendationTheme),
            nameof(RecommendationIconName));
    }
}
