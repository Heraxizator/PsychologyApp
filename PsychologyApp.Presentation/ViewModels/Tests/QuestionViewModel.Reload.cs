using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.ViewModels.Tests;

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

        List<Question> reloaded = CloneQuestions(definition.Questions);

        foreach (Question question in reloaded)
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

        await UiThread.RunAsync(() => Questions.ReplaceRange(reloaded));
    }

    private static List<Question> CloneQuestions(IReadOnlyList<Question> source) =>
        source.Select(question => new Question
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
