using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PsychologyApp.Application.Exposure;
using PsychologyApp.Domain.Exposure;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.Exposure;

public enum ExposureMode
{
    Loading,
    Screening,
    Referral,
    Formulation,
    Ladder,
    Home,
    Before,
    After,
    Feedback
}

/// <summary>
/// The fear ladder on one screen that moves through its stages: safety screening, "my fear", building the ladder, the ladder
/// itself, a task before and after, and the feedback. The rules live in <see cref="ExposureLadder"/>; see docs/exposure-engine.md.
/// </summary>
public sealed class ExposureViewModel : BaseViewModel
{
    private const int MinStepsToSave = 3;

    private readonly INavigationService _navigation;
    private readonly IExposureService _service;
    private readonly TimeProvider _time;

    private ExposurePlan? _plan;
    private ExposureMode _mode = ExposureMode.Loading;
    private FearKind? _kind;
    private ExposureSession? _activeTask;
    private PredictionOutcome? _outcome;
    private bool? _usedSafety;
    private bool _deleteArmed;

    public ExposureViewModel(INavigationService navigation, IExposureService service, TimeProvider? time = null)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        _service = service;
        _time = time ?? TimeProvider.System;

        Questions =
        [
            Question(AppStrings.ExposureScreenSelfHarm),
            Question(AppStrings.ExposureScreenTrauma),
            Question(AppStrings.ExposureScreenRituals),
            Question(AppStrings.ExposureScreenDissociation),
            Question(AppStrings.ExposureScreenSubstances),
            Question(AppStrings.ExposureScreenBody)
        ];

        BackCommand = new AsyncCommand(LeaveStageAsync);
        ScreenCommand = new AsyncCommand(ScreenAsync);
        CrisisCommand = new AsyncCommand(() => _navigation.GoToCrisisHubAsync());
        SaveFormulationCommand = new AsyncCommand(SaveFormulationAsync);
        AddStepCommand = new Command(AddOwnStep);
        SaveLadderCommand = new AsyncCommand(SaveLadderAsync);
        StartTaskCommand = new Command(OpenBefore);
        GoCommand = new AsyncCommand(StartTaskAsync);
        ReportCommand = new Command(OpenAfter);
        SaveReportCommand = new AsyncCommand(SaveReportAsync);
        InterruptCommand = new AsyncCommand(InterruptAsync);
        DoneCommand = new Command(() => ShowHome());
        EditFearCommand = new Command(OpenFormulation);
        EditLadderCommand = new Command(OpenLadder);
        DeletePlanCommand = new AsyncCommand(DeletePlanAsync);
        SafetyAnswerCommand = new Command<string>(answer => UsedSafety = answer == "yes");

