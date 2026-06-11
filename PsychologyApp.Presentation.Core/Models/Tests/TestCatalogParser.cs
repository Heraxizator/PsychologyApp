using System.Text.Json;

namespace PsychologyApp.Presentation.Models.Tests;

public static class TestCatalogParser
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<ParseResult<T>> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            T? value = await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            if (value is null)
            {
                return ParseResult<T>.Failure("Deserialized value was null.");
            }

            return ParseResult<T>.Success(value);
        }
        catch (Exception ex)
        {
            return ParseResult<T>.Failure(ex.Message);
        }
    }

    public static ParseResult<TestDefinition> ParseGroupedQuestionnaire(JsonGroupedQuestionnaireDefinition definition)
    {
        if (TestScoreAnalyzers.Resolve(definition.AnalyzerId) is null)
        {
            return ParseResult<TestDefinition>.Failure($"Unknown analyzer '{definition.AnalyzerId}'.");
        }

        List<Question> questions = definition.Questions
            .Select((group, index) => new Question
            {
                Number = index + 1,
                Answers = group.Answers
                    .Select(answer => new Answer
                    {
                        Ball = answer.Ball,
                        Text = answer.Text,
                        Selected = false
                    })
                    .ToList()
            })
            .ToList();

        return ParseResult<TestDefinition>.Success(new TestDefinition
        {
            TestId = definition.AnalyzerId,
            AnalyzerId = definition.AnalyzerId,
            Title = definition.Title,
            Subtitle = definition.Subtitle,
            Description = definition.Description,
            Comment = definition.Comment,
            Algorithm = definition.Algorithm,
            Kind = TestKind.Questionnaire,
            Questions = questions,
            SingleAnswer = definition.SingleAnswer
        });
    }

    public static IReadOnlyList<TestDefinition> ParseLuscherDefinitions(IReadOnlyList<JsonNavigationTestDefinition> definitions)
    {
        List<TestDefinition> items = [];

        foreach (JsonNavigationTestDefinition definition in definitions)
        {
            LuscherMode mode = LuscherNavigationMapper.ParseMode(definition.NavigationTarget);

            items.Add(new TestDefinition
            {
                TestId = LuscherNavigationMapper.ToTestId(mode),
                Title = definition.Title,
                Subtitle = definition.Subtitle,
                Description = definition.Description,
                Comment = definition.Comment,
                Algorithm = definition.Algorithm,
                Kind = LuscherNavigationMapper.ToKind(mode),
                LuscherMode = mode
            });
        }

        return items;
    }

    public static IReadOnlyList<TestDefinition> ParseSimpleQuestionnaires(IReadOnlyList<JsonSimpleQuestionnaireDefinition> definitions)
    {
        List<TestDefinition> items = [];

        foreach (JsonSimpleQuestionnaireDefinition definition in definitions)
        {
            if (TestScoreAnalyzers.Resolve(definition.AnalyzerId) is null)
            {
                continue;
            }

            if (definition.Answers.Count != definition.Balls.Count)
            {
                continue;
            }

            List<Question> questions = [];

            for (int contextIndex = 0; contextIndex < definition.Questions.Count; contextIndex++)
            {
                List<Answer> answers = [];

                for (int answerIndex = 0; answerIndex < definition.Answers.Count; answerIndex++)
                {
                    answers.Add(new Answer
                    {
                        Number = contextIndex + 1,
                        Ball = definition.Balls[answerIndex],
                        Text = definition.Answers[answerIndex],
                        Selected = false
                    });
                }

                questions.Add(new Question
                {
                    Number = contextIndex + 1,
                    Answers = answers,
                    Context = definition.Questions[contextIndex]
                });
            }

            items.Add(new TestDefinition
            {
                TestId = definition.AnalyzerId,
                AnalyzerId = definition.AnalyzerId,
                Title = definition.Title,
                Subtitle = definition.Subtitle,
                Description = definition.Description,
                Comment = definition.Comment,
                Algorithm = definition.Algorithm,
                Kind = TestKind.Questionnaire,
                Questions = questions,
                SingleAnswer = definition.SingleAnswer
            });
        }

        return items;
    }
}
