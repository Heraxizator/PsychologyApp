using MvvmHelpers;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel
{
    private int _currentIndex;
    private CancellationTokenSource? _autoAdvanceCts;
    private AsyncCommand _nextCommand = default!;
    private bool _isValidationHintVisible;

    public ObservableRangeCollection<AnswerOptionItem> CurrentAnswers { get; } = [];

    public ICommand NextCommand => _nextCommand;
    public ICommand PreviousCommand { get; private set; } = default!;

    public bool IsPreviousVisible => !IsFirstStep;

    public bool IsMultiChoiceHintVisible => !IsSingleAnswer;

    public string MultiChoiceHintText => AppStrings.TestsMultiChoiceHint;

    public string TestFlowId => _session?.TestId ?? string.Empty;

    public string QuestionLeadText => AppStrings.TestsQuestionLead;

    public bool IsValidationHintVisible
    {
        get => _isValidationHintVisible;
        private set => SetProperty(ref _isValidationHintVisible, value);
    }

    public string ValidationHintText => AppStrings.TestsAnswerCurrentToast;

    public bool CanAdvance => IsCurrentQuestionAnswered();

    public int CurrentIndex
    {
        get => _currentIndex;
        private set
        {
            if (SetProperty(ref _currentIndex, value))
            {
                CancelAutoAdvance();
                ClearValidationHint();
                RefreshCurrentAnswers();
                NotifyWizardState();
            }
        }
    }

    public int TotalCount => Questions.Count;

    public TestQuestion? CurrentQuestion =>
        Questions.Count > 0 && _currentIndex >= 0 && _currentIndex < Questions.Count
            ? Questions[_currentIndex]
            : null;

    public int CurrentQuestionNumber => CurrentQuestion?.Number ?? 0;

    public string CurrentQuestionContext => CurrentQuestion?.Context ?? string.Empty;

    public bool HasCurrentQuestion => CurrentQuestion is not null;

    public string StepLabel => AppStrings.TestsStepOf(_currentIndex + 1, Math.Max(1, TotalCount));

    public string CurrentQuestionSemanticDescription =>
        string.IsNullOrWhiteSpace(CurrentQuestionContext)
            ? StepLabel
            : $"{StepLabel}. {CurrentQuestionContext}";

    public double Progress => TotalCount > 0 ? (_currentIndex + 1.0) / TotalCount : 0;

    public bool UseBarProgress => TotalCount > 7;

    public bool IsFirstStep => _currentIndex <= 0;

    public bool IsLastStep => TotalCount == 0 || _currentIndex >= TotalCount - 1;

    public string NextButtonText => IsLastStep ? FinishButtonText : AppStrings.TestsNextButton;

    public string PreviousButtonText => AppStrings.TestsPreviousButton;

    public int NextButtonColumn => IsFirstStep ? 0 : 1;

    public int NextButtonColumnSpan => IsFirstStep ? 2 : 1;

    private void InitializeWizard()
    {
        _nextCommand = new AsyncCommand(AdvanceAsync, () => CanAdvance);
        PreviousCommand = new AsyncCommand(GoPreviousAsync);
        RefreshCurrentAnswers();
        NotifyWizardState();
        LoadMetaAsync();
    }

    private void NotifyWizardState()
    {
        Notify(
            nameof(CurrentQuestion),
            nameof(CurrentQuestionNumber),
            nameof(CurrentQuestionContext),
            nameof(HasCurrentQuestion),
            nameof(StepLabel),
            nameof(CurrentQuestionSemanticDescription),
            nameof(Progress),
            nameof(UseBarProgress),
            nameof(IsFirstStep),
            nameof(IsLastStep),
            nameof(IsPreviousVisible),
            nameof(NextButtonColumn),
            nameof(NextButtonColumnSpan),
            nameof(NextButtonText),
            nameof(TotalCount),
            nameof(CanAdvance),
            nameof(RemainingDurationText),
            nameof(QuestionCountText));
        _nextCommand.RaiseCanExecuteChanged();
    }

    private void NotifyAnswerState()
    {
        ClearValidationHint();
        Notify(nameof(CanAdvance));
        _nextCommand.RaiseCanExecuteChanged();
    }

    private void ClearValidationHint()
    {
        if (_isValidationHintVisible)
        {
            IsValidationHintVisible = false;
        }
    }

    private void ShowValidationHint()
    {
        IsValidationHintVisible = true;
        ValidationHintRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? ValidationHintRequested;

    private void RefreshCurrentAnswers()
    {
        CurrentAnswers.Clear();
        if (CurrentQuestion is null)
        {
            return;
        }

        foreach (Answer answer in CurrentQuestion.Answers)
        {
            CurrentAnswers.Add(new AnswerOptionItem(answer, IsSingleAnswer, SelectAnswer));
        }
    }

    private void SelectAnswer(Answer answer)
    {
        if (CurrentQuestion is null)
        {
            return;
        }

        if (IsSingleAnswer)
        {
            foreach (Answer item in CurrentQuestion.Answers)
            {
                item.Selected = ReferenceEquals(item, answer);
            }

            foreach (AnswerOptionItem option in CurrentAnswers)
            {
                option.IsSelected = ReferenceEquals(option.Answer, answer);
            }

            NotifyAnswerState();
            TryAutoAdvanceAsync().FireAndForget();
            return;
        }

        answer.Selected = !answer.Selected;
        AnswerOptionItem? display = CurrentAnswers.FirstOrDefault(item => ReferenceEquals(item.Answer, answer));
        if (display is not null)
        {
            display.IsSelected = answer.Selected;
        }

        NotifyAnswerState();
    }

    private void CancelAutoAdvance()
    {
        if (_autoAdvanceCts is null)
        {
            return;
        }

        _autoAdvanceCts.Cancel();
        _autoAdvanceCts.Dispose();
        _autoAdvanceCts = null;
    }

    private async Task TryAutoAdvanceAsync()
    {
        if (!UserPreferences.Load().QuestionnaireAutoAdvance)
        {
            return;
        }

        if (!IsSingleAnswer || IsLastStep)
        {
            return;
        }

        CancelAutoAdvance();
        _autoAdvanceCts = new CancellationTokenSource();
        CancellationToken token = _autoAdvanceCts.Token;

        try
        {
            await Task.Delay(300, token);
            if (token.IsCancellationRequested || !IsCurrentQuestionAnswered())
            {
                return;
            }

            await AdvanceAsync();
        }
        catch (OperationCanceledException)
        {
            // Manual Next or step change cancelled the pending auto-advance.
        }
    }

    private async Task AdvanceAsync()
    {
        CancelAutoAdvance();

        if (!IsCurrentQuestionAnswered())
        {
            ShowValidationHint();
            return;
        }

        if (IsLastStep)
        {
            await CalculateAnswersAsync();
            return;
        }

        CurrentIndex++;
    }

    private async Task GoPreviousAsync()
    {
        if (IsFirstStep)
        {
            await GoBackAsync();
            return;
        }

        CurrentIndex--;
    }

    private bool IsCurrentQuestionAnswered() =>
        CurrentQuestion?.Answers.Any(answer => answer.Selected) == true;
}

public sealed class AnswerOptionItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public AnswerOptionItem(Answer answer, bool isSingleChoice, Action<Answer> onSelect)
    {
        Answer = answer;
        IsSingleChoice = isSingleChoice;
        _isSelected = answer.Selected;
        SelectCommand = new Command(() => onSelect(answer));
    }

    public Answer Answer { get; }

    public bool IsSingleChoice { get; }

    public string Text => Answer.Text ?? string.Empty;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public ICommand SelectCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
