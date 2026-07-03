using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTests.Question;

public partial class QuestionViewModel
{
    private bool _isCompleting;
    private QuestionnaireSavedResult? _savedResult;

    private async Task CalculateAnswersAsync()
    {
        if (_isCompleting)
        {
            return;
        }

        if (!_submissionService.TryValidateAllAnswered(Questions))
        {
            _toastService.LongToast(AppStrings.TestsAnswerAllToast);
            return;
        }

        if (_session is null)
        {
            return;
        }

        _isCompleting = true;
        _wizard.CancelAutoAdvance();
        _nextCommand.RaiseCanExecuteChanged();
        try
        {
            QuestionnaireCompletionRequest request =
                new(Questions, _session, _startedAtUtc);

            _savedResult ??= await _runCoordinator.SaveQuestionnaireAsync(
                request,
                _userProgressService).ConfigureAwait(false);

            await UiThread.RunAsync(async () =>
            {
                await _runCoordinator.NavigateQuestionnaireResultAsync(
                    _savedResult,
                    _navigationService).ConfigureAwait(true);
            }).ConfigureAwait(false);

            _savedResult = null;
        }
        catch (TestCompletionNavigationException ex)
        {
            _logger.LogWarning(ex, "Questionnaire result saved but navigation to result screen failed for {TestId}.", _session.TestId);
            _toastService.LongToast(AppStrings.TestsResultNavigationFailedMessage);
        }
        catch (Exception ex)
        {
            _savedResult = null;
            _logger.LogError(ex, "Failed to complete questionnaire for {TestId}.", _session.TestId);
            _toastService.LongToast(AppStrings.TestsResultSaveFailedMessage);
        }
        finally
        {
            _isCompleting = false;
            _nextCommand.RaiseCanExecuteChanged();
        }
    }
}