        LoadAsync().FireAndForget();
    }

    // ---------- Stage ----------

    public ExposureMode Mode
    {
        get => _mode;
        private set
        {
            if (SetProperty(ref _mode, value))
            {
                Notify(nameof(IsLoading), nameof(IsScreening), nameof(IsReferral), nameof(IsFormulation), nameof(IsLadder),
                    nameof(IsHome), nameof(IsBefore), nameof(IsAfter), nameof(IsFeedback));
            }
        }
    }

    public bool IsLoading => Mode == ExposureMode.Loading;
    public bool IsScreening => Mode == ExposureMode.Screening;
    public bool IsReferral => Mode == ExposureMode.Referral;
    public bool IsFormulation => Mode == ExposureMode.Formulation;
    public bool IsLadder => Mode == ExposureMode.Ladder;
    public bool IsHome => Mode == ExposureMode.Home;
    public bool IsBefore => Mode == ExposureMode.Before;
    public bool IsAfter => Mode == ExposureMode.After;
    public bool IsFeedback => Mode == ExposureMode.Feedback;

    public ICommand BackCommand { get; }

    // The examples are in the interface language; a language without its own gets the English ones.
    private static string Language => AppStrings.Language;

    private DateTime Now => _time.GetUtcNow().UtcDateTime;

    private async Task LoadAsync()
    {
        try
        {
            _plan = await _service.GetPlanAsync();
        }
        catch (Exception error)
        {
            BestEffort.Report(error);
        }

        await UiThread.RunAsync(() =>
        {
            if (_plan is null || !ExposureScreening.IsCurrent(_plan, Now))
            {
                Mode = ExposureMode.Screening;
            }
            else if (_plan.Steps.Count == 0)
            {
                OpenFormulation();
            }
            else
            {
                ShowHome();
            }
        });
    }

    /// <summary>Inside a stage "back" returns to the ladder; from the ladder or the first screens it leaves.</summary>
    private Task LeaveStageAsync()
    {
        bool hasLadder = _plan is { Steps.Count: > 0 } && ExposureScreening.IsCurrent(_plan, Now);
        if (hasLadder && Mode is ExposureMode.Formulation or ExposureMode.Ladder or ExposureMode.Before or ExposureMode.After or ExposureMode.Feedback)
        {
            ShowHome();
            return Task.CompletedTask;
        }

        return _navigation.GoBackAsync();
    }

    // ---------- Screening ----------

    public IReadOnlyList<ExposureQuestionItem> Questions { get; }
    public ICommand ScreenCommand { get; }
    public ICommand CrisisCommand { get; }
    public bool CanScreen => Questions.All(question => question.Answer is not null);

    private ExposureQuestionItem Question(string text)
    {
        ExposureQuestionItem? item = null;
        item = new ExposureQuestionItem
        {
            Text = text,
            SelectCommand = new Command<string>(answer =>
            {
                item!.Answer = answer == "yes";
                OnPropertyChanged(nameof(CanScreen));
            })
        };
        return item;
    }

    private string _referralText = string.Empty;
    public string ReferralText
    {
        get => _referralText;
        private set => SetProperty(ref _referralText, value);
    }

    private async Task ScreenAsync()
    {
        if (!CanScreen)
        {
            return;
        }

        bool[] answers = Questions.Select(question => question.Answer == true).ToArray();
        ExposureScreeningResult result = await _service.ScreenAsync(
            new ExposureScreeningAnswers(answers[0], answers[1], answers[2], answers[3], answers[4], answers[5]));
        _plan = await _service.GetPlanAsync();

        switch (result)
        {
            case ExposureScreeningResult.Crisis:
                await _navigation.GoToCrisisHubAsync();
                break;
            case ExposureScreeningResult.Specialist:
                ReferralText = AppStrings.ExposureReferral;
                Mode = ExposureMode.Referral;
                break;
            default:
                if (_plan is { Steps.Count: > 0 })
                {
                    ShowHome();
                }
                else
                {
                    OpenFormulation();
                }

                break;
        }
    }

    // ---------- Formulation ----------

    public ObservableCollection<ExposureChoiceItem> FearKinds { get; } = [];

    private string _fearedOutcome = string.Empty;
    public string FearedOutcome
    {
        get => _fearedOutcome;
        set => SetProperty(ref _fearedOutcome, value);
    }

    private string _fearedBelief = "7";
    public string FearedBelief
    {
        get => _fearedBelief;
        set => SetProperty(ref _fearedBelief, value);
    }

    private string _safetyText = string.Empty;
    public string SafetyText
    {
        get => _safetyText;
        set => SetProperty(ref _safetyText, value);
    }

    private string _avoidedText = string.Empty;
    public string AvoidedText
    {
        get => _avoidedText;
        set => SetProperty(ref _avoidedText, value);
    }

    private IReadOnlyList<string> _fearedSuggestions = [];
    public IReadOnlyList<string> FearedSuggestions
    {
        get => _fearedSuggestions;
        private set => SetProperty(ref _fearedSuggestions, value);
    }

    private IReadOnlyList<string> _safetySuggestions = [];
    public IReadOnlyList<string> SafetySuggestions
    {
        get => _safetySuggestions;
        private set => SetProperty(ref _safetySuggestions, value);
    }

    private IReadOnlyList<string> _avoidedSuggestions = [];
    public IReadOnlyList<string> AvoidedSuggestions
    {
        get => _avoidedSuggestions;
        private set => SetProperty(ref _avoidedSuggestions, value);
    }

    public bool HasKind => _kind is not null;
    public ICommand SaveFormulationCommand { get; }

    private void OpenFormulation()
    {
        FearKinds.Clear();
        foreach (ExposureKindContent content in ExposureContent.Kinds(Language))
        {
            FearKind kind = content.Kind;
            FearKinds.Add(new ExposureChoiceItem
            {
                Title = content.Title,
                Subtitle = content.Example,
                SelectCommand = new Command(() => SelectKind(kind))
            });
        }

        if (_plan is { } plan && !string.IsNullOrWhiteSpace(plan.FearedOutcome))
        {
            FearedOutcome = plan.FearedOutcome;
            FearedBelief = ToScale(plan.FearedOutcomeBelief);
            SafetyText = string.Join(", ", plan.SafetyBehaviours);
            AvoidedText = string.Join(", ", plan.AvoidedSituations);
            SelectKind(plan.Kind);
        }
        else
        {
            _kind = null;
            OnPropertyChanged(nameof(HasKind));
        }

        Mode = ExposureMode.Formulation;
    }

    private void SelectKind(FearKind kind)
    {
        _kind = kind;
        ExposureKindContent content = ExposureContent.For(kind, Language);
        for (int i = 0; i < FearKinds.Count; i++)
        {
            FearKinds[i].IsSelected = ExposureContent.Kinds(Language)[i].Kind == kind;
        }

        FearedSuggestions = content.FearedOutcomes;
        SafetySuggestions = content.SafetyBehaviours;
        AvoidedSuggestions = content.AvoidedSituations;
        OnPropertyChanged(nameof(HasKind));
    }

    private async Task SaveFormulationAsync()
    {
        if (_kind is not { } kind)
        {
            return;
        }

        _plan = await _service.SaveFormulationAsync(kind, FearedOutcome, FromScale(FearedBelief), SplitList(AvoidedText), SplitList(SafetyText));
        OpenLadder();
    }

    // ---------- Ladder builder ----------

    public ObservableCollection<ExposureStepRow> ExampleSteps { get; } = [];
    public ObservableCollection<ExposureStepRow> ChosenSteps { get; } = [];

    private string _newStepText = string.Empty;
    public string NewStepText
    {
        get => _newStepText;
        set => SetProperty(ref _newStepText, value);
    }

    private string _newStepDistress = "5";
    public string NewStepDistress
    {
        get => _newStepDistress;
        set => SetProperty(ref _newStepDistress, value);
    }

    public bool HasExamples => ExampleSteps.Count > 0;
    public bool CanSaveLadder => ChosenSteps.Count >= MinStepsToSave;
    public string LadderCountText => AppStrings.ExposureLadderCount(ChosenSteps.Count, MinStepsToSave);
    public ICommand AddStepCommand { get; }
    public ICommand SaveLadderCommand { get; }

    private void OpenLadder()
    {
        ChosenSteps.Clear();
        foreach (ExposureStep step in _plan is null ? [] : ExposureLadder.Ordered(_plan))
        {
            ChosenSteps.Add(EditableRow(step.Id, step.Text, step.ExpectedDistress));
        }

        ExampleSteps.Clear();
        bool bodyAllowed = _plan?.Screening != ExposureScreeningResult.AllowedWithoutBodyExercises;
        FearKind kind = _plan?.Kind ?? _kind ?? FearKind.Other;
        foreach (ExposureStepExample example in ExposureContent.For(kind, Language).Steps.Where(example => bodyAllowed || !example.ProvokesBodySensations))
        {
            ExposureStepRow? row = null;
            row = new ExposureStepRow
            {
                Id = string.Empty,
                Text = example.Text,
                ToggleCommand = new Command(() => ToggleExample(row!))
            };
            row.Distress = example.ExpectedDistress;
            row.IsChosen = ChosenSteps.Any(chosen => string.Equals(chosen.Text, example.Text, StringComparison.OrdinalIgnoreCase));
            ExampleSteps.Add(row);
        }

        NotifyLadder();
        Mode = ExposureMode.Ladder;
    }

    private ExposureStepRow EditableRow(string id, string text, int distress)
    {
        ExposureStepRow? row = null;
        row = new ExposureStepRow
        {
            Id = id,
            Text = text,
            EasierCommand = new Command(() => Rate(row!, -10)),
            HarderCommand = new Command(() => Rate(row!, +10)),
            ToggleCommand = new Command(() => RemoveChosen(row!))
        };
        row.Distress = distress;
        return row;
    }

    private void ToggleExample(ExposureStepRow example)
    {
        ExposureStepRow? chosen = ChosenSteps.FirstOrDefault(row => string.Equals(row.Text, example.Text, StringComparison.OrdinalIgnoreCase));
        if (chosen is null)
        {
            InsertSorted(EditableRow(string.Empty, example.Text, example.Distress));
        }
        else
        {
            ChosenSteps.Remove(chosen);
        }

        example.IsChosen = chosen is null;
        UiHaptics.Tick();
        NotifyLadder();
    }

    private void AddOwnStep()
    {
        string text = NewStepText.Trim();
        if (text.Length == 0)
        {
            return;
        }

        InsertSorted(EditableRow(string.Empty, text, FromScale(NewStepDistress)));
        NewStepText = string.Empty;
        UiHaptics.Tick();
        NotifyLadder();
    }

    private void RemoveChosen(ExposureStepRow row)
    {
        ChosenSteps.Remove(row);
        foreach (ExposureStepRow example in ExampleSteps.Where(example => string.Equals(example.Text, row.Text, StringComparison.OrdinalIgnoreCase)))
        {
            example.IsChosen = false;
        }

        NotifyLadder();
    }

    /// <summary>The person moves a step up or down the scale; the list keeps itself ordered from easy to hard.</summary>
    private void Rate(ExposureStepRow row, int delta)
    {
        row.Distress += delta;
        ChosenSteps.Remove(row);
        InsertSorted(row);
        UiHaptics.Tick();
    }

    private void InsertSorted(ExposureStepRow row)
    {
        int index = 0;
        while (index < ChosenSteps.Count && ChosenSteps[index].Distress <= row.Distress)
        {
            index++;
        }

        ChosenSteps.Insert(index, row);
    }

    private void NotifyLadder() => Notify(nameof(HasExamples), nameof(CanSaveLadder), nameof(LadderCountText));

    private async Task SaveLadderAsync()
    {
        if (!CanSaveLadder)
        {
            return;
        }

        _plan = await _service.SaveStepsAsync(ChosenSteps.Select(row => new ExposureStep(row.Id, row.Text, row.Distress)).ToList());
        ShowHome();
    }

    // ---------- Home ----------

    public ObservableCollection<ExposureStepRow> LadderRows { get; } = [];

    private string _progressTitle = string.Empty;
    public string ProgressTitle
    {
        get => _progressTitle;
        private set => SetProperty(ref _progressTitle, value);
    }

    private string _progressDetail = string.Empty;
    public string ProgressDetail
    {
        get => _progressDetail;
        private set => SetProperty(ref _progressDetail, value);
    }

    private string _fearSummary = string.Empty;
    public string FearSummary
    {
        get => _fearSummary;
        private set => SetProperty(ref _fearSummary, value);
    }

    private string _currentStepText = string.Empty;
    public string CurrentStepText
    {
        get => _currentStepText;
        private set => SetProperty(ref _currentStepText, value);
    }

    private string _pendingText = string.Empty;
    public string PendingText
    {
        get => _pendingText;
        private set => SetProperty(ref _pendingText, value);
    }

    public bool HasCurrentStep => CurrentStepText.Length > 0 && !HasPending;
    public bool HasPending => PendingText.Length > 0;
    public bool IsLadderDone => _plan is { Steps.Count: > 0 } plan && ExposureLadder.CurrentStep(plan) is null;
    public bool NeedsEasierStep => _plan is { } plan && ExposureLadder.CurrentStep(plan) is { } step && ExposureLadder.NeedsEasierStep(plan, step);
    public bool HasTestedPredictions => _plan is { } plan && ExposureLadder.Progress(plan).PredictionsTested > 0;
    public string DeleteText => _deleteArmed ? AppStrings.ExposureDeleteConfirm : AppStrings.ExposureDelete;

    public ICommand StartTaskCommand { get; }
    public ICommand ReportCommand { get; }
    public ICommand EditFearCommand { get; }
    public ICommand EditLadderCommand { get; }
    public ICommand DeletePlanCommand { get; }

    private void ShowHome()
    {
        _deleteArmed = false;
        if (_plan is not { } plan)
        {
            Mode = ExposureMode.Screening;
            return;
        }

        ExposureStep? current = ExposureLadder.CurrentStep(plan);
        LadderRows.Clear();
        foreach (ExposureStep step in ExposureLadder.Ordered(plan))
        {
            bool mastered = ExposureLadder.IsMastered(plan, step);
            int tasks = ExposureLadder.SessionsOf(plan, step.Id).Count(session => session.IsReported);
            ExposureStepRow row = new()
            {
                Id = step.Id,
                Text = step.Text,
                IsMastered = mastered,
                IsCurrent = step == current,
                StatusIcon = mastered ? "CheckCircle" : step == current ? "ArrowForward" : "RadioButtonUnchecked",
                StatusText = mastered
                    ? AppStrings.ExposureStepMastered
                    : tasks > 0 ? AppStrings.ExposureStepTasks(tasks) : string.Empty
            };
            row.Distress = step.ExpectedDistress;
            LadderRows.Add(row);
        }

        ExposureProgress progress = ExposureLadder.Progress(plan);
        ProgressTitle = progress.PredictionsTested == 0
            ? AppStrings.ExposureProgressStart
            : AppStrings.ExposureProgressPredictions(progress.PredictionsTested, progress.PredictionsCameTrue);
        ProgressDetail = AppStrings.ExposureProgressSteps(progress.StepsMastered, progress.StepsTotal)
                         + (progress.AverageBeliefDrop is { } drop ? " · " + AppStrings.ExposureProgressBeliefDrop((int)Math.Round(drop / 10.0)) : string.Empty);
        FearSummary = string.IsNullOrWhiteSpace(plan.FearedOutcome) ? string.Empty : AppStrings.ExposureFearSummary(plan.FearedOutcome);
        CurrentStepText = current?.Text ?? string.Empty;
        PendingText = ExposureLadder.Pending(plan) is { } pending
            ? plan.Steps.FirstOrDefault(step => step.Id == pending.StepId)?.Text ?? string.Empty
            : string.Empty;

        Notify(nameof(HasCurrentStep), nameof(HasPending), nameof(IsLadderDone), nameof(NeedsEasierStep), nameof(HasTestedPredictions),
            nameof(DeleteText), nameof(HasFearSummary));
        Mode = ExposureMode.Home;
    }

    public bool HasFearSummary => FearSummary.Length > 0;

    private async Task DeletePlanAsync()
    {
        // Two taps: the ladder holds weeks of records, a single slip must not erase it.
        if (!_deleteArmed)
        {
            _deleteArmed = true;
            OnPropertyChanged(nameof(DeleteText));
            return;
        }

        await _service.DeletePlanAsync();
        _plan = null;
        foreach (ExposureQuestionItem question in Questions)
        {
            question.Answer = null;
        }

        OnPropertyChanged(nameof(CanScreen));
        Mode = ExposureMode.Screening;
    }

    // ---------- Before a task ----------

    private string _taskStepText = string.Empty;
    public string TaskStepText
    {
        get => _taskStepText;
        private set => SetProperty(ref _taskStepText, value);
    }

    private string _prediction = string.Empty;
    public string Prediction
    {
        get => _prediction;
        set => SetProperty(ref _prediction, value);
    }

    private string _beliefBefore = "7";
    public string BeliefBefore
    {
        get => _beliefBefore;
        set => SetProperty(ref _beliefBefore, value);
    }

    private string _droppedText = string.Empty;
    public string DroppedText
    {
        get => _droppedText;
        set => SetProperty(ref _droppedText, value);
    }

    private IReadOnlyList<string> _predictionSuggestions = [];
    public IReadOnlyList<string> PredictionSuggestions
    {
        get => _predictionSuggestions;
        private set => SetProperty(ref _predictionSuggestions, value);
    }

    private IReadOnlyList<string> _droppedSuggestions = [];
    public IReadOnlyList<string> DroppedSuggestions
    {
        get => _droppedSuggestions;
        private set => SetProperty(ref _droppedSuggestions, value);
    }

    public ICommand GoCommand { get; }

    private void OpenBefore()
    {
        if (_plan is not { } plan || ExposureLadder.CurrentStep(plan) is not { } step)
        {
            return;
        }

        TaskStepText = step.Text;
        Prediction = string.Empty;
        BeliefBefore = "7";
        DroppedText = string.Empty;
        PredictionSuggestions = new[] { plan.FearedOutcome }
            .Concat(ExposureContent.For(plan.Kind, Language).FearedOutcomes)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        DroppedSuggestions = plan.SafetyBehaviours.Count > 0 ? plan.SafetyBehaviours : ExposureContent.For(plan.Kind, Language).SafetyBehaviours;
        Mode = ExposureMode.Before;
    }

    private async Task StartTaskAsync()
    {
        if (_plan is not { } plan || ExposureLadder.CurrentStep(plan) is not { } step)
        {
            return;
        }

        await _service.StartTaskAsync(step.Id, Prediction, FromScale(BeliefBefore), SplitList(DroppedText));
        _plan = await _service.GetPlanAsync();
        UiHaptics.Tick();
        ShowHome();
    }

    // ---------- After a task ----------

    public ObservableCollection<ExposureChoiceItem> Outcomes { get; } = [];

    private string _whatHappened = string.Empty;
    public string WhatHappened
    {
        get => _whatHappened;
        set => SetProperty(ref _whatHappened, value);
    }

    private string _beliefAfter = "5";
    public string BeliefAfter
    {
        get => _beliefAfter;
        set => SetProperty(ref _beliefAfter, value);
    }

    private string _peakDistress = "5";
    public string PeakDistress
    {
        get => _peakDistress;
        set => SetProperty(ref _peakDistress, value);
    }

    private string _learned = string.Empty;
    public string Learned
    {
        get => _learned;
        set => SetProperty(ref _learned, value);
    }

    public IReadOnlyList<string> LessonSuggestions => ExposureContent.Lessons(Language);

    public bool? UsedSafety
    {
        get => _usedSafety;
        private set
        {
            SetProperty(ref _usedSafety, value);
            Notify(nameof(UsedSafetyYes), nameof(UsedSafetyNo), nameof(CanReport));
        }
    }

    public bool UsedSafetyYes => _usedSafety == true;
    public bool UsedSafetyNo => _usedSafety == false;
    public ICommand SafetyAnswerCommand { get; }

    private string _predictionReminder = string.Empty;
    public string PredictionReminder
    {
        get => _predictionReminder;
        private set => SetProperty(ref _predictionReminder, value);
    }

    public bool CanReport => _outcome is not null && _usedSafety is not null;
    public ICommand SaveReportCommand { get; }
    public ICommand InterruptCommand { get; }

    private void OpenAfter()
    {
        if (_plan is not { } plan || ExposureLadder.Pending(plan) is not { } pending)
        {
            return;
        }

        _activeTask = pending;
        TaskStepText = plan.Steps.FirstOrDefault(step => step.Id == pending.StepId)?.Text ?? string.Empty;
        PredictionReminder = string.IsNullOrWhiteSpace(pending.Prediction)
            ? string.Empty
            : AppStrings.ExposurePredictionReminder(pending.Prediction, ToScale(pending.BeliefBefore));
        WhatHappened = string.Empty;
        Learned = string.Empty;
        BeliefAfter = ToScale(pending.BeliefBefore);
        PeakDistress = "5";
        _outcome = null;
        UsedSafety = null;

        Outcomes.Clear();
        foreach ((PredictionOutcome outcome, string title) in new[]
                 {
                     (PredictionOutcome.NotHappened, AppStrings.ExposureOutcomeNo),
                     (PredictionOutcome.Partly, AppStrings.ExposureOutcomePartly),
                     (PredictionOutcome.Happened, AppStrings.ExposureOutcomeYes)
                 })
        {
            Outcomes.Add(new ExposureChoiceItem { Title = title, SelectCommand = new Command(() => SelectOutcome(outcome)) });
        }

        Mode = ExposureMode.After;
    }

    private void SelectOutcome(PredictionOutcome outcome)
    {
        _outcome = outcome;
        PredictionOutcome[] order = [PredictionOutcome.NotHappened, PredictionOutcome.Partly, PredictionOutcome.Happened];
        for (int i = 0; i < Outcomes.Count; i++)
        {
            Outcomes[i].IsSelected = order[i] == outcome;
        }

        UiHaptics.Tick();
        OnPropertyChanged(nameof(CanReport));
    }

    private async Task SaveReportAsync()
    {
        if (_activeTask is not { } task || _outcome is not { } outcome || _usedSafety is not { } usedSafety)
        {
            return;
        }

        ExposureReportResult result = await _service.ReportTaskAsync(
            task.Id,
            new ExposureReport(WhatHappened, outcome, FromScale(BeliefAfter), FromScale(PeakDistress), usedSafety, Learned));
        _plan = result.Plan;

        if (result.Crisis)
        {
            // Words that read as a crisis come first: help, not the next step of the ladder.
            await _navigation.GoToCrisisHubAsync();
            return;
        }

        ShowFeedback(result.Plan.Sessions.First(session => session.Id == task.Id));
    }

    private async Task InterruptAsync()
    {
        if (_activeTask is not { } task)
        {
            return;
        }

        _plan = await _service.InterruptTaskAsync(task.Id, FromScale(PeakDistress));
        ShowFeedback(_plan.Sessions.First(session => session.Id == task.Id));
    }

    // ---------- Feedback ----------

    public ObservableCollection<string> Insights { get; } = [];

    private string _beliefChangeText = string.Empty;
    public string BeliefChangeText
    {
        get => _beliefChangeText;
        private set => SetProperty(ref _beliefChangeText, value);
    }

    public bool HasBeliefChange => BeliefChangeText.Length > 0;
    public ICommand DoneCommand { get; }

    private void ShowFeedback(ExposureSession session)
    {
        BeliefChangeText = session is { Outcome: not null, BeliefAfter: { } after }
            ? AppStrings.ExposureBeliefChange(ToScale(session.BeliefBefore), ToScale(after))
            : string.Empty;

        Insights.Clear();
        if (!string.IsNullOrWhiteSpace(session.Learned))
        {
            Insights.Add(AppStrings.ExposureLearnedSummary(session.Learned));
        }

        foreach (ExposureInsight insight in ExposureInsights.After(_plan!, session))
        {
            Insights.Add(insight switch
            {
                ExposureInsight.Disconfirmed => AppStrings.ExposureInsightDisconfirmed,
                ExposureInsight.CameTrue => AppStrings.ExposureInsightCameTrue,
                ExposureInsight.Interrupted => AppStrings.ExposureInsightInterrupted,
                ExposureInsight.SafetyBehaviourUsed => AppStrings.ExposureInsightSafety,
                ExposureInsight.StepMastered => AppStrings.ExposureInsightMastered,
                ExposureInsight.LadderDone => AppStrings.ExposureInsightLadderDone,
                _ => AppStrings.ExposureInsightEasier
            });
        }

        OnPropertyChanged(nameof(HasBeliefChange));
        UiHaptics.Tick();
        Mode = ExposureMode.Feedback;
    }

    // ---------- Helpers ----------

    /// <summary>Ratings are asked on the app's usual 0..10 scale and kept as 0..100 (percent of belief, distress).</summary>
    private static int FromScale(string value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int scale) ? Math.Clamp(scale, 0, 10) * 10 : 50;

    private static string ToScale(int percent) =>
        ((int)Math.Round(Math.Clamp(percent, 0, 100) / 10.0)).ToString(CultureInfo.InvariantCulture);

    private static List<string> SplitList(string text) =>
        text.Split([',', ';', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    protected override void RefreshLocalizedProperties()
    {
        if (Mode == ExposureMode.Home)
        {
            ShowHome();
        }
    }
}
