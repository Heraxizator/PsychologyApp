using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>
/// Whole conversations, read the way a person would read them. Each case came from reading a transcript and finding something a thoughtful
/// listener would not have said: asking "what is the hardest part?" after good news, offering a breathing exercise after a death, explaining
/// the 0-10 scale to someone who had just said something real.
/// </summary>
public class CompanionConversationQualityTests
{
    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    private sealed record Turn(string User, CompanionReply Reply)
    {
        public string Text => string.Join(" ", Reply.Messages);

        public bool OffersPractice => Reply.QuickReplies.Any(q => q.Kind == ChatQuickReplyKinds.Practice);

        public bool AsksForScale => Reply.QuickReplies.Any(q => q.Kind == ChatQuickReplyKinds.Rating);
    }

    private static List<Turn> Talk(bool english, params string[] messages)
    {
        CompanionDialogue dialogue = new(
            new LexiconSituationAnalyzer(),
            new KeywordCrisisDetector(),
            english,
            new Random(11),
            new FixedTime(new DateTime(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc)),
            emotionGuesser: EmotionGuesser.Bundled);
        CompanionState state = dialogue.Open(new CompanionState(), previous: null).State;
        List<Turn> turns = [];
        foreach (string message in messages)
        {
            CompanionReply reply = dialogue.Respond(state, new CompanionInput.FreeText(message));
            turns.Add(new Turn(message, reply));
            state = reply.State;
        }

        return turns;
    }

    private static List<Turn> Talk(params string[] messages) => Talk(false, messages);

    [Fact]
    public void ADeathGetsCondolenceAndCompanyNotAScaleOrAPractice()
    {
        List<Turn> chat = Talk("Привет", "У меня умерла бабушка на прошлой неделе", "Мы были очень близки", "Мне её так не хватает");

        Assert.Contains("жаль", chat[1].Text);
        Assert.All(chat.Skip(1), t =>
        {
            Assert.False(t.AsksForScale, t.Text);
            Assert.False(t.OffersPractice, t.Text);
            Assert.DoesNotContain("самое тяжёлое", t.Text);
        });
        Assert.Equal(CompanionEmotion.Sadness.ToString(), chat[^1].Reply.State.Emotion);
    }

    [Fact]
    public void FearOfDeathIsNotTreatedAsGrief()
    {
        List<Turn> chat = Talk("Я очень боюсь смерти, не могу об этом не думать");

        Assert.DoesNotContain("жаль", chat[0].Text);
        Assert.False(chat[0].Reply.State.GriefShared);
    }

    [Fact]
    public void ADifferentFeelingAfterTheLossEndsTheGriefMode()
    {
        List<Turn> chat = Talk("Мой отец умер в марте", "Сейчас меня очень злит, что врачи ничего не сделали", "Я просто в ярости на них");

        Assert.Contains("жаль", chat[0].Text);
        Assert.Equal(CompanionEmotion.Anger.ToString(), chat[^1].Reply.State.Emotion);
    }

    [Fact]
    public void GoodNewsIsCelebratedNotInterrogated()
    {
        List<Turn> chat = Talk("Привет", "У меня сегодня хорошая новость, меня повысили");

        Assert.DoesNotContain("самое тяжёлое", chat[1].Text);
        Assert.Contains("Здорово", chat[1].Text + chat[1].Reply.State.Emotion, StringComparison.Ordinal);
        Assert.False(chat[1].AsksForScale);
    }

    [Fact]
    public void AnEnglishAchievementIsCelebratedToo()
    {
        List<Turn> chat = Talk(true, "I got promoted today!");

        Assert.DoesNotContain("hardest", chat[0].Text);
        Assert.Matches("(?i)congratulations|great|glad|news", chat[0].Text);
    }

    [Fact]
    public void ARefusalToTalkIsRespectedAndNotMistakenForExhaustion()
    {
        List<Turn> chat = Talk("Привет", "Устала от всех", "Нет, не хочу ничего обсуждать");

        Assert.Contains("не будем", chat[2].Text);
        Assert.DoesNotContain("усталость", chat[2].Text);
        Assert.False(chat[2].AsksForScale);
    }

    [Theory]
    [InlineData("Сколько стоит это приложение?", "оплат")]
    [InlineData("Где хранятся мои данные?", "телефон")]
    [InlineData("Нужен ли интернет?", "без интернета")]
    [InlineData("Кто тебя создал?", "разработчик")]
    public void AQuestionAboutTheAppIsAnsweredNotTreatedAsAStory(string question, string expected)
    {
        List<Turn> chat = Talk(question);

        Assert.Contains(expected, chat[0].Text);
        Assert.DoesNotContain("самое тяжёлое", chat[0].Text);
    }

