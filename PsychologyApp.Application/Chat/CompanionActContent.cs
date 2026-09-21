using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>Wording for conversational moves other than "describing a feeling", plus person-specific questions and summaries.</summary>
public static class CompanionActContent
{
    // ----- replies to conversational moves -----

    public static string Greeting(bool english, Random random) => Pick(random, english
        ? ["Hello! How are you today?", "Hi! What is on your mind today?"]
        : ["Здравствуйте! Как вы сегодня?", "Привет! Что у вас на душе сегодня?"]);

    public static string Thanks(bool english) => english
        ? "You're welcome. Shall we continue, or is that enough for today?"
        : "Пожалуйста. Продолжим или на сегодня достаточно?";

    public static string Goodbye(bool english) => english
        ? "Take care of yourself. I'm here whenever you want to come back."
        : "Берегите себя. Я здесь, когда захотите вернуться.";

    public static string DontKnow(bool english) => english
        ? "That's okay, not knowing is normal. Let's try another way: which of these is closest?"
        : "Это нормально, не знать. Давайте попробуем по-другому: что из этого ближе всего?";

    public static string Refuses(bool english) => english
        ? "Okay, we won't go into that. We can do something for the body, talk about something else, or stop here."
        : "Хорошо, не будем об этом. Можем сделать что-то для тела, поговорить о другом или остановиться.";

    public static string Advice(bool english) => english
        ? "I can't decide for you and I don't diagnose, but I can help you sort things out and suggest proven practices. What would you most like to change first?"
        : "Я не могу решить за вас и не ставлю диагнозов, но могу помочь разобраться и предложить проверенные практики. Что вы хотели бы изменить в первую очередь?";

    public static string AboutCompanion(bool english) => english
        ? "I'm a companion app: not a person and not a doctor. Everything you write stays on your phone. I listen and help you sort things out, but I don't replace a professional. What would you like to talk about?"
        : "Я приложение-собеседник: не человек и не врач. Всё, что вы пишете, остаётся на вашем телефоне. Я слушаю и помогаю разобраться, но не заменяю специалиста. О чём хотите поговорить?";

    public static string Complaint(bool english) => english
        ? "I'm sorry that wasn't helpful. What would be more useful right now?"
        : "Мне жаль, что вышло не то. Что было бы полезнее сейчас?";

    public static string YesProbe(bool english, Random random) => Pick(random, english
        ? ["I see. What is the main thing about it for you?", "Understood. What matters most to you in this?"]
        : ["Понимаю. Что для вас в этом главное?", "Ясно. Что для вас важнее всего в этой ситуации?"]);

    public static string NoProbe(bool english, Random random) => Pick(random, english
        ? ["Okay, thanks for clarifying. How would you put it, then?", "Alright. What is closer to the truth for you?"]
        : ["Хорошо, спасибо, что уточнили. Как бы вы это описали?", "Ясно. Что тогда ближе к правде?"]);

    public static string OtherTopic(bool english) => english ? "I'm listening. What would you like to talk about?" : "Слушаю вас. О чём хотите поговорить?";

    // ----- labels of the chips that go with those replies -----

    public static ChatQuickReply Continue(bool english) => new(ChatQuickReplyKinds.Act, english ? "Let's continue" : "Продолжим", "continue");
    public static ChatQuickReply Enough(bool english) => new(ChatQuickReplyKinds.Act, english ? "Enough for today" : "На сегодня достаточно", "enough");
    public static ChatQuickReply BodyPractice(bool english) => new(ChatQuickReplyKinds.Act, english ? "Something for the body" : "Практика для тела", "body");
    public static ChatQuickReply TalkOther(bool english) => new(ChatQuickReplyKinds.Act, english ? "Talk about something else" : "Поговорить о другом", "other");
    public static ChatQuickReply Vent(bool english) => new(ChatQuickReplyKinds.Act, english ? "Just let it out" : "Просто выговориться", "vent");
    public static ChatQuickReply Understand(bool english) => new(ChatQuickReplyKinds.Act, english ? "Sort out the situation" : "Разобраться в ситуации", "understand");

    // ----- mixed feelings, summaries, callbacks -----

