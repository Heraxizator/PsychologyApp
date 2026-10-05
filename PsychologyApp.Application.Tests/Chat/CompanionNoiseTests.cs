using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>Input with nothing to understand ("1111", "asdfgh", "???") must not be answered as if it were a story.</summary>
public class CompanionNoiseTests
{
    private const string FakeEmpathy = "Спасибо, что рассказали";

    private static CompanionDialogue Create(bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3));

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    [Theory]
    [InlineData("1111")]
    [InlineData("11")]
    [InlineData("123456")]
    [InlineData("12 34")]
    public void Bare_digits_are_not_a_story(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.DoesNotContain(FakeEmpathy, string.Join(' ', reply.Messages));
        Assert.Contains("цифр", reply.Messages[0]);
        Assert.Equal(0, reply.State.Turns);
        Assert.Empty(reply.State.RecentTexts);
        Assert.Equal(1, reply.State.UnclearStreak);
    }

    [Theory]
    [InlineData("asdfgh")]
    [InlineData("qwerty")]
    [InlineData("йцукен")]
    [InlineData("фыва")]
    [InlineData("аааааа")]
    [InlineData("sdfsdf")]
    [InlineData("ghjkl")]
    public void Key_mashing_is_not_a_story(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.DoesNotContain(FakeEmpathy, string.Join(' ', reply.Messages));
        Assert.Equal(0, reply.State.Turns);
        Assert.Empty(reply.State.RecentTexts);
    }

    [Fact]
    public void Noise_does_not_disturb_what_the_companion_already_knows()
    {
        CompanionDialogue d = Create();
        CompanionState known = Say(d, new CompanionState(), "мне очень тревожно из-за работы").State;

        CompanionReply reply = Say(d, known, "1111");

        Assert.Equal(known.Emotion, reply.State.Emotion);
        Assert.Equal(known.Theme, reply.State.Theme);
        Assert.Equal(known.Turns, reply.State.Turns);
        Assert.Equal(known.RecentTexts, reply.State.RecentTexts);
        Assert.Equal(known.FirstQuote, reply.State.FirstQuote);
    }

    [Fact]
    public void Repeated_noise_offers_the_feelings_to_pick_from_and_a_real_message_resets_the_count()
    {
        CompanionDialogue d = Create();
        CompanionState state = new();

        state = Say(d, state, "1111").State;
        CompanionReply second = Say(d, state, "asdfgh");

        Assert.Equal(2, second.State.UnclearStreak);
        Assert.Contains(second.QuickReplies, q => q.Kind == ChatQuickReplyKinds.Emotion);

        CompanionReply real = Say(d, second.State, "мне очень тревожно из-за работы");

        Assert.Equal(0, real.State.UnclearStreak);
    }

    [Fact]
    public void Digits_while_the_tension_question_is_open_explain_the_scale()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { QuestionPending = true, LastQuestion = "Насколько сильное напряжение сейчас, от 0 до 10?" };

        CompanionReply reply = Say(d, state, "1111");

        Assert.Contains("0 — совсем спокойно", reply.Messages[0]);
        Assert.Equal(11, reply.QuickReplies.Count);
        Assert.All(reply.QuickReplies, q => Assert.Equal(ChatQuickReplyKinds.Rating, q.Kind));
    }

    [Fact]
    public void A_typed_rating_still_answers_the_tension_question()
    {
        CompanionDialogue d = Create();
        CompanionState state = new()
        {
            QuestionPending = true,
            LastQuestion = "Насколько сильное напряжение сейчас, от 0 до 10?",
            Emotion = "Anxiety",
            Turns = 2
        };

        CompanionReply reply = Say(d, state, "7");

        Assert.Equal(7, reply.State.FirstIntensity);
    }

    [Fact]
    public void A_short_number_answers_an_open_question_but_1111_does_not()
    {
        CompanionDialogue d = Create();
        CompanionState asked = new() { QuestionPending = true, LastQuestion = "Как часто это бывает?" };

        CompanionReply answer = Say(d, asked, "5");
        CompanionReply noise = Say(d, asked, "1111");

        Assert.Equal(1, answer.State.Turns);
        Assert.Equal(0, noise.State.Turns);
    }

    [Fact]
    public void A_lone_number_with_no_question_open_asks_what_it_is_about()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "7");

        Assert.StartsWith("К чему это число?", reply.Messages[0]);
        Assert.DoesNotContain(FakeEmpathy, reply.Messages[0]);
    }

    [Fact]
    public void Question_marks_alone_mean_what_and_are_not_laughter()
    {
        CompanionDialogue d = Create();
        CompanionState asked = Say(d, new CompanionState(), "мне очень тревожно из-за работы").State;

        CompanionReply reply = Say(d, asked, "???");

        Assert.DoesNotContain("улыб", string.Join(' ', reply.Messages));
        Assert.Matches("^(Простите, спрошу ещё раз|Повторю вопрос)", reply.Messages[0]);
    }

    [Fact]
    public void Exclamation_marks_alone_are_strong_feeling_not_a_smile()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "!!!!");

        Assert.DoesNotContain("улыб", reply.Messages[0]);
        Assert.Contains("случилось", reply.Messages[0] + string.Join(' ', reply.Messages));
    }

    [Theory]
    [InlineData("ghbdtn")]
    [InlineData("Ghbdtn")]
    public void Russian_typed_on_the_english_layout_is_read_as_russian(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.Matches("Как вы сегодня|Что у вас на душе", reply.Messages[0]);
    }

    [Fact]
    public void A_feeling_typed_on_the_wrong_layout_is_recognised()
    {
        // "nhtdjuf" is "тревога" typed with the English layout.
        CompanionReply reply = Say(Create(), new CompanionState(), "nhtdjuf");

        Assert.Equal(CompanionEmotion.Anxiety, reply.Emotion);
    }

    [Fact]
    public void English_words_in_a_russian_chat_stay_english()
    {
        Assert.Matches("Как вы сегодня|Что у вас на душе", Say(Create(), new CompanionState(), "hello").Messages[0]);
        Assert.DoesNotContain("цифр", Say(Create(), new CompanionState(), "tired").Messages[0]);
    }

    [Fact]
    public void The_layout_is_not_touched_in_an_english_chat()
    {
        CompanionReply reply = Say(Create(english: true), new CompanionState(), "ghbdtn");

        Assert.DoesNotContain("Как вы", reply.Messages[0]);
    }

    [Fact]
    public void English_noise_gets_an_english_answer()
    {
        CompanionReply reply = Say(Create(english: true), new CompanionState(), "1111");

        Assert.Contains("digits", reply.Messages[0]);
    }

    [Theory]
    [InlineData("тест")]
    [InlineData("test")]
    [InlineData("ку-ку")]
    public void One_or_two_empty_words_are_not_thanked_as_a_story(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.DoesNotContain(FakeEmpathy, string.Join(' ', reply.Messages));
        Assert.Equal(1, reply.State.UnclearStreak);
        Assert.Equal(0, reply.State.Turns);
    }

    [Theory]
    [InlineData("работа")]
    [InlineData("тревога")]
    [InlineData("начальник")]
    public void One_word_with_a_topic_a_feeling_or_a_person_is_still_a_statement(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.Equal(1, reply.State.Turns);
        Assert.Equal(0, reply.State.UnclearStreak);
    }

    [Fact]
    public void An_unknown_word_answering_an_open_question_is_an_answer()
    {
        CompanionState asked = new() { QuestionPending = true, LastQuestion = "Что в этой ситуации самое тяжёлое?" };

        CompanionReply reply = Say(Create(), asked, "тишина");

        Assert.Equal(1, reply.State.Turns);
    }

    [Theory]
    [InlineData("ну")]
    [InlineData("эээ")]
    [InlineData("ммм")]
    [InlineData("hmm")]
    public void Hesitation_gets_a_backchannel(string text)
    {
        CompanionReply reply = Say(Create(), new CompanionState(), text);

        Assert.DoesNotContain(FakeEmpathy, string.Join(' ', reply.Messages));
        Assert.Equal(0, reply.State.UnclearStreak);
    }

    [Theory]
    [InlineData("привет")]
    [InlineData("работа")]
    [InlineData("тревога")]
    [InlineData("мама")]
    [InlineData("встреча")]
    [InlineData("стресс")]
    [InlineData("взгляд")]
    [InlineData("здравствуйте")]
    [InlineData("ссора")]
    [InlineData("птср")]
    [InlineData("аня")]
    [InlineData("нет")]
    [InlineData("все плохо")]
    [InlineData("hello")]
    [InlineData("tired")]
    [InlineData("work")]
    [InlineData("rhythm")]
    public void Real_words_are_never_noise(string word) =>
        Assert.False(UtteranceClassifier.IsNoise(word));

    [Fact]
    public void Unclear_streak_survives_a_restart()
    {
        CompanionState state = new() { UnclearStreak = 2 };

        Assert.Equal(2, CompanionState.Deserialize(state.Serialize()).UnclearStreak);
        Assert.Equal(0, CompanionState.Deserialize("{\"turns\":3}").UnclearStreak);
    }
}
