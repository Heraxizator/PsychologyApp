using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel
{
    private async Task CalculateAnswersAsync()
    {
        if (!_submissionService.TryValidateAllAnswered(Questions))
        {
            _toastService.LongToast(AppStrings.TestsAnswerAllToast);
            return;
        }

        QuestionnaireSubmission submission = _submissionService.Calculate(Questions, Analyzer, _session);

        if (_session is not null)
        {
            await _submissionService.SaveAsync(
                _userProgressService,
                _session,
                submission.Score,
                submission.Interpretation,
                CancellationToken.None);
        }

        await _navigationService.GoToTestResultAsync(
            submission.Score,
            submission.Interpretation,
            submission.RecommendedTechnique,
            _session?.TestId,
            submission.InterpretationDetail,
            _session?.AnalyzerId);
    }
}
