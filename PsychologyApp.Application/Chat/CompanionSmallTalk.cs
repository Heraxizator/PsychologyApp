using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// The everyday side of a conversation: backchannels, jokes, good news, "how are you?", names, "what can you do?", "say that again".
/// Wording never assumes anyone's gender, including the companion's own. Needs review by a psychologist before release.
/// </summary>
public static class CompanionSmallTalk
{
    // ----- greetings that know the time of day -----

    /// <summary>Opening word for the first message of a chat, by local hour. At night it also notes the late hour.</summary>
    public static string Hello(int hour, bool english) => hour switch
    {
        >= 5 and < 12 => english ? "Good morning." : "Доброе утро.",
        >= 12 and < 18 => english ? "Good afternoon." : "Добрый день.",
        >= 18 and < 23 => english ? "Good evening." : "Добрый вечер.",
        _ => english ? "Hello, it's quite late." : "Здравствуйте, уже поздно."
    };

    public static bool IsNight(int hour) => hour is >= 23 or < 5;

    public static string FirstGreeting(int hour, bool english, Random random, string? name)
    {
        string hello = Hello(hour, english);
        string who = name is null ? string.Empty : $", {name}.";
        string tail = IsNight(hour)
            ? english ? " If you can't sleep, we can talk. What is going on?" : " Если не спится, можем поговорить. Что происходит?"
            : Pick(random, english
                ? [" I'm here to listen. Tell me in your own words what is going on right now.", " What's on your mind at the moment? Tell me as it comes."]
                : [" Я здесь, чтобы выслушать. Расскажите своими словами, что сейчас происходит.", " Что у вас на душе прямо сейчас? Расскажите, как есть."]);
        return who.Length == 0 ? hello + tail : hello.TrimEnd('.') + who + tail;
    }

    /// <summary>Reply to a bare "hi". Always asks how the person is today.</summary>
    public static string GreetingReply(int hour, bool english, Random random, string? name)
    {
        string who = name is null ? string.Empty : $", {name}";
        return Pick(random, english
            ? [$"{Hello(hour, true).TrimEnd('.')}{who}! How are you today?", $"Hi{who}! What is on your mind today?"]
            : [$"{Hello(hour, false).TrimEnd('.')}{who}! Как вы сегодня?", $"Здравствуйте{who}! Что у вас на душе сегодня?"]);
    }

    // ----- backchannel, jokes, silence -----

    public static string Acknowledge(bool questionPending, bool english, Random random) => questionPending
        ? Pick(random, english
            ? ["Take your time. If you'd rather not answer, we can skip it.", "No rush. We can also skip this question if it doesn't fit."]
            : ["Не торопитесь. Если не хочется отвечать, можем пропустить этот вопрос.", "Без спешки. Если вопрос не подходит, можно его пропустить."])
        : Pick(random, english
            ? ["I'm here. Say more whenever you want.", "Okay. I'm listening if there's more.", "Mm-hm. What comes to mind?"]
            : ["Я рядом. Продолжайте, когда захотите.", "Хорошо. Слушаю, если есть что добавить.", "Угу. Что приходит в голову?"]);

    public static string Reaction(string text, bool sad, bool english, Random random)
    {
        if (sad)
        {
            return Pick(random, english
                ? ["I see that it's hard right now. Would you like to tell me in words what happened?", "That looks like a heavy moment. I'm here. What happened?"]
                : ["Вижу, что сейчас нелегко. Хотите рассказать словами, что случилось?", "Похоже, момент тяжёлый. Я рядом. Что произошло?"]);
        }

        if (text.Contains("...", StringComparison.Ordinal) || text.Contains('…'))
        {
            return Pick(random, english
                ? ["It's okay if there are no words yet. I'm here.", "Sometimes it's hard to say. Take your time, I'm not going anywhere."]
                : ["Ничего, если слов пока нет. Я рядом.", "Иногда сложно сказать. Не торопитесь, я никуда не ухожу."]);
        }

        return Pick(random, english
            ? ["Good to see a smile. What's the mood today?", "Glad there's some lightness. What's on your mind?"]
            : ["Хорошо, что получается улыбнуться. Какое сегодня настроение?", "Приятно, когда есть лёгкость. Что у вас на душе?"]);
    }

