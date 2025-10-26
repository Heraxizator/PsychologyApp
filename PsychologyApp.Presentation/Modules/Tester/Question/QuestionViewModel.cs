using MvvmHelpers;
using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Base.ServiceLocator.Toast;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.Modules.Tester;

public class QuestionViewModel : BaseViewModel
{
    public ObservableRangeCollection<Question> Questions { get; private set; } = [];
    public ICommand ConfirmCommand { get; private set; } = default!;
    public readonly bool IsSingleAnswer = default!;

    private Func<int, string> Analyzer { get; set; } = default!;

    public QuestionViewModel(INavigation navigation, List<Question> questions, Func<int, string> analyzer, bool singleAnswer)
    {
        Navigation = navigation;

        Analyzer = analyzer;

        IsSingleAnswer = singleAnswer;

        Questions.AddRange(questions);

        ConfirmCommand = new Command(async () => await CalculateAnswersAsync());
    }

    private async Task CalculateAnswersAsync()
    {
        if (Questions.All(x => x.Answers.Any(x => x.Selected is true)) is false)
        {
            ServiceLocator.Instance.GetService<IToastService>().LongToast("Нужно ответить на все вопросы");
            return;
        }

        int questionBalls = new();

        for (int index = 0; index < Questions.Count; index++)
        {
            questionBalls += Questions[index].Answers.Where(x => x.Selected is true).Sum(x => x.Ball);
        }

        bool finishSelected = await ServiceLocator.Instance.GetService<IDialogService>().AskAsync($"Ваш результат: {questionBalls}", ConfigureResultByBalls(questionBalls), "Завершить", "Повторить");

        await ConfigureEndAsync(finishSelected);
    }

    private async Task ConfigureEndAsync(bool isFinish)
    {
        if (isFinish is true)
        {
            await Navigation!.PopToRootAsync(false);
        }

        else
        {
            Array.ForEach(Questions.ToArray(), x => x.Answers.ForEach(x => x.Selected = false));
        }
    }

    private string ConfigureResultByBalls(int ball)
    {
        return Analyzer.Invoke(ball);
    }
}
