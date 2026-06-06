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
    }

    private async Task CalculateAnswersAsync()
    {
        if (Questions.All(x => x.Answers.Any(x => x.Selected is true)) is false)
        {
            _toastService.LongToast("Нужно ответить на все вопросы");
            return;
        }

        int questionBalls = 0;

        for (int index = 0; index < Questions.Count; index++)
        {
            questionBalls += Questions[index].Answers.Where(x => x.Selected is true).Sum(x => x.Ball);
        }

        bool finishSelected = await _dialogService.AskAsync(
            $"Ваш результат: {questionBalls}",
            ConfigureResultByBalls(questionBalls),
            "Завершить",
            "Продолжить");

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
