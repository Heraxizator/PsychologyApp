using MvvmHelpers;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.Question;

public partial class QuestionViewModel
{
    private readonly QuestionWizardCoordinator _wizard = new();
    private readonly SemaphoreSlim _advanceGate = new(1, 1);
    private AsyncCommand _nextCommand = default!;
    private bool _isValidationHintVisible;

    public ObservableRangeCollection<AnswerOptionItem> CurrentAnswers { get; } = [];

    public ICommand NextCommand => _nextCommand;
    public ICommand PreviousCommand { get; private set; } = default!;

    public bool IsPreviousVisible => !_wizard.IsFirstStep;

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

    public bool CanAdvance => !_isCompleting && IsCurrentQuestionAnswered();

    public int CurrentIndex => _wizard.CurrentIndex;

    public int TotalCount => Questions.Count;

    public TestQuestion? CurrentQuestion =>
        Questions.Count > 0 && _wizard.CurrentIndex >= 0 && _wizard.CurrentIndex < Questions.Count
            ? Questions[_wizard.CurrentIndex]
            : null;

    public int CurrentQuestionNumber => CurrentQuestion?.Number ?? 0;

    public string CurrentQuestionContext => CurrentQuestion?.Context ?? string.Empty;

    public bool HasCurrentQuestion => CurrentQuestion is not null;

    public string StepLabel => _wizard.StepLabel(TotalCount);

    public string CurrentQuestionSemanticDescription =>
        string.IsNullOrWhiteSpace(CurrentQuestionContext)
            ? StepLabel
            : $"{StepLabel}. {CurrentQuestionContext}";

    public double Progress => _wizard.Progress(TotalCount);

    public bool UseBarProgress => _wizard.UseBarProgress(TotalCount);

    public bool IsFirstStep => _wizard.IsFirstStep;

    public bool IsLastStep => _wizard.IsLastStep(TotalCount);

    public string NextButtonText => IsLastStep ? FinishButtonText : AppStrings.TestsNextButton;

    public string PreviousButtonText => AppStrings.TestsPreviousButton;

    public int NextButtonColumn => IsFirstStep ? 0 : 1;

    public int NextButtonColumnSpan => IsFirstStep ? 2 : 1;

    private void InitializeWizard()
    {
        _wizard.ValidationHintRequested += ShowValidationHint;
        _nextCommand = new AsyncCommand(AdvanceAsync, () => CanAdvance);
        PreviousCommand = new AsyncCommand(GoPreviousAsync);
        RefreshCurrentAnswers();
        NotifyWizardState();
        LoadMetaAsync();
    }

    private void NotifyWizardState()
    {
        Notify(
            nameof(CurrentIndex),
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
        if (_isCompleting || CurrentQuestion is null)
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
            if (!_isCompleting)
            {
                _wizard.TryAutoAdvanceAsync(
                    IsSingleAnswer,
                    TotalCount,
                    IsCurrentQuestionAnswered,
                    AdvanceAsync,
                    () => !_isCompleting).FireAndForget();
            }

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

    private async Task AdvanceAsync()
    {
        await _advanceGate.WaitAsync();
        try
        {
            _wizard.CancelAutoAdvance();

            bool reachedLastStep = _wizard.TryAdvance(TotalCount, IsCurrentQuestionAnswered, out bool showValidationHint);
            if (showValidationHint)
            {
                return;
            }

            if (reachedLastStep)
            {
                await CalculateAnswersAsync();
                return;
            }

            NotifyWizardState();
            RefreshCurrentAnswers();
        }
        finally
        {
            _advanceGate.Release();
        }
    }

    private async Task GoPreviousAsync()
    {
        if (_wizard.IsFirstStep)
        {
            await GoBackAsync();
            return;
        }

        _wizard.MovePrevious();
        NotifyWizardState();
        RefreshCurrentAnswers();
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
