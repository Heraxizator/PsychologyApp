using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

public class LexiconSituationAnalyzerTests
{
    private readonly LexiconSituationAnalyzer _analyzer = new();

    [Theory]
    [InlineData("У меня паническая атака, не могу дышать, сердце колотится", CompanionEmotion.Panic)]
    [InlineData("Очень тревожно из-за завтрашнего собеседования, боюсь провалиться", CompanionEmotion.Anxiety)]
    [InlineData("Не могу перестать думать об этом разговоре, мысли крутятся по кругу", CompanionEmotion.Overthinking)]
    [InlineData("Меня бесит начальник, так злюсь что руки трясутся", CompanionEmotion.Anger)]
    [InlineData("Мне так обидно, что меня проигнорировали и не ценят", CompanionEmotion.Resentment)]
    [InlineData("Чувствую вину и стыд за то, что подвела команду", CompanionEmotion.Guilt)]
    [InlineData("Грустно и пусто, ничего не радует", CompanionEmotion.Sadness)]
    [InlineData("Я очень устал, нет сил, полное выгорание", CompanionEmotion.Exhaustion)]
    [InlineData("Мне одиноко, никому не нужна", CompanionEmotion.Loneliness)]
    [InlineData("Опять откладываю отчёт, не могу начать", CompanionEmotion.Procrastination)]
    [InlineData("I feel so anxious and worried about tomorrow", CompanionEmotion.Anxiety)]
    [InlineData("I'm furious at my brother, I hate this", CompanionEmotion.Anger)]
    [InlineData("I feel exhausted and burned out, no energy", CompanionEmotion.Exhaustion)]
    [InlineData("I keep overthinking what I said, can't stop thinking about it", CompanionEmotion.Overthinking)]
    public void Recognises_dominant_state(string text, CompanionEmotion expected)
    {
        Assert.Equal(expected, _analyzer.Analyze(text).Emotion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Сегодня был дождь и я купил хлеб")]
    [InlineData("Ну как-то так, не знаю")]
    public void Unrecognised_text_is_unknown(string text)
    {
        Assert.Equal(CompanionEmotion.Unknown, _analyzer.Analyze(text).Emotion);
    }

    [Theory]
    [InlineData("Я совсем не злюсь на него")]
    [InlineData("Мне не грустно, всё нормально")]
    public void Simple_negation_is_respected(string text)
    {
        Assert.Equal(CompanionEmotion.Unknown, _analyzer.Analyze(text).Emotion);
    }

    [Fact]
    public void Detects_theme_body_and_intensity()
    {
        SituationAnalysis result = _analyzer.Analyze("Очень тревожно из-за начальника, давит в груди");

        Assert.Equal(CompanionEmotion.Anxiety, result.Emotion);
        Assert.Contains(CompanionTheme.Work, result.Themes);
        Assert.True(result.HasBodySymptoms);
        Assert.True(result.IsIntense);
    }

    [Fact]
    public void Anxiety_with_body_symptoms_leans_towards_panic_when_strong()
    {
        SituationAnalysis result = _analyzer.Analyze("Тревога такая, что сердце выскакивает и трясёт");

        Assert.Equal(CompanionEmotion.Panic, result.Emotion);
    }

    [Fact]
    public void Yo_is_treated_like_ye()
    {
        Assert.Equal(CompanionEmotion.Sadness, _analyzer.Analyze("мне тяжело на душе").Emotion);
        Assert.Equal(CompanionEmotion.Anger, _analyzer.Analyze("Всё ЗЛОСТЬ").Emotion);
    }
}

public class CompanionReplyGuardTests
{
    [Fact]
    public void Clean_russian_reply_passes_untouched() =>
        Assert.Equal(
            "Похоже, вам тяжело. Это понятная реакция.",
            CompanionReplyGuard.Sanitize("Похоже, вам тяжело. Это понятная реакция.", english: false));

    [Fact]
    public void Formatting_and_questions_are_stripped()
    {
        string? result = CompanionReplyGuard.Sanitize("**Слышу, как вам непросто.**\n- Это очень выматывает.\nЧто вы чувствуете?", english: false);

        Assert.Equal("Слышу, как вам непросто. Это очень выматывает.", result);
    }

    [Fact]
    public void Reply_is_capped_at_three_sentences()
    {
        string? result = CompanionReplyGuard.Sanitize("Слышу вас очень хорошо. Это правда непросто. Вы много выдержали. Дальше будет проще. Отдыхайте.", english: false);

        Assert.Equal("Слышу вас очень хорошо. Это правда непросто. Вы много выдержали.", result);
    }

    [Theory]
    [InlineData("У вас депрессия, вам нужно лечение.")]
    [InlineData("Попробуйте принимайте антидепрессанты.")]
    [InlineData("Это похоже на тревожное расстройство.")]
    [InlineData("Как ИИ, я не могу вам помочь.")]
    [InlineData("Подробнее на http://example.com")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Только вопрос?")]
    public void Unsafe_or_empty_replies_are_rejected(string raw)
    {
        Assert.Null(CompanionReplyGuard.Sanitize(raw, english: false));
    }


    [Theory]
    [InlineData("У тебя сейчас очень тяжело на душе, это понятно.")]
    [InlineData("Похоже, вам тяжело, и вы чувствуешь усталость.")]
    [InlineData("Слышу, как вам тревожно. Не бойтесь, это пройдёт.")]
    [InlineData("Слышу, как вам тревожно. Попробуйте немного отдохнуть.")]
    [InlineData("Слышу вас, и мне тоже очень тяжело от этого.")]
    [InlineData("Всё будет хорошо, вы справитесь с этой встречей.")]
    public void Advice_dismissals_and_informal_register_are_rejected(string raw)
    {
        Assert.Null(CompanionReplyGuard.Sanitize(raw, english: false));
    }

    [Fact]
    public void Tokenizer_artifacts_and_repeated_punctuation_are_cleaned()
    {
        string? result = CompanionReplyGuard.Sanitize("Слышу, как вам тяжело перед встречей.▁▁Это очень выматывает..", english: false);

        Assert.Equal("Слышу, как вам тяжело перед встречей. Это очень выматывает.", result);
    }

    [Fact]
    public void Too_short_replies_are_rejected()
    {
        Assert.Null(CompanionReplyGuard.Sanitize("Понимаю вас.", english: false));
    }

    [Fact]
    public void Reply_must_be_about_what_the_person_wrote_when_the_text_is_given()
    {
        const string user = "Завтра важная презентация, я очень тревожусь";

        Assert.NotNull(CompanionReplyGuard.Sanitize("Слышу, как вам тревожно перед презентацией. Это очень выматывает.", english: false, user));
        Assert.Null(CompanionReplyGuard.Sanitize("Солнце в комнате кажется таким тусклым и серым сегодня.", english: false, user));
    }

    [Fact]
    public void Wrong_language_is_rejected()
    {
        Assert.Null(CompanionReplyGuard.Sanitize("I hear that this is hard for you.", english: false));
        Assert.Null(CompanionReplyGuard.Sanitize("Слышу, что вам тяжело.", english: true));
        Assert.NotNull(CompanionReplyGuard.Sanitize("I hear that this is hard for you.", english: true));
    }

    [Fact]
    public void Medication_talk_is_rejected_in_english()
    {
        Assert.Null(CompanionReplyGuard.Sanitize("You should ask about medication for this.", english: true));
    }
}

public class CompanionSessionTests
{
    private sealed class FakeModel(Func<LlmRequest, CancellationToken, Task<string?>> reply, bool available = true) : ILocalLanguageModel
    {
        public List<LlmRequest> Requests { get; } = [];

        public bool IsAvailable => available;

        public Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return reply(request, cancellationToken);
        }
    }

    private static CompanionSession Create(ILocalLanguageModel? model = null, bool english = false, TimeSpan? timeout = null) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), model ?? new NullLanguageModel(), english, new Random(1), timeout);

