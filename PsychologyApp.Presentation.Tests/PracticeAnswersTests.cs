using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PracticeAnswersTests
{
    private static readonly TechniqueEntrySeed[] ThoughtRecord =
    [
        new("Ситуация"),
        new("Автоматическая мысль"),
        new("Эмоция", Kind: EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
        new("Альтернативная мысль"),
        new("Эмоция сейчас", Kind: EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
    ];

    [Fact]
    public void Rank_puts_often_used_chips_first_and_keeps_the_rest_in_order()
    {
        string[] chips = ["Окно", "Стол", "Компьютер", "Небо"];
        Dictionary<string, int> usage = new() { ["Компьютер"] = 5, ["Небо"] = 1 };

        Assert.Equal(["Компьютер", "Небо", "Окно", "Стол"], PracticeAnswers.Rank(chips, usage));
    }

    [Fact]
    public void Rank_without_usage_keeps_the_written_order()
    {
        string[] chips = ["Окно", "Стол", "Компьютер"];

        Assert.Equal(chips, PracticeAnswers.Rank(chips, new Dictionary<string, int>()));
    }

    [Theory]
    [InlineData(new[] { "", "", "" }, 0)]
    [InlineData(new[] { "Ссора", "", "" }, 1)]
    [InlineData(new[] { "Ссора", "Мысль", "" }, 2)]
    [InlineData(new[] { "Ссора", "", "Ответ" }, 1)]
    [InlineData(new[] { "Ссора", "Мысль", "Ответ" }, 2)]
    public void ResumeStep_opens_the_first_unanswered_step(string[] answers, int expected)
    {
        Assert.Equal(expected, PracticeAnswers.ResumeStep(answers));
    }

    [Fact]
    public void ResumeStep_of_an_empty_practice_is_the_first()
    {
        Assert.Equal(0, PracticeAnswers.ResumeStep([]));
    }

    [Fact]
    public void FromPayload_pairs_answers_with_questions_and_leaves_out_the_tension()
    {
        const string payload = """{"Fields":{"0":"Не ответили на сообщение","1":"«Меня осудят»","2":"8","3":"Человек занят","4":"5"}}""";

        IReadOnlyList<PracticeAnswer> answers = PracticeAnswers.FromPayload(payload, ThoughtRecord);

        Assert.Equal(
            [
                new PracticeAnswer("Ситуация", "Не ответили на сообщение"),
                new PracticeAnswer("Автоматическая мысль", "«Меня осудят»"),
                new PracticeAnswer("Альтернативная мысль", "Человек занят")
            ],
            answers);
    }

    [Fact]
    public void FromPayload_shows_other_scales_as_a_score()
    {
        TechniqueEntrySeed[] future = [new("Проблема"), new("Важность через 50 лет", Kind: EntryFieldKind.Rating0To10)];

        IReadOnlyList<PracticeAnswer> answers = PracticeAnswers.FromPayload("""{"fields":{"0":"Экзамен","1":"2"}}""", future);

        Assert.Equal(new PracticeAnswer("Важность через 50 лет", "2 / 10"), answers[1]);
    }

    [Fact]
    public void FromPayload_lists_the_thoughts_of_a_paper_practice_once_each()
    {
        const string payload = """{"Items":[{"Id":"№1","Text":"«Я не справлюсь»"},{"Id":"№2","Text":"«Я не справлюсь»"},{"Id":"№3","Text":"  "}]}""";

        IReadOnlyList<PracticeAnswer> answers = PracticeAnswers.FromPayload(payload, [new TechniqueEntrySeed("Мысль")]);

        Assert.Equal([new PracticeAnswer("Мысль", "«Я не справлюсь»")], answers);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("""{"Other":1}""")]
    public void FromPayload_without_answers_is_empty(string? payload)
    {
        Assert.Empty(PracticeAnswers.FromPayload(payload, ThoughtRecord));
    }

    [Fact]
    public void TypedParts_splits_the_answer_and_leaves_out_offered_chips_and_sentences()
    {
        IReadOnlyList<string> parts = PracticeAnswers.TypedParts(
            "кот, Компьютер; чашка чая.\nя сижу дома и думаю о завтрашнем дне, х",
            ["Компьютер"]);

        Assert.Equal(["Кот", "Чашка чая"], parts);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" , ; ")]
    public void TypedParts_of_nothing_is_empty(string? answer)
    {
        Assert.Empty(PracticeAnswers.TypedParts(answer, []));
    }

    [Fact]
    public void OwnChips_appear_after_repeated_typing_for_the_same_question_only()
    {
        Dictionary<string, int> counts = new()
        {
            [PracticeAnswers.OwnChipKey("Вижу", "Кот")] = 3,
            [PracticeAnswers.OwnChipKey("Вижу", "Окно")] = 2,
            [PracticeAnswers.OwnChipKey("Вижу", "Лампа")] = 1,
            [PracticeAnswers.OwnChipKey("Слышу", "Музыка")] = 5,
            ["Компьютер"] = 9
        };

        Assert.Equal(["Кот", "Окно"], PracticeAnswers.OwnChips("Вижу", counts, []));
        Assert.Equal(["Окно"], PracticeAnswers.OwnChips("Вижу", counts, ["кот"]));
        Assert.Empty(PracticeAnswers.OwnChips("Чувствую", counts, []));
    }

    [Fact]
    public void OwnChips_are_capped()
    {
        Dictionary<string, int> counts = Enumerable.Range(0, 10).ToDictionary(i => PracticeAnswers.OwnChipKey("Вижу", $"Вещь {i}"), _ => 3);

        Assert.Equal(PracticeAnswers.OwnChipMax, PracticeAnswers.OwnChips("Вижу", counts, []).Count);
    }
}
