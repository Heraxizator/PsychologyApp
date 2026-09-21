using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class SmallTalkClassifierTests
{
    [Theory]
    [InlineData("ок", Utterance.Acknowledge)]
    [InlineData("Понятно", Utterance.Acknowledge)]
    [InlineData("ну ладно", Utterance.Acknowledge)]
    [InlineData("i see", Utterance.Acknowledge)]
    [InlineData("ахаха", Utterance.Reaction)]
    [InlineData("lol", Utterance.Reaction)]
    [InlineData(")))", Utterance.Reaction)]
    [InlineData("...", Utterance.Reaction)]
    [InlineData("😢", Utterance.Reaction)]
    [InlineData("Мне стало легче", Utterance.SharesGoodNews)]
    [InlineData("получилось!", Utterance.SharesGoodNews)]
    [InlineData("хорошо", Utterance.SharesGoodNews)]
    [InlineData("i feel better", Utterance.SharesGoodNews)]
    [InlineData("Как дела?", Utterance.AsksHowAreYou)]
    [InlineData("как ты?", Utterance.AsksHowAreYou)]
    [InlineData("how are you", Utterance.AsksHowAreYou)]
    [InlineData("Как тебя зовут?", Utterance.AsksName)]
    [InlineData("what is your name", Utterance.AsksName)]
    [InlineData("Меня зовут Аня", Utterance.IntroducesSelf)]
    [InlineData("my name is Anna", Utterance.IntroducesSelf)]
    [InlineData("Что ты умеешь?", Utterance.AsksCapabilities)]
    [InlineData("what can you do", Utterance.AsksCapabilities)]
    [InlineData("повтори", Utterance.AsksToRepeat)]
    [InlineData("Не поняла", Utterance.AsksToRepeat)]
    [InlineData("что?", Utterance.AsksToRepeat)]
    [InlineData("давай другой вопрос", Utterance.SkipsQuestion)]
    [InlineData("пропустим", Utterance.SkipsQuestion)]
    [InlineData("skip", Utterance.SkipsQuestion)]
    public void Everyday_moves_are_recognised(string text, Utterance expected) =>
        Assert.Equal(expected, UtteranceClassifier.Classify(text, hasFeeling: false));

    [Theory]
    [InlineData("Мне не стало легче, всё так же плохо")]
    [InlineData("Как ты думаешь, стоит ли мне уйти с работы")]
    [InlineData("Мне хорошо, но всё равно тревожно")]
    [InlineData("Не получилось помириться с мамой")]
    [InlineData("i feel better but still anxious")]
    public void Ordinary_sentences_are_not_mistaken_for_small_talk(string text) =>
        Assert.Equal(Utterance.Statement, UtteranceClassifier.Classify(text, hasFeeling: false));

    [Theory]
    [InlineData("Меня зовут Аня", "Аня")]
    [InlineData("привет, меня зовут ВЕРА.", "Вера")]
    [InlineData("my name is anna", "Anna")]
    [InlineData("Зови меня Сашей", "Сашей")]
    public void A_name_is_taken_from_the_introduction(string text, string expected)
    {
        Assert.True(UtteranceClassifier.TryExtractName(text, out string name));
        Assert.Equal(expected, name);
    }

    [Theory]
    [InlineData("Меня зовут")]
    [InlineData("меня зовут 123")]
    [InlineData("привет")]
    public void No_name_is_invented(string text) =>
        Assert.False(UtteranceClassifier.TryExtractName(text, out _));

    [Fact]
    public void Sad_faces_are_told_from_smiles()
    {
        Assert.True(UtteranceClassifier.IsSadReaction("😭"));
        Assert.False(UtteranceClassifier.IsSadReaction(")))"));
    }
}

