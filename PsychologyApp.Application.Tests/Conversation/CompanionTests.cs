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
    public void Reasoning_blocks_and_special_tokens_are_removed()
    {
        string? result = CompanionReplyGuard.Sanitize(
            "<think>\nОна расстроена, надо ответить мягко.\n</think>\n\nСлышу, как вам тяжело перед встречей. Это очень выматывает.<|im_end|>",
            english: false);

        Assert.Equal("Слышу, как вам тяжело перед встречей. Это очень выматывает.", result);
    }

    [Fact]
    public void Unfinished_reasoning_block_rejects_the_reply()
    {
        Assert.Null(CompanionReplyGuard.Sanitize("<think>\nНадо подумать, что ответить этому человеку и как именно", english: false));
    }


    [Theory]
    [InlineData("Слышу, как вам тяжело. Гrief — это чувство, которое сложно нести.")]
    [InlineData("Видите, как тяжело, когда в семье nobody делает своё.")]
    [InlineData("Слышу вас очень хорошо. Просто держите себя в руках сейчас.")]
    public void Code_switching_and_disguised_advice_are_rejected_in_russian(string raw)
    {
        Assert.Null(CompanionReplyGuard.Sanitize(raw, english: false));
    }

    [Fact]
    public void Latin_words_are_fine_in_english_replies()
    {
        Assert.NotNull(CompanionReplyGuard.Sanitize("It sounds like the exam is weighing heavily on you right now.", english: true));
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

public class CompanionContentCatalogTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Titles_used_by_the_companion_match_the_technique_catalog(bool english)
    {
        ITechniqueCatalogService catalog = new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => english ? "en" : "ru"));
        TechniqueId[] ids = Enum.GetValues<TechniqueId>()
            .Where(id => id == TechniqueId.Grounding
                || Enum.GetValues<CompanionEmotion>().Any(e => TechniqueSuggester.Suggest(new SituationAnalysis(e, 1, false, false, [])).Contains(id)))
            .ToArray();

        foreach (TechniqueId id in ids)
        {
            Assert.Equal((await catalog.GetAsync(id)).PageName, CompanionContent.TechniqueTitle(id, english));
            Assert.False(string.IsNullOrWhiteSpace(CompanionContent.TechniqueWhy(id, english)));
        }
    }
}
