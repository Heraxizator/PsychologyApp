using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel
{
    public string PageTitle => AppStrings.TestsQuestionnaireTitle;
    public string QuestionPrefix => AppStrings.TestsQuestionPrefix;
    public string FinishButtonText => AppStrings.TestsFinishButton;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(QuestionPrefix),
            nameof(FinishButtonText),
            nameof(NextButtonText),
            nameof(PreviousButtonText),
            nameof(StepLabel),
            nameof(Progress),
            nameof(UseBarProgress),
            nameof(MultiChoiceHintText),
            nameof(QuestionLeadText),
            nameof(ValidationHintText),
            nameof(DurationText),
            nameof(QuestionCountText),
            nameof(RemainingDurationText));
        ReloadQuestionsAsync().FireAndForget();
    }
}
