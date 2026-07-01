using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.TestResult;

public partial class TestResultViewModel
{
    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;
    public string RetakeButtonText => AppStrings.TestRetakeButton;
    public string BackToListButtonText => AppStrings.TestsBackToList;

    protected override void RefreshLocalizedProperties()
    {
        ApplyResult();
        Notify(
            nameof(PageTitle),
            nameof(TryTechniqueButtonText),
            nameof(RetakeButtonText),
            nameof(BackToListButtonText),
            nameof(RecommendationHint),
            nameof(RecommendationTitle));
    }
}
