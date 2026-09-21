using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class UtteranceClassifierTests
{
    [Theory]
    [InlineData("Привет", Utterance.Greeting)]
    [InlineData("Здравствуйте!", Utterance.Greeting)]
    [InlineData("hello there", Utterance.Greeting)]
    [InlineData("Спасибо", Utterance.Thanks)]
    [InlineData("большое спасибо тебе", Utterance.Thanks)]
    [InlineData("thanks a lot", Utterance.Thanks)]
    [InlineData("Пока", Utterance.Goodbye)]
    [InlineData("до свидания", Utterance.Goodbye)]
    [InlineData("good night", Utterance.Goodbye)]
    [InlineData("Не знаю", Utterance.DontKnow)]
    [InlineData("хз", Utterance.DontKnow)]
    [InlineData("i dont know", Utterance.DontKnow)]
    [InlineData("Не хочу об этом говорить", Utterance.RefusesToTalk)]
    [InlineData("i'd rather not", Utterance.RefusesToTalk)]
    [InlineData("Что мне делать?", Utterance.AsksForAdvice)]
    [InlineData("посоветуй что-нибудь", Utterance.AsksForAdvice)]
    [InlineData("what should I do", Utterance.AsksForAdvice)]
    [InlineData("Ты кто?", Utterance.AsksAboutCompanion)]
    [InlineData("ты человек или бот?", Utterance.AsksAboutCompanion)]
    [InlineData("are you a bot?", Utterance.AsksAboutCompanion)]
    [InlineData("Ты меня не понимаешь", Utterance.ComplainsAboutCompanion)]
    [InlineData("это бесполезно", Utterance.ComplainsAboutCompanion)]
    [InlineData("да", Utterance.Yes)]
    [InlineData("наверное, да", Utterance.Yes)]
    [InlineData("нет", Utterance.No)]
    [InlineData("вряд ли", Utterance.No)]
    public void Recognises_what_the_person_is_doing(string text, Utterance expected) =>
        Assert.Equal(expected, UtteranceClassifier.Classify(text, hasFeeling: false));

    [Theory]
    [InlineData("Мне очень тревожно из-за работы")]
    [InlineData("не могу уснуть")]
    [InlineData("Пока я не знаю, как объяснить, но мне плохо")]
    [InlineData("Спасибо за вопрос, но я так устал, что просто нет сил")]
    [InlineData("Не могу перестать думать о разговоре")]
    public void Ordinary_statements_are_not_mistaken_for_social_phrases(string text) =>
        Assert.Equal(Utterance.Statement, UtteranceClassifier.Classify(text, hasFeeling: true));

    [Fact]
    public void Social_phrases_next_to_a_feeling_stay_statements() =>
        Assert.Equal(Utterance.Statement, UtteranceClassifier.Classify("привет, мне очень тревожно", hasFeeling: true));

    [Fact]
    public void Talking_about_the_companion_is_recognised_even_next_to_a_feeling() =>
        Assert.Equal(Utterance.AsksAboutCompanion, UtteranceClassifier.Classify("мне страшно, ты вообще кто?", hasFeeling: true));

    [Fact]
    public void Blank_text_is_a_statement() =>
        Assert.Equal(Utterance.Statement, UtteranceClassifier.Classify("  ", hasFeeling: false));
}