    public static string GoodNews(bool english, Random random) => Pick(random, english
        ? ["That's good to hear! What do you think helped?", "Nice, that matters. What made the difference?", "I'm glad. Try to notice what exactly helped, so you can repeat it. What was it?"]
        : ["Это хорошо слышать! Как думаете, что помогло?", "Отлично, это важно. Что стало решающим?", "Здорово. Заметьте, что именно помогло, чтобы повторить это потом. Что это было?"]);

    public static IReadOnlyList<ChatQuickReply> GoodNewsReplies(bool english) =>
    [
        CompanionActContent.Continue(english),
        CompanionActContent.Enough(english)
    ];

    // ----- about the companion -----

    public static string HowAreYou(bool english, Random random) => Pick(random, english
        ? ["Thank you for asking. I'm an app and I don't have moods, but I care how you are. How are you?", "I'm ready to listen. More importantly, how are you today?"]
        : ["Спасибо, что спросили. Я приложение и настроения у меня нет, но мне важно, как вы. Как вы?", "Я на связи и слушаю. Важнее, как вы сегодня?"]);

    public static string Name(bool english) => english
        ? "I don't have a name, I'm just a companion in this app. What should I call you?"
        : "У меня нет имени, я просто собеседник в этом приложении. А как мне называть вас?";

    public static string Nice(string name, bool hasEmotion, bool english) => english
        ? $"Nice to meet you, {name}. {(hasEmotion ? "Shall we go on with what we were talking about?" : "How are you today?")}"
        : $"Приятно познакомиться, {name}. {(hasEmotion ? "Продолжим то, о чём говорили?" : "Как вы сегодня?")}";

    public static string Capabilities(bool english) => english
        ? "I can listen and help you sort out what you feel, show what usually helps with anxiety, anger, guilt and exhaustion, explain things like panic or CBT, and guide short practices. I'm not a doctor and I don't diagnose. Where shall we start?"
        : "Я умею слушать и помогать разобраться в том, что вы чувствуете, подсказывать, что обычно помогает при тревоге, злости, вине и усталости, объяснять такие вещи, как паника или КПТ, и проводить короткие практики. Я не врач и не ставлю диагнозов. С чего начнём?";

    public static IReadOnlyList<ChatQuickReply> CapabilityReplies(bool english) =>
    [
        CompanionActContent.Vent(english),
        CompanionActContent.BodyPractice(english),
        CompanionActContent.Understand(english)
    ];

    // ----- clarification -----

    public static string Repeat(string? lastQuestion, bool english, Random random)
    {
        if (string.IsNullOrWhiteSpace(lastQuestion))
        {
            return english
                ? "I'm trying to understand what you feel right now. Tell me in any words what is bothering you most."
                : "Я пытаюсь понять, что вы сейчас чувствуете. Расскажите любыми словами, что беспокоит вас больше всего.";
        }

        string lead = Pick(random, english
            ? ["Sorry, let me ask again.", "Let me repeat the question."]
            : ["Простите, спрошу ещё раз.", "Повторю вопрос."]);
        string tail = english ? " You can answer in a word or skip it." : " Можно ответить одним словом или пропустить.";
        return $"{lead} {lastQuestion}{tail}";
    }

    public static string Skipped(bool english, Random random) => Pick(random, english
        ? ["Sure, let's move on.", "No problem, another question then."]
        : ["Конечно, идём дальше.", "Без проблем, тогда другой вопрос."]);

    // ----- addressing by name, sparingly -----

