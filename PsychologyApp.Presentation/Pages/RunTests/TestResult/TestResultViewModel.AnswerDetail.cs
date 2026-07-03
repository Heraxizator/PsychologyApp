using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel
{
    public string AnswerDetailTitle => AppStrings.TestResultAnswersTitle;
    public string AnswerDetailDurationText { get; private set; } = string.Empty;
    public IReadOnlyList<QuestionnaireResultQuestion> AnswerDetailQuestions { get; private set; } =
        Array.Empty<QuestionnaireResultQuestion>();
    public bool HasAnswerDetail { get; private set; }

    private void ApplyAnswerDetail(QuestionnaireResultDetail? detail)
    {
        HasAnswerDetail = detail is not null;
        AnswerDetailDurationText = detail is null
            ? string.Empty
            : AppStrings.TestResultDuration(detail.DurationSeconds);
        AnswerDetailQuestions = detail?.Questions ?? Array.Empty<QuestionnaireResultQuestion>();

        Notify(
            nameof(AnswerDetailTitle),
            nameof(AnswerDetailDurationText),
            nameof(AnswerDetailQuestions),
            nameof(HasAnswerDetail));
    }
}
