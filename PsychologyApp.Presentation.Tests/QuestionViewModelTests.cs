using Moq;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuestionViewModelTests
{
    [Fact]
    public async Task ConfirmCommand_IncompleteAnswers_ShowsValidationToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();

        List<Question> questions =
        [
            new()
            {
                Answers =
                [
                    new Answer { Ball = 1, Selected = false },
                    new Answer { Ball = 2, Selected = false }
                ]
            }
        ];

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Result {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            navigationService);

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        toast.Verify(t => t.LongToast("Нужно ответить на все вопросы"), Times.Once);
        dialog.Verify(
            d => d.AskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmCommand_CompleteAnswers_ShowsScoreDialog()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        dialog
            .Setup(d => d.AskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        List<Question> questions =
        [
            new()
            {
                Answers =
                [
                    new Answer { Ball = 3, Selected = true },
                    new Answer { Ball = 1, Selected = false }
                ]
            },
            new()
            {
                Answers =
                [
                    new Answer { Ball = 2, Selected = true },
                    new Answer { Ball = 4, Selected = false }
                ]
            }
        ];

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            navigationService);

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        dialog.Verify(
            d => d.AskAsync("Ваш результат: 5", "Score 5", "Завершить", "Продолжить"),
            Times.Once);
        navigation.Verify(n => n.PopToRootAsync(false), Times.Once);
    }
}