    public static string Mixed(CompanionEmotion first, CompanionEmotion second, bool english) => english
        ? $"There seem to be several feelings at once here: {CompanionContent.EmotionName(first, true)} and {CompanionContent.EmotionName(second, true)}. That happens when a situation touches a lot at once."
        : $"Здесь, похоже, сразу несколько чувств: {CompanionContent.EmotionName(first, false)} и {CompanionContent.EmotionName(second, false)}. Так бывает, когда ситуация задевает многое сразу.";

    public static string Recap(CompanionEmotion emotion, CompanionTheme? theme, CompanionPerson? person, int? first, int? last, bool english)
    {
        string feeling = CompanionContent.EmotionName(emotion, english);
        string about = person is { } p ? PersonName(p, english) : theme is { } t ? CompanionContent.ThemeName(t, english) : string.Empty;
        string tension = first is { } f && last is { } l && f != l
            ? (english ? $" The tension went from {f} to {l}." : $" Напряжение изменилось с {f} до {l}.")
            : last is { } only ? (english ? $" The tension is about {only}." : $" Напряжение около {only}.") : string.Empty;

        string subject = about.Length == 0 ? feeling : $"{feeling} ({(english ? "about" : "тема")}: {about})";
        return english
            ? $"Let me sum up: the main thing right now is {subject}.{tension} Is that right?"
            : $"Если подытожить: главное сейчас — {subject}.{tension} Верно ли это?";
    }

    public static IReadOnlyList<ChatQuickReply> RecapReplies(bool english) =>
    [
        new(ChatQuickReplyKinds.Recap, english ? "Yes, that's right" : "Да, верно", "yes"),
        new(ChatQuickReplyKinds.Recap, english ? "Not quite" : "Не совсем", "no")
    ];

    public static string RecapYes(bool english) => english
        ? "Good. What would you like to change first?"
        : "Хорошо. Что из этого хотелось бы изменить в первую очередь?";

    public static string RecapNo(bool english) => english
        ? "Thank you for correcting me. What should be adjusted?"
        : "Спасибо, что поправляете. Что стоит уточнить?";

    public static string Callback(string quote, bool english) => english
        ? $"At the start you said: “{quote}”. Has anything changed since then?"
        : $"В начале вы говорили: «{quote}». Что-нибудь изменилось с тех пор?";

    // ----- questions that use who the conversation is about -----

    private static readonly Dictionary<(CompanionEmotion, CompanionPerson), (string Ru, string En)> Targeted = new()
    {
        [(CompanionEmotion.Anger, CompanionPerson.Boss)] = ("Как вы обычно реагируете, когда начальник так поступает: промолчите или скажете вслух?", "How do you usually react when your boss does that: stay quiet or say it aloud?"),
        [(CompanionEmotion.Anger, CompanionPerson.Colleague)] = ("Это повторяется или случилось впервые?", "Is this a pattern, or did it happen for the first time?"),
        [(CompanionEmotion.Anger, CompanionPerson.Partner)] = ("Что для вас важно в этой ситуации, о чём вы хотели бы сказать партнёру?", "What matters to you here that you would like to tell your partner?"),
        [(CompanionEmotion.Anger, CompanionPerson.Parent)] = ("Что именно в словах или поступках родителей задевает вас сильнее всего?", "What exactly in your parents' words or actions hits you the hardest?"),
        [(CompanionEmotion.Resentment, CompanionPerson.Partner)] = ("Говорили ли вы партнёру, что вам это важно и больно?", "Have you told your partner that this matters to you and hurts?"),
        [(CompanionEmotion.Resentment, CompanionPerson.Friend)] = ("Чего вам не хватило от друга: внимания, поддержки, уважения?", "What was missing from your friend: attention, support, respect?"),
        [(CompanionEmotion.Resentment, CompanionPerson.Boss)] = ("Чего вам не хватило от руководителя: признания, справедливости, уважения?", "What was missing from your manager: recognition, fairness, respect?"),
        [(CompanionEmotion.Resentment, CompanionPerson.Parent)] = ("Что вы хотели бы услышать от родителя в этой ситуации?", "What would you have liked to hear from your parent here?"),
        [(CompanionEmotion.Guilt, CompanionPerson.Child)] = ("Что бы вы хотели сказать ребёнку, если бы могли вернуться на минуту назад?", "What would you like to tell your child if you could go back a minute?"),
        [(CompanionEmotion.Guilt, CompanionPerson.Parent)] = ("Что бы вы хотели сказать этому человеку сейчас?", "What would you like to say to this person now?"),
        [(CompanionEmotion.Anxiety, CompanionPerson.Boss)] = ("Что в реакции руководства пугает вас больше всего?", "What about your manager's possible reaction scares you the most?"),
        [(CompanionEmotion.Anxiety, CompanionPerson.Child)] = ("Что именно вас тревожит за ребёнка: здоровье, учёба, безопасность?", "What worries you about your child: health, school, safety?"),
        [(CompanionEmotion.Sadness, CompanionPerson.Partner)] = ("Чего вам сейчас больше всего не хватает из того, что было в отношениях?", "What do you miss most from what the relationship gave you?"),
        [(CompanionEmotion.Loneliness, CompanionPerson.Friend)] = ("Когда вы в последний раз чувствовали настоящую близость с кем-то?", "When did you last feel real closeness with someone?")
    };