    public static string Goodbye(string? name, bool english) => name is null
        ? CompanionActContent.Goodbye(english)
        : english
            ? $"Take care of yourself, {name}. I'm here whenever you want to come back."
            : $"Берегите себя, {name}. Я здесь, когда захотите вернуться.";

    public static string Thanks(string? name, bool english) => name is null
        ? CompanionActContent.Thanks(english)
        : english
            ? $"You're welcome, {name}. Shall we continue, or is that enough for today?"
            : $"Пожалуйста, {name}. Продолжим или на сегодня достаточно?";

    /// <summary>Puts a name in front of a sentence the way people do: "Аня, Если подытожить…" becomes "Аня, если подытожить…".</summary>
    public static string Address(string? name, string sentence)
    {
        if (name is null || sentence.Length == 0 || !char.IsLetter(sentence[0]))
        {
            return sentence;
        }

        return $"{name}, {char.ToLowerInvariant(sentence[0])}{sentence[1..]}";
    }

    // ----- the scale question, answered in words -----

    public static string ScaleHelp(bool english) => english
        ? "An approximate number is enough: 0 is completely calm, 10 is as strong as it gets. You can tap a number below."
        : "Достаточно примерно: 0 — совсем спокойно, 10 — сильнее не бывает. Можно нажать цифру ниже.";

    // ----- following the person's feeling, not just nodding -----

    /// <summary>A short, specific echo of the feeling for a later message. Replaces the generic "I understand".</summary>
    public static string Validation(CompanionEmotion emotion, bool english, Random random) => emotion switch
    {
        CompanionEmotion.Panic or CompanionEmotion.Anxiety => Pick(random, english
            ? ["That much worry is tiring.", "Feeling on edge like this is hard to carry."]
            : ["Столько тревоги выматывает.", "Трудно нести в себе такое напряжение."]),
        CompanionEmotion.Overthinking => Pick(random, english
            ? ["Thoughts going round like that don't let you rest.", "That loop is exhausting."]
            : ["Когда мысли ходят по кругу, отдохнуть не получается.", "Этот круг очень выматывает."]),
        CompanionEmotion.Anger => Pick(random, english
            ? ["It makes sense that this made you angry.", "Anger here is understandable."]
            : ["Понятно, что это вызывает злость.", "Злиться здесь вполне объяснимо."]),
        CompanionEmotion.Resentment => Pick(random, english
            ? ["It's painful when someone important hurts you.", "Resentment usually means it mattered."]
            : ["Больно, когда важный человек задевает.", "Обида обычно говорит о том, что это было важно."]),
        CompanionEmotion.Guilt => Pick(random, english
            ? ["Guilt can be heavy, especially when you care.", "You're hard on yourself here."]
            : ["Вина бывает тяжёлой, особенно когда вам не всё равно.", "Вы очень строги к себе в этом."]),
        CompanionEmotion.Sadness => Pick(random, english
            ? ["That sounds sad.", "It's okay to feel sad about this."]
            : ["Это звучит грустно.", "Грустить об этом нормально."]),
        CompanionEmotion.Exhaustion => Pick(random, english
            ? ["You sound really worn out.", "It's a lot to carry when there's no strength left."]
            : ["Похоже, вы очень вымотаны.", "Тяжело, когда сил не остаётся."]),
        CompanionEmotion.Loneliness => Pick(random, english
            ? ["Feeling alone is hard.", "I'm glad you're not saying this to the wall."]
            : ["Одиночество переносить трудно.", "Хорошо, что вы говорите об этом не в пустоту."]),
        CompanionEmotion.Procrastination => Pick(random, english
            ? ["That stuck feeling is unpleasant.", "It's frustrating when you want to start and can't."]
            : ["Это неприятное чувство застревания.", "Обидно, когда хочешь начать и не получается."]),
        _ => Pick(random, english ? ["I hear you.", "Thank you for explaining."] : ["Слышу вас.", "Спасибо, что объясняете."])
    };

