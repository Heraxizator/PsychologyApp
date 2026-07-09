using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryViewModel
{
    public string PageTitle => AppStrings.TechniqueTheory;
    public string BackText => AppStrings.Back;
    public string LoadingText => AppStrings.PracticeLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
}
