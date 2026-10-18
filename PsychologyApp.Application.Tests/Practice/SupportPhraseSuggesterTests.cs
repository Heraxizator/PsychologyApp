using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Practice;

public class SupportPhraseSuggesterTests
{
    private sealed class FakeModel(string? reply, bool available = true, bool fail = false) : ILanguageModel
    {
        public int Calls { get; private set; }

        public bool IsAvailable => available;

        public Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default)
        {
            Calls++;
            return fail ? throw new InvalidOperationException("model crashed") : Task.FromResult(reply);
        }
    }

    private static SupportPhraseSuggester Create(ILanguageModel? model = null) =>
        new(model ?? new NullLanguageModel(), new KeywordCrisisDetector());

    [Theory]
    [InlineData(EntryFieldRole.None)]
    [InlineData(EntryFieldRole.Problem)]
    [InlineData(EntryFieldRole.Emotion)]
    public async Task Other_fields_get_no_phrases(EntryFieldRole role)
    {
        Assert.Empty(await Create().SuggestAsync(role, "что угодно", english: false));
    }

    [Fact]
    public async Task A_reframe_fits_the_trap_in_the_thought()
    {
        IReadOnlyList<string> phrases = await Create().SuggestAsync(EntryFieldRole.Reframe, "Они подумают, что я некомпетентен", english: false);

        Assert.Equal(SupportPhraseSuggester.MaxSuggestions, phrases.Count);
        Assert.Contains(phrases, p => p.Contains("что думают другие", StringComparison.Ordinal));
    }

    [Fact]
    public async Task A_thought_without_a_known_trap_still_gets_general_balanced_phrases()
    {
        IReadOnlyList<string> phrases = await Create().SuggestAsync(EntryFieldRole.Reframe, "Мне грустно", english: true);

        Assert.Contains("This is a thought, not a fact", phrases);
    }

    [Theory]
    [InlineData(EntryFieldRole.Kindness, false, "Ошибаться — по-человечески")]
    [InlineData(EntryFieldRole.SelfKindness, false, "Я делаю всё, что могу")]
    [InlineData(EntryFieldRole.SelfKindness, true, "I'm doing the best I can")]
    public async Task Kind_words_are_offered_in_the_language_of_the_app(EntryFieldRole role, bool english, string expected)
    {
        IReadOnlyList<string> phrases = await Create().SuggestAsync(role, "опять всё испортил", english);

        Assert.Contains(expected, phrases);
    }

    [Fact]
    public async Task Nothing_is_offered_when_the_context_reads_as_a_crisis()
    {
        Assert.Empty(await Create().SuggestAsync(EntryFieldRole.SelfKindness, "не хочу жить", english: false));
    }

    [Fact]
    public async Task Model_variants_that_pass_the_guard_come_first()
    {
        FakeModel model = new("- Я не знаю наверняка, что обо мне подумают коллеги\n- Попробуй просто успокоиться и не волнуйся");

        IReadOnlyList<string> phrases = await Create(model).SuggestAsync(EntryFieldRole.Reframe, "Коллеги меня осудят", english: false);

        Assert.Equal(1, model.Calls);
        Assert.Equal("Я не знаю наверняка, что обо мне подумают коллеги.", phrases[0].TrimEnd('.') + ".");
        Assert.DoesNotContain(phrases, p => p.Contains("Попробуй", StringComparison.Ordinal));
    }

    [Fact]
    public async Task A_failing_or_missing_model_falls_back_to_reviewed_phrases()
    {
        IReadOnlyList<string> failed = await Create(new FakeModel(null, fail: true)).SuggestAsync(EntryFieldRole.Reframe, "Всё пропало", english: false);
        FakeModel missing = new("anything", available: false);
        IReadOnlyList<string> absent = await Create(missing).SuggestAsync(EntryFieldRole.Reframe, "Всё пропало", english: false);

        Assert.NotEmpty(failed);
        Assert.NotEmpty(absent);
        Assert.Equal(0, missing.Calls);
    }

    [Fact]
    public async Task Kind_words_to_a_friend_never_ask_the_model()
    {
        FakeModel model = new("Ты молодец");

        await Create(model).SuggestAsync(EntryFieldRole.Kindness, "ошибся на работе", english: false);

        Assert.Equal(0, model.Calls);
    }
}