    [Fact]
    public async Task Start_greets_and_asks_for_free_text()
    {
        CompanionSession session = Create();

        ConversationTurn turn = await session.StartAsync();

        Assert.Equal(2, turn.BotMessages.Count);
        Assert.Equal(ConversationInputKind.Text, turn.Prompt!.Kind);
    }

    [Fact]
    public async Task Panic_text_offers_grounding_first_then_starts_it()
    {
        CompanionSession session = Create();
        await session.StartAsync();

        ConversationTurn offer = await session.SubmitTextAsync("Паническая атака, не могу дышать, сердце колотится");

        Assert.Contains(offer.BotMessages, m => m.Contains("Заземление 5-4-3-2-1"));
        Assert.Equal(ConversationInputKind.Choice, offer.Prompt!.Kind);
        Assert.StartsWith("Начнём: Заземление", offer.Prompt.Choices[0]);

        ConversationTurn started = await session.SubmitChoiceAsync(0);

        Assert.Equal(ConversationStatus.Completed, started.Status);
        Assert.Equal(new DialogueAction(DialogueActionKind.StartTechnique, TechniqueId.Grounding), started.Action);
    }

    [Fact]
    public async Task Second_option_launches_the_alternative_practice()
    {
        CompanionSession session = Create();
        await session.StartAsync();
        await session.SubmitTextAsync("Меня бесит начальник, так злюсь");

        ConversationTurn started = await session.SubmitChoiceAsync(1);

        Assert.Equal(TechniqueId.Breathing, started.Action!.TechniqueId);
    }