    [Fact]
    public void ThePriceQuestionNeverClaimsAPrice()
    {
        string answer = Talk("Сколько стоит это приложение?")[0].Text;

        Assert.DoesNotContain("бесплатн", answer);
        Assert.DoesNotContain("₽", answer);
        Assert.DoesNotContain("руб", answer);
    }

    [Fact]
    public void SomethingRealSaidWhileTheScaleWasOpenIsAnsweredNotMetWithTheScaleHelp()
    {
        List<Turn> chat = Talk("Я поссорилась с мужем, он сказал, что я истеричка", "Мне обидно и я злюсь", "Не знаю, может он прав");

        Assert.True(chat[1].AsksForScale);
        Assert.DoesNotContain("Достаточно примерно", chat[2].Text);
    }

    [Fact]
    public void AShortDontKnowWhileTheScaleIsOpenStillGetsTheScaleHelp()
    {
        List<Turn> chat = Talk("Мне очень тревожно", "Это из-за работы", "Не знаю");

        Assert.True(chat[1].AsksForScale);
        Assert.True(chat[2].AsksForScale);
        Assert.Contains("Достаточно примерно", chat[2].Text);
    }

    [Fact]
    public void WhoIsMentionedFirstDecidesTheTheme()
    {
        List<Turn> chat = Talk("Дети целый день на мне, муж на работе, я не успеваю ничего, хочется выть");

        Assert.Equal(CompanionTheme.Family.ToString(), chat[0].Reply.State.Theme);
    }

    [Fact]
    public void PanicIsNotSaidTwiceInOneReply()
    {
        List<Turn> chat = Talk("Сердце колотится и руки трясутся");

        string text = chat[0].Text;
        int first = text.IndexOf("поможем телу успокоиться", StringComparison.Ordinal);
        Assert.True(first >= 0);
        Assert.Equal(-1, text.IndexOf("поможем телу успокоиться", first + 5, StringComparison.Ordinal));
    }

    [Fact]
    public void ShakingBeforeASpeechIsAnxietyNotAPanicAttack()
    {
        List<Turn> chat = Talk("Завтра выступление перед всем отделом, меня трясёт");

        Assert.Equal(CompanionEmotion.Anxiety.ToString(), chat[0].Reply.State.Emotion);
        Assert.False(chat[0].OffersPractice);
    }

    [Fact]
    public void AGreetingDoesNotBringTheFirstPracticeOfferCloser()
    {
        List<Turn> chat = Talk("Привет", "Мне тревожно из-за работы", "Начальник давит на меня");

        Assert.False(chat[1].OffersPractice);
        Assert.False(chat[2].OffersPractice, chat[2].Text);
    }

    [Theory]
    [InlineData("Меня бросил парень после трёх лет", "Sadness")]
    [InlineData("Меня сократили на работе", "Anxiety")]
    [InlineData("Мне поставили диагноз, жду результатов анализов", "Anxiety")]
    [InlineData("Я неудачница, ничего у меня не получается", "Sadness")]
    [InlineData("Начальник опять накричал при всех", "Resentment")]
    public void LifeEventsAreHeardAsTheFeelingTheyUsuallyBring(string message, string expectedEmotion)
    {
        List<Turn> chat = Talk(message);

        Assert.Equal(expectedEmotion, chat[0].Reply.State.Emotion);
        Assert.DoesNotContain("самое тяжёлое", chat[0].Text);
    }

    [Fact]
    public void AJokeRequestIsAnsweredHonestlyAndVaries()
    {
        List<Turn> chat = Talk("Расскажи анекдот", "А ты умеешь шутить?", "Знаешь анекдот?");

        Assert.All(chat, t => Assert.Contains("цитат", t.Text));
        Assert.True(chat.Select(t => t.Text).Distinct().Count() >= 2);
    }

    [Fact]
    public void BoredomIsMetWithCompanyNotWithAnInterrogation()
    {
        List<Turn> chat = Talk("Мне скучно");

        Assert.Contains("поговорить", chat[0].Text);
        Assert.DoesNotContain("самое тяжёлое", chat[0].Text);
    }

    [Fact]
    public void NoPracticesMeansNoUnpromptedOffersAfterwards()
    {
        List<Turn> chat = Talk("Мне тревожно из-за работы", "Начальник давит на меня", "7", "не хочу делать никакие практики", "Он опять придрался к отчёту", "Я не знаю как с ним говорить", "Это продолжается каждый день");

        Assert.Contains("без практик", chat[3].Text);
        Assert.All(chat.Skip(3), t => Assert.False(t.OffersPractice, t.Text));
    }

