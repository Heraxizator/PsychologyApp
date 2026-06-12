using MvvmHelpers;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Common;
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
        _ = dialogService;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _session = session;

        Analyzer = analyzer;
        IsSingleAnswer = singleAnswer;
        Questions.AddRange(questions);

        ConfirmCommand = new AsyncCommand(CalculateAnswersAsync);
        BackCommand = new AsyncCommand(GoBackAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(QuestionPrefix), nameof(FinishButtonText));
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

        TechniqueId? recommended = _session?.AnalyzerId is string analyzerId
            ? TestScoreAnalyzers.RecommendTechnique(analyzerId, questionBalls)
            : null;

        await _navigationService.GoToTestResultAsync(questionBalls, interpretation, recommended, _session?.TestId);
    }

    private async Task SaveResultAsync(int score, string summary)
    {
        if (_session is null || string.IsNullOrWhiteSpace(_session.TestId))
        {
            return;
        }

        await _userProgressService.SaveTestResultAsync(_session.TestId, score, summary);
    }

    private string ConfigureResultByBalls(int ball) => Analyzer.Invoke(ball);
}
