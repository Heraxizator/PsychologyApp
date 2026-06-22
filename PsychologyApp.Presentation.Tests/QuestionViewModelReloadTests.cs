using Moq;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Tests;
using System.Reflection;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuestionViewModelReloadTests
{
    [Fact]
    public async Task RefreshLocalizedProperties_ReloadsQuestionsFromCatalog()
    {
        Mock<ITestCatalogService> catalogService = new();
        catalogService
            .Setup(service => service.GetByIdAsync("beck", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestDefinition
            {
                TestId = "beck",
                Title = "Beck",
                Subtitle = "Subtitle",
                Description = "Description",
                Comment = "Comment",
                Algorithm = [],
                Kind = TestKind.Questionnaire,
                Questions =
                [
                    new Question
                    {
                        Number = 1,
                        Context = "English question",
                        Answers = [new Answer { Number = 1, Ball = 1, Text = "Yes" }]
                    }
                ]
            });

        List<Question> questions =
        [
            new()
            {
                Number = 1,
                Context = "Russian question",
                Answers = [new Answer { Number = 1, Ball = 1, Text = "Да", Selected = true }]
            }
        ];

        QuestionViewModel viewModel = new(
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            Mock.Of<IToastService>(),
            Mock.Of<IDialogService>(),
            new TestNavigationService(Mock.Of<INavigation>()),
            Mock.Of<IUserProgressService>(),
            new QuestionnaireSubmissionService(),
            catalogService.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        MethodInfo? refresh = typeof(QuestionViewModel).GetMethod(
            "RefreshLocalizedProperties",
            BindingFlags.Instance | BindingFlags.NonPublic);
        refresh!.Invoke(viewModel, null);

        await Task.Delay(100);

        Assert.Equal("English question", viewModel.Questions[0].Context);
        Assert.True(viewModel.Questions[0].Answers[0].Selected);
    }
}