    /// <summary>The feeling has changed since the last message. Saying so shows that the person is being followed.</summary>
    public static string Shift(CompanionEmotion from, CompanionEmotion to, bool english) => english
        ? $"It seems {CompanionContent.EmotionName(to, true)} is now in front, more than {CompanionContent.EmotionName(from, true)}."
        : $"Похоже, теперь на первый план вышло другое: {CompanionContent.EmotionName(to, false)}, а не {CompanionContent.EmotionName(from, false)}.";


    // ----- what helped before -----

    public static string PreferredNote(TechniqueId id, bool english) => english
        ? $"Last time “{CompanionContent.TechniqueTitle(id, true)}” helped you."
        : $"В прошлый раз вам помогла практика «{CompanionContent.TechniqueTitle(id, false)}».";

    public static string Remembered(TechniqueId id, bool english) => english
        ? $"I'll remember that “{CompanionContent.TechniqueTitle(id, true)}” works for you and suggest it first next time."
        : $"Запомню, что практика «{CompanionContent.TechniqueTitle(id, false)}» вам помогает, и в следующий раз предложу её первой.";


    /// <summary>Appreciation for a long message. A person who wrote a lot deserves to hear it was read.</summary>
    public static string LongMessage(bool english, Random random) => Pick(random, english
        ? ["Thank you for telling me so much.", "That was a lot to put into words, thank you.", "I read all of it. Thank you for trusting me with it."]
        : ["Спасибо, что так подробно рассказали.", "Это было непросто описать, спасибо вам.", "Прочитано целиком. Спасибо, что доверяете."]);

    // ----- provocation and insults aimed at the companion -----

    /// <summary>
    /// An insult, a slur or a crude command lands here. No lecture, no scolding, no wounded tone — the companion has no
    /// body or pride to protect, so it says so plainly, then hands the conversation straight back to the person.
    /// </summary>
    public static string Boundary(bool english, Random random) => Pick(random, english
        ?
        [
            "That won't land on me — I'm software, no gender, no body, nothing to offend. If you want to talk for real, I'm here. What's actually going on?",
            "No hard feelings, I can't be hurt that way. Testing the waters is fine. What's really up?",
            "Fair enough, sometimes it's tempting to poke at the thing on the other end. I'm not going anywhere. What's actually bothering you?"
        ]
        :
        [
            "Меня это не заденет — я программа, без тела и без обид. Если захотите поговорить всерьёз, я рядом. Что на самом деле происходит?",
            "Ничего страшного, проверять границы — это нормально. Обидеть меня так не получится. Что у вас на самом деле случилось?",
            "Понимаю, иногда хочется зацепить собеседника. Я никуда не денусь. Расскажите, что правда происходит?"
        ]);

    // ----- the journal, read at the start of a chat -----

    /// <summary>Opens the chat around a low mood already logged in the journal today, instead of the usual greeting.
    /// Quotes the person's own note when they left one — the same courtesy as quoting their own words in the chat itself.</summary>
    public static string JournalReference(MoodEntryDTO mood, bool english, Random random)
    {
        if (!string.IsNullOrWhiteSpace(mood.Note))
        {
            return english
                ? $"Hi. I saw today's journal entry: “{mood.Note.Trim()}”. Want to talk about it?"
                : $"Здравствуйте. Видел(а) сегодняшнюю запись в дневнике: «{mood.Note.Trim()}». Расскажете подробнее?";
        }

        return Pick(random, english
            ? ["Hi. Today's journal entry looked heavy. What happened?", "Hello. I noticed today was marked as a hard day in the journal — want to talk about it?"]
            : ["Здравствуйте. Сегодняшняя запись в дневнике выглядит тяжёлой. Что случилось?", "Привет. Заметил(а), что сегодня в дневнике тяжёлый день — расскажете, что произошло?"]);
    }

    private static string Pick(Random random, string[] variants) => variants[random.Next(variants.Length)];
}
