using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class QuestionViewModel
{
    public string PageTitle => AppStrings.TestsQuestionnaireTitle;
    public string QuestionPrefix => AppStrings.TestsQuestionPrefix;
    public string FinishButtonText => AppStrings.TestsFinishButton;

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(QuestionPrefix), nameof(FinishButtonText));
        ReloadQuestionsAsync().FireAndForget();
    }
}
