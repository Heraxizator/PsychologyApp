using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class QuestionWizardCoordinator
{
    private int _currentIndex;
    private CancellationTokenSource? _autoAdvanceCts;

    public int CurrentIndex => _currentIndex;

    public event Action? ValidationHintRequested;

    public void Reset() => _currentIndex = 0;

    public void ClampIndex(int totalCount)
    {
        CancelAutoAdvance();
        if (totalCount <= 0)
        {
            _currentIndex = 0;
            return;
        }

        _currentIndex = Math.Clamp(_currentIndex, 0, totalCount - 1);
    }

    public bool IsFirstStep => _currentIndex <= 0;

    public bool IsLastStep(int totalCount) => totalCount == 0 || _currentIndex >= totalCount - 1;

    public double Progress(int totalCount) => totalCount > 0 ? (_currentIndex + 1.0) / totalCount : 0;

    public bool UseBarProgress(int totalCount) => totalCount > 7;

    public string StepLabel(int totalCount) =>
        AppStrings.TestsStepOf(_currentIndex + 1, Math.Max(1, totalCount));

    public bool TryAdvance(int totalCount, Func<bool> isCurrentAnswered, out bool showValidationHint)
    {
        CancelAutoAdvance();
        showValidationHint = false;

        if (!isCurrentAnswered())
        {
            showValidationHint = true;
            ValidationHintRequested?.Invoke();
            return false;
        }

        if (IsLastStep(totalCount))
        {
            return true;
        }

        _currentIndex++;
        return false;
    }

    public void MovePrevious()
    {
        CancelAutoAdvance();
        if (!IsFirstStep)
        {
            _currentIndex--;
        }
    }

    public void CancelAutoAdvance()
    {
        if (_autoAdvanceCts is null)
        {
            return;
        }

        _autoAdvanceCts.Cancel();
        _autoAdvanceCts.Dispose();
        _autoAdvanceCts = null;
    }

    public async Task TryAutoAdvanceAsync(
        bool isSingleAnswer,
        int totalCount,
        Func<bool> isCurrentAnswered,
        Func<Task> advanceAsync,
        Func<bool>? canAutoAdvance = null)
    {
        if (!UserPreferences.Load().QuestionnaireAutoAdvance)
        {
            return;
        }

        if (!isSingleAnswer || IsLastStep(totalCount))
        {
            return;
        }

        CancelAutoAdvance();
        _autoAdvanceCts = new CancellationTokenSource();
        CancellationToken token = _autoAdvanceCts.Token;

        try
        {
            await Task.Delay(300, token);
            if (token.IsCancellationRequested || canAutoAdvance?.Invoke() == false || !isCurrentAnswered())
            {
                return;
            }

            await advanceAsync();
        }
        catch (OperationCanceledException)
        {
            // Manual Next or step change cancelled the pending auto-advance.
        }
    }
}
