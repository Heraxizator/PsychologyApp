using MvvmHelpers;
using PsychologyApp.Presentation.Modules.Tests;
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

    public QuestionViewModel(
        INavigation navigation,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        IToastService toastService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        BindNavigation(navigation, navigationService);
        _toastService = toastService;
        _dialogService = dialogService;

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

        bool finishSelected = await _dialogService.AskAsync(
            AppStrings.TestsResultTitle(questionBalls),
            ConfigureResultByBalls(questionBalls),
            AppStrings.TestsFinishButton,
            AppStrings.TestsContinueButton);

        await ConfigureEndAsync(finishSelected);
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