    [Fact]
    public void AnExplicitRequestStillGetsAPracticeAfterDecliningThem()
    {
        List<Turn> chat = Talk("Мне тревожно из-за работы", "Начальник давит на меня", "не хочу никаких практик", "Всё-таки что мне делать? Дай совет");

        Assert.Contains("практик", chat[3].Text);
    }

    [Fact]
    public void LaterIsAcceptedWithoutPressure()
    {
        List<Turn> chat = Talk("Мне тревожно из-за работы", "Начальник давит на меня", "потом");

        Assert.Contains("вернёмся", chat[2].Text);
        Assert.False(chat[2].OffersPractice);
    }

    [Fact]
    public void SayingYesRightAfterAnOfferStartsThatPractice()
    {
        CompanionDialogue dialogue = new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english: false, new Random(11), new FixedTime(new DateTime(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc)));
        CompanionState state = dialogue.Open(new CompanionState(), previous: null).State;
        CompanionReply offer = dialogue.Respond(state, new CompanionInput.FreeText("Сердце колотится и руки трясутся"));
        Assert.NotNull(offer.State.OfferedPrimary);

        CompanionReply accepted = dialogue.Respond(offer.State, new CompanionInput.FreeText("Ладно, давай попробуем"));

        Assert.Equal(DialogueActionKind.StartTechnique, accepted.Action?.Kind);
        Assert.Equal(offer.State.OfferedPrimary, accepted.Action?.TechniqueId?.ToString());
    }

    [Fact]
    public void AskingForSomethingElseAfterAnOfferShowsTheOtherPractice()
    {
        CompanionDialogue dialogue = new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english: false, new Random(11), new FixedTime(new DateTime(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc)));
        CompanionState state = dialogue.Open(new CompanionState(), previous: null).State;
        CompanionReply offer = dialogue.Respond(state, new CompanionInput.FreeText("Сердце колотится и руки трясутся"));

        CompanionReply other = dialogue.Respond(offer.State, new CompanionInput.FreeText("А есть что-то другое?"));

        Assert.Contains(other.QuickReplies, q => q.Kind == ChatQuickReplyKinds.Practice && q.Payload == offer.State.OfferedAlternative);
    }

    [Fact]
    public void ALongLastingLowMoodGetsOneCalmWordAboutTalkingToAProfessional()
    {
        List<Turn> chat = Talk("Мне грустно и ничего не радует", "Это уже месяц", "Совсем нет желания что-то делать", "Каждый день одно и то же");

        int notes = chat.Count(t => t.Text.Contains("врачом или психологом", StringComparison.Ordinal));
        Assert.Equal(1, notes);
    }

    [Fact]
    public void LetUsTalkTomorrowIsAFarewellNotAnEmotion()
    {
        List<Turn> chat = Talk("Мне не с кем поговорить", "Давай завтра ещё поговорим");

        Assert.Contains("Берегите себя", chat[1].Text);
        Assert.False(chat[1].AsksForScale);
    }

    [Fact]
    public void EnglishGriefAndDecliningAreHandledToo()
    {
        List<Turn> grief = Talk(true, "My grandmother passed away last week");
        Assert.Contains("sorry", grief[0].Text);
        Assert.False(grief[0].AsksForScale);

        List<Turn> decline = Talk(true, "I feel anxious about work", "My boss is pushing me", "I do not want any practices");
        Assert.Contains("no practices", decline[2].Text);
    }

    [Theory]
    [InlineData("прив")]
    [InlineData("здрасьте")]
    public void InformalGreetingsAreGreetings(string hello)
    {
        List<Turn> chat = Talk(hello);

        Assert.DoesNotContain("не хватает слов", chat[0].Text);
        Assert.Matches("(?i)здравствуйте|добр|привет", chat[0].Text);
    }

    [Fact]
    public void AMessageWithoutAFeelingDoesNotFlipTheKnownFeeling()
    {
        List<Turn> chat = Talk("все достали", "хочется закрыться и ни с кем не общаться", "не знаю зачем я это пишу");

        Assert.Equal(chat[1].Reply.State.Emotion, chat[2].Reply.State.Emotion);
        Assert.DoesNotContain("на первый план", chat[2].Text);
    }

    [Fact]
    public void WhatIsAPanicAttackIsAnExplanationNotAnEmergency()
    {
        List<Turn> chat = Talk("А паническая атака это что?");

        Assert.Contains("внезапный всплеск", chat[0].Text);
        Assert.False(chat[0].OffersPractice);
    }
}
