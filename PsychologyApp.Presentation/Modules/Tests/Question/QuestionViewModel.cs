using MvvmHelpers;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class QuestionViewModel : BaseViewModel
{
    public ObservableRangeCollection<Question> Questions { get; private set; } = [];
    public ICommand ConfirmCommand { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public readonly bool IsSingleAnswer = default!;

    public string PageTitle => AppStrings.TestsQuestionnaireTitle;
    public string QuestionPrefix => AppStrings.TestsQuestionPrefix;
    public string FinishButtonText => AppStrings.TestsFinishButton;

    private Func<int, string> Analyzer { get; set; } = default!;
    private readonly IToastService _toastService;
    private readonly IDialogService _dialogService;
    private readonly IUserProgressService _userProgressService;
    private readonly INavigationService _navigationService;
    private readonly TestSessionInfo? _session;

    public QuestionViewModel(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        IToastService toastService,
        IDialogService dialogService,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        TestSessionInfo? session = null)
    {
        BindNavigation(navigation, navigationService);
        _toastService = toastService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _session = session;

        Analyzer = analyzer;
        IsSingleAnswer = singleAnswer;
        Questions.AddRange(questions);

        ConfirmCommand = new AsyncCommand(CalculateAnswersAsync);
        BackCommand = new AsyncCommand(GoToRootAsync);
        UserPreferences.Changed += OnPreferencesChanged;
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(QuestionPrefix));
        OnPropertyChanged(nameof(FinishButtonText));
    }

    private async Task CalculateAnswersAsync()
    {
        if (Questions.All(x => x.Answers.Any(x => x.Selected is true)) is false)
        {
            _toastService.LongToast(AppStrings.TestsAnswerAllToast);
            return;
        }

        int questionBalls = 0;

        for (int index = 0; index < Questions.Count; index++)
        {
            questionBalls += Questions[index].Answers.Where(x => x.Selected is true).Sum(x => x.Ball);
        }

        string interpretation = ConfigureResultByBalls(questionBalls);
        await SaveResultAsync(questionBalls, interpretation);

        string message = interpretation;
        TechniqueId? recommended = _session?.AnalyzerId is string analyzerId
            ? TestScoreAnalyzers.RecommendTechnique(analyzerId, questionBalls)
            : null;
        if (recommended is TechniqueId techniqueId)
        {
            message += $"\n\n{AppStrings.TestTryTechnique}";
        }

        bool finishSelected = await _dialogService.AskAsync(
            AppStrings.TestsResultTitle(questionBalls),
            message,
            AppStrings.TestsFinishButton,
            recommended is not null ? AppStrings.TestTryTechnique : AppStrings.TestsContinueButton);

        if (!finishSelected && recommended is TechniqueId technique)
        {
            await _navigationService.GoToTechniqueAsync(technique);
            return;
        }

        await ConfigureEndAsync(finishSelected);
    }

    private async Task SaveResultAsync(int score, string summary)
    {
        if (_session is null || string.IsNullOrWhiteSpace(_session.TestId))
        {
            return;
        }

        await _userProgressService.SaveTestResultAsync(_session.TestId, score, summary);
    }

    private async Task ConfigureEndAsync(bool isFinish)
    {
        if (isFinish)
        {
            await GoToRootAsync();
        }
        else
        {
            Array.ForEach(Questions.ToArray(), x => x.Answers.ForEach(x => x.Selected = false));
        }
    }

    private string ConfigureResultByBalls(int ball) => Analyzer.Invoke(ball);
}
