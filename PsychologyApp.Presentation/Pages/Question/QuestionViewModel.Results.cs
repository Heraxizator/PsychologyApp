using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;

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

        if (_session is null)
        {
            return;
        }

        await _runCoordinator.CompleteQuestionnaireAsync(
            new QuestionnaireCompletionRequest(Questions, _session, _startedAtUtc),
            _userProgressService,
            _navigationService);
    }
}