public class CompanionIntelligenceTests
{
    private static CompanionDialogue Create(bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3));

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string kind, string label, string? payload = null) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(kind, label, payload)));

    [Fact]
    public void A_greeting_gets_a_greeting_not_an_interrogation()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Привет");

        Assert.Single(reply.Messages);
        Assert.Contains("сегодня", reply.Messages[0]);
        Assert.Empty(reply.State.AskedQuestions);
        Assert.Empty(reply.State.RecentTexts);
    }

    [Fact]
    public void Thanks_offers_to_continue_or_stop_and_stopping_says_goodbye()
    {
        CompanionDialogue d = Create();

        CompanionReply thanks = Say(d, new CompanionState { Turns = 3, Emotion = "Anxiety" }, "Спасибо");
        CompanionReply stop = Tap(d, thanks.State, ChatQuickReplyKinds.Act, "На сегодня достаточно", "enough");

        Assert.Equal(2, thanks.QuickReplies.Count);
        Assert.Contains("Берегите себя", stop.Messages[0]);
        Assert.Empty(stop.QuickReplies);
    }

    [Fact]
    public void I_dont_know_leads_to_a_choice_of_feelings_instead_of_another_question()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Не знаю");

        Assert.Equal(8, reply.QuickReplies.Count);
        Assert.All(reply.QuickReplies, q => Assert.Equal(ChatQuickReplyKinds.Emotion, q.Kind));
    }

    [Fact]
    public void Refusing_to_talk_is_respected_and_alternatives_are_offered()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 2, Emotion = "Guilt" }, "Не хочу об этом говорить");

        Assert.Contains("не будем", reply.Messages[0]);
        Assert.Equal(["body", "other", "enough"], reply.QuickReplies.Select(q => q.Payload));
        Assert.DoesNotContain(reply.Messages, m => m.EndsWith('?'));
    }

    [Fact]
    public void Body_practice_chip_offers_a_calming_practice()
    {
        CompanionReply reply = Tap(Create(), new CompanionState { Emotion = "Guilt", Turns = 2 }, ChatQuickReplyKinds.Act, "Практика для тела", "body");

        Assert.Contains(reply.QuickReplies, q => q.Kind == ChatQuickReplyKinds.Practice && q.Payload == "Breathing");
    }

    [Fact]
    public void Asking_for_advice_gets_an_honest_answer_about_what_the_companion_can_do()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 1, Emotion = "Anxiety" }, "Что мне делать?");

        Assert.Contains("не ставлю диагнозов", reply.Messages[0]);
        Assert.EndsWith("?", reply.Messages[0]);
    }

    [Fact]
    public void Asked_who_it_is_the_companion_says_it_is_an_app_and_stays_on_the_phone()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Ты кто?");

        Assert.Contains("не человек и не врач", reply.Messages[0]);
        Assert.Contains("на вашем телефоне", reply.Messages[0]);
    }

    [Fact]
    public void Complaining_about_the_companion_asks_what_would_help_instead()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 2, Emotion = "Anger" }, "Ты меня не понимаешь");

        Assert.Equal(["vent", "understand", "body"], reply.QuickReplies.Select(q => q.Payload));
    }

    [Fact]
    public void Yes_or_no_after_a_question_is_treated_as_an_answer_to_it()
    {
        CompanionDialogue d = Create();
        CompanionState asked = Say(d, new CompanionState(), "Меня бесит начальник, так злюсь").State;
        Assert.True(asked.QuestionPending);

        CompanionReply yes = Say(d, asked, "да");
        CompanionReply no = Say(d, asked, "нет");

        Assert.Contains(yes.Messages[0], new[] { "Понимаю. Что для вас в этом главное?", "Ясно. Что для вас важнее всего в этой ситуации?" });
        Assert.Contains(no.Messages[0], new[] { "Хорошо, спасибо, что уточнили. Как бы вы это описали?", "Ясно. Что тогда ближе к правде?" });
        Assert.Single(yes.Messages);
    }

    [Fact]
    public void A_bare_yes_without_a_pending_question_is_just_a_statement()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "да");

        Assert.NotEmpty(reply.State.RecentTexts);
    }

    [Fact]
    public void A_person_in_the_story_leads_to_a_question_about_that_person()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Меня бесит начальник, опять раскритиковал при всех");

        Assert.Equal("Boss", reply.State.Person);
        Assert.Contains("начальник так поступает", reply.Messages[^1]);
    }

    [Fact]
    public void Guilt_about_a_child_asks_what_the_person_would_say_to_the_child()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Мне стыдно, что я накричала на ребёнка");

        Assert.Equal("Child", reply.State.Person);
        Assert.Contains("ребёнку", reply.Messages[^1]);
    }

    [Fact]
    public void Two_strong_feelings_are_named_together()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Я злюсь на него и мне так стыдно за свои слова");

        Assert.Contains("несколько чувств", reply.Messages[0]);
        Assert.Contains("злость", reply.Messages[0]);
        Assert.Contains("вина", reply.Messages[0]);
    }

    [Fact]
    public void After_several_messages_the_companion_sums_up_and_reacts_to_the_answer()
    {
        CompanionDialogue d = Create();
        CompanionState state = new()
        {
            Turns = 3, Emotion = "Anger", Theme = "Work", Person = "Boss", ScaleAsked = true, FirstIntensity = 8, LastIntensity = 5,
            TurnsSinceOffer = 1, RecentTexts = ["a", "b", "c"], AskedQuestions = ["t:Anger:Boss"]
        };

        CompanionReply recap = Say(d, state, "Меня бесит начальник, он опять всё повторил");

        Assert.Contains("подытожить", recap.Messages[^1]);
        Assert.Contains("злость", recap.Messages[^1]);
        Assert.Contains("с 8 до 5", recap.Messages[^1]);
        Assert.Equal(["yes", "no"], recap.QuickReplies.Select(q => q.Payload));
        Assert.Contains("изменить в первую очередь", Tap(d, recap.State, ChatQuickReplyKinds.Recap, "Да, верно", "yes").Messages[0]);
        Assert.Contains("поправляете", Tap(d, recap.State, ChatQuickReplyKinds.Recap, "Не совсем", "no").Messages[0]);
    }

    [Fact]
    public void The_summary_is_not_repeated_every_message()
    {
        CompanionDialogue d = Create();
        CompanionState state = new()
        {
            Turns = 4, LastRecapTurn = 4, Emotion = "Anger", ScaleAsked = true, TurnsSinceOffer = 1, RecentTexts = ["a", "b", "c"]
        };

        CompanionReply reply = Say(d, state, "Он вообще меня не слушает, злюсь");

        Assert.DoesNotContain(reply.Messages, m => m.Contains("подытожить"));
    }

    [Fact]
    public void Later_the_companion_returns_to_the_first_thing_the_person_said()
    {
        CompanionDialogue d = Create();
        CompanionState state = new()
        {
            Turns = 4, LastRecapTurn = 4, Emotion = "Anxiety", ScaleAsked = true, TurnsSinceOffer = 1, RecentTexts = ["a", "b", "c"],
            FirstQuote = "Завтра важная презентация", AskedQuestions = ["trigger", "worst_case", "body", "need", "meaning", "more"]
        };

        CompanionReply reply = Say(d, state, "Я боюсь, что всё пойдёт не так");

        Assert.Contains("В начале вы говорили: «Завтра важная презентация»", string.Join(' ', reply.Messages));
        Assert.True(reply.State.CallbackAsked);
    }

    [Fact]
    public void Person_specific_questions_come_from_a_real_bank_and_do_not_repeat()
    {
        HashSet<string> asked = [];
        foreach (CompanionPerson person in Enum.GetValues<CompanionPerson>())
        {
            foreach (CompanionEmotion emotion in Enum.GetValues<CompanionEmotion>().Where(e => e != CompanionEmotion.Unknown))
            {
                List<string> chain = [];
                string? id;
                while ((id = CompanionActContent.TargetedQuestionId(emotion, person, chain)) is not null)
                {
                    Assert.DoesNotContain(id, chain);
                    string? ru = CompanionActContent.TargetedQuestion(id, english: false);
                    string? en = CompanionActContent.TargetedQuestion(id, english: true);
                    Assert.False(string.IsNullOrWhiteSpace(ru), id);
                    Assert.False(string.IsNullOrWhiteSpace(en), id);
                    Assert.EndsWith("?", ru);
                    chain.Add(id);
                    asked.Add(id);
                }

                Assert.NotEmpty(chain);
            }
        }

        Assert.True(asked.Count >= 15);
    }

    [Fact]
    public void English_moves_and_questions_are_english()
    {
        CompanionDialogue d = Create(english: true);

        CompanionReply thanks = Say(d, new CompanionState(), "thanks");
        CompanionReply about = Say(d, new CompanionState(), "are you a bot?");
        CompanionReply boss = Say(d, new CompanionState(), "I'm so angry at my boss, he criticised me again");

        foreach (string message in thanks.Messages.Concat(about.Messages).Concat(boss.Messages))
        {
            Assert.DoesNotContain(message, m => m is >= 'Ѐ' and <= 'ӿ');
        }

        Assert.Contains("boss", boss.Messages[^1]);
    }

    [Fact]
    public void Mentions_of_people_and_mixed_feelings_are_extracted_by_the_analyzer()
    {
        LexiconSituationAnalyzer analyzer = new();

        SituationAnalysis result = analyzer.Analyze("Мама опять сказала, что я всё делаю не так, я злюсь и мне стыдно");

        Assert.Contains(CompanionPerson.Parent, result.Persons!);
        Assert.NotEqual(CompanionEmotion.Unknown, result.Secondary);
    }
}
