using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel
{
    private async Task ReloadQuestionsAsync()
    {
        if (_session is null || string.IsNullOrWhiteSpace(_session.TestId))
        {
            return;
        }

        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_session.TestId);
        if (definition?.Questions is null)
        {
            return;
        }

        Dictionary<int, HashSet<int>> selectedAnswers = Questions.ToDictionary(
            question => question.Number,
            question => question.Answers
                .Where(answer => answer.Selected)
                .Select(answer => answer.Number)
                .ToHashSet());

        List<TestQuestion> reloaded = CloneQuestions(definition.Questions);

        foreach (TestQuestion question in reloaded)
        {
            if (!selectedAnswers.TryGetValue(question.Number, out HashSet<int>? selected))
            {
                continue;
            }

            foreach (Answer answer in question.Answers)
            {
                answer.Selected = selected.Contains(answer.Number);
            }
        }

        await UiThread.RunAsync(() =>
        {
            Questions.ReplaceRange(reloaded);
            _currentIndex = Math.Min(_currentIndex, Math.Max(0, Questions.Count - 1));
            RefreshCurrentAnswers();
            NotifyWizardState();
        });
    }

    private static List<TestQuestion> CloneQuestions(IReadOnlyList<TestQuestion> source) =>
        source.Select(question => new TestQuestion
        {
            Number = question.Number,
            Context = question.Context,
            Answers = question.Answers
                .Select(answer => new Answer
                {
                    Number = answer.Number,
                    Ball = answer.Ball,
                    Text = answer.Text,
                    Selected = false
                })
                .ToList()
        }).ToList();
}