public class CompanionRealismTests
{
    private static CompanionDialogue Create(bool english = false, TimeProvider? time = null, int seed = 3) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(seed), time);

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    private static TimeProvider At(int hour) => new FixedTime(new DateTimeOffset(2026, 6, 1, hour, 0, 0, TimeSpan.Zero));

    [Fact]
    public void A_backchannel_does_not_derail_the_conversation()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех").State;

        CompanionReply reply = Say(d, state, "ясно");

        Assert.Single(reply.Messages);
        Assert.Equal(state.Emotion, reply.State.Emotion);
        Assert.Empty(reply.QuickReplies);
    }

    [Fact]
    public void A_backchannel_after_a_question_says_it_can_be_skipped()
    {
        CompanionDialogue d = Create();
        CompanionState asked = Say(d, new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех").State;
        Assert.True(asked.QuestionPending);

        CompanionReply reply = Say(d, asked, "ок");

        Assert.Contains("пропустить", reply.Messages[0]);
    }

    [Fact]
    public void Good_news_is_celebrated_not_analysed()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 4, Emotion = "Anxiety" }, "Мне стало легче");

        Assert.Single(reply.Messages);
        Assert.EndsWith("?", reply.Messages[0]);
        Assert.DoesNotContain(reply.QuickReplies, c => c.Kind == ChatQuickReplyKinds.Practice);
    }

    [Fact]
    public void A_smile_gets_a_light_reply_and_tears_get_a_gentle_question()
    {
        CompanionDialogue d = Create();

        CompanionReply smile = Say(d, new CompanionState(), "ахаха");
        CompanionReply tears = Say(d, new CompanionState(), "😭");

        Assert.DoesNotContain("нелегко", smile.Messages[0]);
        Assert.Contains(tears.Messages[0], new[] { "Вижу, что сейчас нелегко. Хотите рассказать словами, что случилось?", "Похоже, момент тяжёлый. Я рядом. Что произошло?" });
    }

    [Fact]
    public void The_companion_is_honest_about_how_it_is()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Как дела?");

        Assert.Contains("приложение", reply.Messages[0]);
    }

    [Fact]
    public void The_companion_can_say_what_it_does_and_offers_next_steps()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Что ты умеешь?");

        Assert.Contains("не ставлю диагнозов", reply.Messages[0]);
        Assert.Equal(3, reply.QuickReplies.Count);
    }

    [Fact]
    public void A_name_is_remembered_and_used_when_saying_goodbye()
    {
        CompanionDialogue d = Create();
        CompanionReply hello = Say(d, new CompanionState(), "Меня зовут Аня");
        Assert.Equal("Аня", hello.State.UserName);

        CompanionReply bye = Say(d, hello.State, "Пока");

        Assert.Contains("Аня", bye.Messages[0]);
    }

    [Fact]
    public void After_asking_the_name_a_bare_word_is_taken_as_one()
    {
        CompanionDialogue d = Create();
        CompanionReply asked = Say(d, new CompanionState(), "Как тебя зовут?");
        Assert.True(asked.State.NamePending);

        CompanionReply named = Say(d, asked.State, "Вера");

        Assert.Equal("Вера", named.State.UserName);
        Assert.False(named.State.NamePending);
    }

    [Fact]
    public void A_bare_word_is_not_a_name_unless_the_name_was_asked_for()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Вера");

        Assert.Null(reply.State.UserName);
    }

    [Fact]
    public void A_feeling_after_asking_the_name_is_still_a_feeling()
    {
        CompanionDialogue d = Create();
        CompanionState asked = Say(d, new CompanionState(), "Как тебя зовут?").State;

        CompanionReply reply = Say(d, asked, "Мне очень тревожно");

        Assert.Null(reply.State.UserName);
        Assert.Equal("Anxiety", reply.State.Emotion);
    }

    [Fact]
    public void The_question_can_be_repeated_when_the_person_did_not_understand()
    {
        CompanionDialogue d = Create();
        CompanionReply asked = Say(d, new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех");
        string question = asked.Messages[^1];

        CompanionReply repeat = Say(d, asked.State, "не поняла");

        Assert.Contains(question, repeat.Messages[0]);
    }

    [Fact]
    public void Skipping_a_question_moves_on_to_another_one()
    {
        CompanionDialogue d = Create();
        CompanionReply asked = Say(d, new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех");
        string question = asked.Messages[^1];

        CompanionReply skipped = Say(d, asked.State, "давай другой вопрос");

        Assert.NotEmpty(skipped.Messages);
        Assert.DoesNotContain(question, skipped.Messages);
        Assert.EndsWith("?", skipped.Messages[^1]);
    }

    [Fact]
    public void A_long_message_is_acknowledged_as_such()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, new CompanionState(), "Меня бесит начальник").State;
        string longText = "Сегодня опять всё то же самое: начальник при всех раскритиковал мой отчёт, хотя я сидела над ним всю ночь, коллеги молчали, а потом ещё и премию урезали, и я просто не понимаю, как мне теперь с этим быть и стоит ли вообще продолжать так работать дальше";

        CompanionReply reply = Say(d, state, longText);

        Assert.Contains(new[] { "Спасибо, что так подробно рассказали.", "Это было непросто описать, спасибо вам.", "Прочитано целиком." }, phrase => reply.Messages[0].Contains(phrase));
    }

    [Theory]
    [InlineData(8, "Доброе утро")]
    [InlineData(14, "Добрый день")]
    [InlineData(20, "Добрый вечер")]
    public void The_first_greeting_matches_the_time_of_day(int hour, string expected)
    {
        CompanionReply reply = Create(time: At(hour)).Open(new CompanionState(), previous: null);

        Assert.StartsWith(expected, reply.Messages[0]);
        Assert.Equal(2, reply.Messages.Count);
    }

    [Fact]
    public void At_night_the_greeting_notes_the_late_hour_and_offers_to_talk()
    {
        CompanionReply reply = Create(time: At(3)).Open(new CompanionState(), previous: null);

        Assert.Contains("уже поздно", reply.Messages[0]);
        Assert.Contains("не спится", reply.Messages[0]);
    }

    [Fact]
    public void The_same_thing_is_not_said_twice_in_a_row()
    {
        // Every seed must avoid repeating the acknowledgement it has just used.
        for (int seed = 0; seed < 30; seed++)
        {
            CompanionDialogue d = Create(seed: seed);
            CompanionState state = Say(d, new CompanionState(), "Мне тревожно из-за работы").State;
            HashSet<string> heard = [];
            for (int turn = 0; turn < 3; turn++)
            {
                CompanionReply reply = Say(d, state, "ок");
                Assert.True(heard.Add(reply.Messages[0]), $"seed {seed}, turn {turn}: '{reply.Messages[0]}' was repeated");
                state = reply.State;
            }
        }
    }

    [Theory]
    [InlineData("7")]
    [InlineData("7/10")]
    [InlineData("около 7")]
    [InlineData("7.")]
    public void A_typed_number_answers_the_tension_question(string typed)
    {
        CompanionDialogue d = Create();
        CompanionState waiting = new() { Turns = 2, Emotion = "Anxiety", QuestionPending = true, LastQuestion = "Насколько сильное напряжение сейчас, от 0 до 10?" };

        CompanionReply reply = Say(d, waiting, typed);

        Assert.Equal(7, reply.State.LastIntensity);
    }

    [Fact]
    public void A_number_is_only_a_rating_when_a_rating_was_asked_for()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 2, Emotion = "Anxiety" }, "7");

        Assert.Null(reply.State.LastIntensity);
    }

    [Fact]
    public void An_answer_over_ten_is_not_a_rating()
    {
        CompanionState waiting = new() { Turns = 2, Emotion = "Anxiety", QuestionPending = true, LastQuestion = "Насколько сильное напряжение сейчас, от 0 до 10?" };

        CompanionReply reply = Say(Create(), waiting, "42");

        Assert.Null(reply.State.LastIntensity);
    }

    [Theory]
    [InlineData("ага")]
    [InlineData("нет")]
    [InlineData("не знаю")]
    public void Words_in_reply_to_the_scale_question_get_help_with_the_scale(string answer)
    {
        CompanionState waiting = new() { Turns = 2, Emotion = "Anxiety", QuestionPending = true, LastQuestion = "Насколько сильное напряжение сейчас, от 0 до 10?" };

        CompanionReply reply = Say(Create(), waiting, answer);

        Assert.Contains("0 — совсем спокойно", reply.Messages[0]);
        Assert.Equal(11, reply.QuickReplies.Count);
    }

    [Fact]
    public void A_typed_number_after_a_practice_is_the_new_tension()
    {
        CompanionDialogue d = Create();
        CompanionState after = d.FollowUpAfterPractice(new CompanionState { Turns = 3, Emotion = "Anxiety", LastIntensity = 8 }).State;

        CompanionReply reply = Say(d, after, "4");

        Assert.Equal(4, reply.State.LastIntensity);
        Assert.Contains("Было 8, стало 4", reply.Messages[0]);
    }

    [Fact]
    public void Well_in_answer_to_a_question_about_the_problem_is_agreement_not_good_news()
    {
        CompanionDialogue d = Create();
        CompanionState asked = Say(d, new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех").State;

        CompanionReply reply = Say(d, asked, "хорошо");

        Assert.DoesNotContain(reply.Messages[0], new[] { "Это хорошо слышать! Как думаете, что помогло?", "Отлично, это важно. Что стало решающим?" });
    }

    [Fact]
    public void A_change_of_feeling_is_noticed()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, new CompanionState(), "Я очень тревожусь из-за завтрашней встречи").State;

        CompanionReply reply = Say(d, state, "А ещё я так устала, сил вообще нет");

        Assert.Contains("на первый план", reply.Messages[0]);
    }

    [Fact]
    public void A_one_sentence_message_is_not_parroted_back()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Мне очень тревожно из-за завтрашней встречи");

        Assert.DoesNotContain("Вы пишете", reply.Messages[0]);
        Assert.DoesNotContain("«Мне очень тревожно из-за завтрашней встречи»", reply.Messages[0]);
    }

    [Fact]
    public void One_sentence_is_quoted_when_the_message_has_several()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Сегодня был странный день. Мне очень тревожно из-за завтрашней встречи. Не знаю, что делать.");

        Assert.Contains("«Мне очень тревожно из-за завтрашней встречи»", reply.Messages[0]);
    }


    [Fact]
    public void The_new_state_survives_a_restart()
    {
        CompanionState state = new()
        {
            UserName = "Аня",
            NamePending = true,
            LastQuestion = "Что случилось?",
            RecentReplies = ["Понимаю.", "Слышу вас."]
        };

        CompanionState restored = CompanionState.Deserialize(state.Serialize());

        Assert.Equal("Аня", restored.UserName);
        Assert.True(restored.NamePending);
        Assert.Equal("Что случилось?", restored.LastQuestion);
        Assert.Equal(["Понимаю.", "Слышу вас."], restored.RecentReplies);
    }

    [Fact]
    public void Crisis_wording_beats_every_everyday_rule()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "ок, не хочу больше жить");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
    }

    [Fact]
    public void English_conversation_is_equally_lively()
    {
        CompanionDialogue d = Create(english: true);

        Assert.Contains("app", Say(d, new CompanionState(), "how are you").Messages[0]);
        Assert.Contains("Nice to meet you, Anna", Say(d, new CompanionState(), "my name is Anna").Messages[0]);
        Assert.Contains("diagnose", Say(d, new CompanionState(), "what can you do").Messages[0]);
    }
}

/// <summary>A clock stopped at one moment, with UTC as the local zone so the hour is the same on every machine.</summary>
internal sealed class FixedTime(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