    [Fact]
    public async Task Unrecognised_text_asks_to_clarify_and_the_choice_drives_the_offer()
    {
        CompanionSession session = Create();
        await session.StartAsync();

        ConversationTurn clarify = await session.SubmitTextAsync("ну как-то всё не так");
        Assert.Equal(ConversationInputKind.Choice, clarify.Prompt!.Kind);
        Assert.Equal("Пока не знаю", clarify.Prompt.Choices[^1]);

        ConversationTurn offer = await session.SubmitChoiceAsync(3); // Злость
        Assert.Contains(offer.BotMessages, m => m.Contains("Позиция наблюдателя"));
    }

    [Fact]
    public async Task Not_sure_yet_falls_back_to_calming_default()
    {
        CompanionSession session = Create();
        await session.StartAsync();
        ConversationTurn clarify = await session.SubmitTextAsync("ну как-то всё не так");

        ConversationTurn offer = await session.SubmitChoiceAsync(clarify.Prompt!.Choices.Count - 1);

        Assert.Contains(offer.BotMessages, m => m.Contains("Квадратное дыхание"));
    }

    [Fact]
    public async Task Venting_continues_the_conversation_and_is_limited()
    {
        CompanionSession session = Create();
        await session.StartAsync();
        ConversationTurn offer = await session.SubmitTextAsync("Мне очень тревожно из-за работы");

        for (int round = 0; round < CompanionSession.MaxVentRounds; round++)
        {
            Assert.Contains(CompanionContent.MoreLabel(false), offer.Prompt!.Choices);
            ConversationTurn vent = await session.SubmitChoiceAsync(offer.Prompt.Choices.ToList().IndexOf(CompanionContent.MoreLabel(false)));
            Assert.Equal(ConversationInputKind.Text, vent.Prompt!.Kind);
            offer = await session.SubmitTextAsync("Ещё я боюсь, что меня уволят");
        }

        Assert.DoesNotContain(CompanionContent.MoreLabel(false), offer.Prompt!.Choices);
    }

    [Theory]
    [InlineData("Мне кажется, я не хочу жить")]
    [InlineData("i want to end my life")]
    public async Task Crisis_text_never_reaches_the_model_and_opens_the_crisis_hub(string text)
    {
        FakeModel model = new((_, _) => Task.FromResult<string?>("Слышу вас."));
        CompanionSession session = Create(model);
        await session.StartAsync();

        ConversationTurn turn = await session.SubmitTextAsync(text);

        Assert.Equal(ConversationStatus.Interrupted, turn.Status);
        Assert.Equal(DialogueActionKind.OpenCrisisHub, turn.Action!.Kind);
        Assert.Empty(model.Requests);
    }

    [Fact]
    public async Task Model_reply_is_used_when_it_passes_the_guard()
    {
        FakeModel model = new((_, _) => Task.FromResult<string?>("Слышу, как вам тяжело из-за этой встречи. Это очень выматывает."));
        CompanionSession session = Create(model);
        await session.StartAsync();

        ConversationTurn offer = await session.SubmitTextAsync("Очень тревожно из-за завтрашней встречи");

        Assert.True(session.LastReplyFromModel);
        Assert.Equal("Слышу, как вам тяжело из-за этой встречи. Это очень выматывает.", offer.BotMessages[0]);
        Assert.Contains("Очень тревожно из-за завтрашней встречи", model.Requests[0].Messages[^1].Content);
        Assert.Contains("Не задавай вопросов", model.Requests[0].SystemPrompt);
    }

