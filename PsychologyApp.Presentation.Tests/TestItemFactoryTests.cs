using Moq;
using PsychologyApp.Presentation.Entities.Test;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestItemFactoryTests
{
    [Fact]
    public async Task Create_Questionnaire_StartActionNavigatesWithSession()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new RecordingTestNavigationService(navigation.Object);
        TestDefinition definition = new()
        {
            TestId = "heck_hess",
            AnalyzerId = "heck_hess",
            Title = "Heck",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire,
            SingleAnswer = true,
            Questions =
            [
                new Question
                {
                    Answers = [new Answer { Ball = 1, Text = "Yes", Selected = false }]
                }
            ]
        };

        TestItem item = TestItemFactory.Create(definition, navigationService);
        await item.StartAsync();

        Assert.Equal("heck_hess", navigationService.LastSession?.TestId);
        Assert.Equal("heck_hess", navigationService.LastSession?.AnalyzerId);
        Assert.NotNull(navigationService.LastQuestions);
    }

    private sealed class RecordingTestNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public List<Question>? LastQuestions { get; private set; }
        public TestSessionInfo? LastSession { get; private set; }

        public override Task GoToQuestionPageAsync(
            List<Question> questions,
            Func<int, string> scoreAnalyzer,
            bool singleAnswer,
            TestSessionInfo? session = null)
        {
            LastQuestions = questions;
            LastSession = session;
            return Task.CompletedTask;
        }
    }
}