    private static readonly Dictionary<CompanionPerson, (string Ru, string En)> PersonFallback = new()
    {
        [CompanionPerson.Boss] = ("Как эта ситуация с руководителем отражается на вашей работе и самочувствии?", "How does this situation with your manager affect your work and how you feel?"),
        [CompanionPerson.Colleague] = ("Как эта ситуация с коллегами влияет на ваши рабочие дни?", "How does this situation with colleagues affect your workdays?"),
        [CompanionPerson.Partner] = ("Как это отражается на ваших отношениях?", "How does this show up in your relationship?"),
        [CompanionPerson.Parent] = ("Как давно в отношениях с родителями так?", "How long has it been like this with your parents?"),
        [CompanionPerson.Child] = ("Как это связано с тем, что вы чувствуете как родитель?", "How is this connected with what you feel as a parent?"),
        [CompanionPerson.Friend] = ("Насколько для вас важен этот человек?", "How important is this person to you?"),
        [CompanionPerson.Relative] = ("Как этот человек связан с тем, что вы сейчас чувствуете?", "How is this person connected with what you feel now?")
    };

    /// <summary>Id of a question that fits both the feeling and the person, or null when there is none.</summary>
    public static string? TargetedQuestionId(CompanionEmotion emotion, CompanionPerson? person, IReadOnlyList<string> asked)
    {
        if (person is not { } p)
        {
            return null;
        }

        string specific = $"t:{emotion}:{p}";
        if (Targeted.ContainsKey((emotion, p)) && !asked.Contains(specific))
        {
            return specific;
        }

        string fallback = $"t:*:{p}";
        return !asked.Contains(fallback) ? fallback : null;
    }

    public static string? TargetedQuestion(string id, bool english)
    {
        string[] parts = id.Split(':');
        if (parts.Length != 3 || parts[0] != "t" || !Enum.TryParse(parts[2], out CompanionPerson person))
        {
            return null;
        }

        (string Ru, string En) text;
        if (parts[1] == "*")
        {
            text = PersonFallback[person];
        }
        else if (Enum.TryParse(parts[1], out CompanionEmotion emotion) && Targeted.TryGetValue((emotion, person), out text))
        {
        }
        else
        {
            return null;
        }

        return english ? text.En : text.Ru;
    }

    public static string PersonName(CompanionPerson person, bool english) => person switch
    {
        CompanionPerson.Boss => english ? "boss" : "руководитель",
        CompanionPerson.Colleague => english ? "colleagues" : "коллеги",
        CompanionPerson.Partner => english ? "partner" : "партнёр",
        CompanionPerson.Parent => english ? "parents" : "родители",
        CompanionPerson.Child => english ? "child" : "ребёнок",
        CompanionPerson.Friend => english ? "friend" : "друг",
        _ => english ? "family" : "близкие"
    };

    private static string Pick(Random random, string[] variants) => variants[random.Next(variants.Length)];
}