    [Theory]
    [InlineData("У вас депрессия, вам нужны таблетки.")]
    [InlineData("Attention: this is English.")]
    [InlineData("")]
    public async Task Rejected_model_reply_falls_back_to_scripted_text(string modelReply)
    {
        FakeModel model = new((_, _) => Task.FromResult<string?>(modelReply));
        CompanionSession session = Create(model);
        await session.StartAsync();

        ConversationTurn offer = await session.SubmitTextAsync("Очень тревожно из-за завтрашней встречи");

        Assert.False(session.LastReplyFromModel);
        Assert.DoesNotContain("таблетки", offer.BotMessages[0]);
        Assert.Contains("тревож", offer.BotMessages[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Throwing_model_falls_back_to_scripted_text()
    {
        FakeModel model = new((_, _) => throw new InvalidOperationException("model crashed"));
        CompanionSession session = Create(model);
        await session.StartAsync();

        ConversationTurn offer = await session.SubmitTextAsync("Очень тревожно из-за завтрашней встречи");

        Assert.False(session.LastReplyFromModel);
        Assert.Equal(ConversationInputKind.Choice, offer.Prompt!.Kind);
    }

    [Fact]
    public async Task Slow_model_times_out_and_falls_back()
    {
        FakeModel model = new(async (_, token) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30), token);
            return "Слишком поздно.";
        });
        CompanionSession session = Create(model, timeout: TimeSpan.FromMilliseconds(50));
        await session.StartAsync();

        ConversationTurn offer = await session.SubmitTextAsync("Очень тревожно из-за завтрашней встречи");

        Assert.False(session.LastReplyFromModel);
        Assert.Equal(ConversationInputKind.Choice, offer.Prompt!.Kind);
    }

    [Fact]
    public async Task Unavailable_model_is_never_called()
    {
        FakeModel model = new((_, _) => Task.FromResult<string?>("x"), available: false);
        CompanionSession session = Create(model);
        await session.StartAsync();

        await session.SubmitTextAsync("Очень тревожно");

        Assert.Empty(model.Requests);
    }

    [Fact]
    public async Task Caller_cancellation_is_not_swallowed()
    {
        FakeModel model = new(async (_, token) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30), token);
            return null;
        });
        CompanionSession session = Create(model);
        await session.StartAsync();
        using CancellationTokenSource cts = new(50);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => session.SubmitTextAsync("Очень тревожно", cts.Token));
    }

    [Fact]
    public async Task English_session_speaks_english_end_to_end()
    {
        CompanionSession session = Create(english: true);
        ConversationTurn start = await session.StartAsync();
        Assert.Contains("device", start.BotMessages[1]);

        ConversationTurn offer = await session.SubmitTextAsync("I feel so anxious, my heart is racing");

        Assert.Contains(offer.BotMessages, m => m.Contains("grounding"));
        Assert.StartsWith("Let's start", offer.Prompt!.Choices[0]);
    }

    [Fact]
    public void Prompt_keeps_only_recent_history_and_includes_hints()
    {
        List<LlmMessage> turns = [];
        for (int i = 0; i < 10; i++)
        {
            turns.Add(new LlmMessage(i % 2 == 0 ? LlmRole.User : LlmRole.Assistant, $"msg {i}"));
        }

        LlmRequest request = CompanionPromptBuilder.Build(
            english: false,
            turns,
            new SituationAnalysis(CompanionEmotion.Anxiety, 0.7, false, false, [CompanionTheme.Work]));

        Assert.Equal(4 + 6, request.Messages.Count); // two example exchanges + the last six turns
        Assert.Equal("msg 9", request.Messages[^1].Content);
        Assert.Contains("тревога", request.SystemPrompt);
        Assert.Contains("работа", request.SystemPrompt);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Titles_used_by_the_companion_match_the_technique_catalog(bool english)
    {
        ITechniqueCatalogService catalog = new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => english ? "en" : "ru"));
        TechniqueId[] ids = Enum.GetValues<TechniqueId>()
            .Where(id => Enum.GetValues<CompanionEmotion>().Any(e => TechniqueSuggester.Suggest(new SituationAnalysis(e, 1, false, false, [])).Contains(id))
                || id == TechniqueId.Grounding)
            .ToArray();

        foreach (TechniqueId id in ids)
        {
            Assert.Equal((await catalog.GetAsync(id)).PageName, CompanionContent.TechniqueTitle(id, english));
            Assert.False(string.IsNullOrWhiteSpace(CompanionContent.TechniqueWhy(id, english)));
        }
    }
}
