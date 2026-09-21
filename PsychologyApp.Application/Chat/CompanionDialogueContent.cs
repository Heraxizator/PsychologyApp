using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>Reviewed wording of the messenger companion: questions, acknowledgements, check-ins. Never addresses the person by gender.</summary>
public static class CompanionDialogueContent
{
    // Question ids, in the order each state prefers to explore them.
    private static readonly Dictionary<CompanionEmotion, string[]> QuestionOrder = new()
    {
        [CompanionEmotion.Panic] = ["body", "trigger", "need"],
        [CompanionEmotion.Anxiety] = ["trigger", "worst_case", "body", "need"],
        [CompanionEmotion.Overthinking] = ["thought", "need", "trigger"],
        [CompanionEmotion.Anger] = ["trigger", "need", "body"],
        [CompanionEmotion.Resentment] = ["trigger", "need", "meaning"],
        [CompanionEmotion.Guilt] = ["what_happened", "kind", "need"],
        [CompanionEmotion.Sadness] = ["onset", "need", "support"],
        [CompanionEmotion.Exhaustion] = ["onset", "rest", "need"],
        [CompanionEmotion.Loneliness] = ["support", "need", "onset"],
        [CompanionEmotion.Procrastination] = ["obstacle", "smallest_step", "need"],
        [CompanionEmotion.Unknown] = ["open1", "open2", "open3"]
    };

    private static readonly string[] Generic = ["meaning", "more", "need"];

    /// <summary>Next question this person has not been asked yet, preferring the ones that fit their state.</summary>
    public static string? NextQuestionId(CompanionEmotion emotion, IReadOnlyList<string> asked)
    {
        foreach (string id in QuestionOrder[emotion].Concat(Generic))
        {
            if (!asked.Contains(id))
            {
                return id;
            }
        }

        return null;
    }

    public static string Question(string id, bool english) => CompanionActContent.TargetedQuestion(id, english) ?? BankQuestion(id, english);

    private static string BankQuestion(string id, bool english) => id switch
    {
        "trigger" => english ? "What happened that set this off?" : "Что произошло, из-за чего это началось?",
        "worst_case" => english ? "What exactly are you afraid might happen?" : "Чего именно вы боитесь? Что, по-вашему, может случиться?",
        "body" => english ? "Where do you feel it in your body: chest, stomach, shoulders?" : "Где в теле вы это чувствуете: в груди, в животе, в плечах?",
        "thought" => english ? "Which thought keeps coming back the most?" : "Какая мысль возвращается чаще всего?",
        "need" => english ? "What would you most like right now?" : "Чего бы вам сейчас больше всего хотелось?",
        "what_happened" => english ? "Tell me what happened, and what you are telling yourself about it." : "Расскажите, что случилось и что вы говорите себе по этому поводу.",
        "kind" => english ? "What would you say to a close friend in the same situation?" : "Что бы вы сказали близкому другу, окажись он в такой же ситуации?",
        "onset" => english ? "When did this start? Has anything changed lately?" : "Когда это началось? Что-то изменилось в последнее время?",
        "support" => english ? "Is there someone close you could share this with?" : "Есть ли рядом человек, с которым вы могли бы этим поделиться?",
        "rest" => english ? "When did you last really rest?" : "Когда вы в последний раз по-настоящему отдыхали?",
        "obstacle" => english ? "What gets in the way of starting? What feels hardest about it?" : "Что мешает начать? Что в этом деле кажется самым тяжёлым?",
        "smallest_step" => english ? "What is the smallest step you could take in five minutes?" : "Какой самый маленький шаг вы могли бы сделать за пять минут?",
        "meaning" => english ? "What is the hardest part of this for you?" : "Что для вас в этом самое трудное?",
        "more" => english ? "I'm listening. What else matters?" : "Я слушаю. Что ещё важно?",
        "open1" => english ? "Tell me a bit more: what is the hardest part of this situation?" : "Расскажите чуть подробнее: что в этой ситуации самое тяжёлое?",
        "open2" => english ? "What do you feel when you think about it?" : "Что вы чувствуете, когда думаете об этом?",
        "open3" => english ? "How is it affecting your day and how you feel?" : "Как это сказывается на вашем дне и самочувствии?",
        _ => english ? "I'm listening." : "Я слушаю."
    };

    public static string ScaleQuestion(bool english) => english
        ? "How strong is the tension right now, from 0 to 10?"
        : "Насколько сильное напряжение сейчас, от 0 до 10?";

    public static string Acknowledgement(bool english, Random random) => Pick(random, english
        ? ["I understand.", "Thank you for telling me.", "I hear you.", "That is not easy.", "It's good that you're talking about it.", "I'm with you."]
        : ["Понимаю.", "Спасибо, что рассказываете.", "Слышу вас.", "Это непросто.", "Хорошо, что вы об этом говорите.", "Я с вами."]);

    public static string QuoteLine(string quote, bool english, Random random) => Pick(random, english
        ? [$"You write: “{quote}”.", $"“{quote}”. I hear that."]
        : [$"Вы пишете: «{quote}».", $"«{quote}». Я слышу это."]);

    public static string ScaleHigh(bool english) => english
        ? "That is very strong tension. Let's start with something that helps the body."
        : "Это очень сильное напряжение. Давайте начнём с того, что помогает телу.";

    public static string ScaleMedium(bool english) => english ? "I understand, that is noticeable." : "Понимаю, это ощутимо.";

    public static string ScaleLow(bool english) => english ? "It's good that it isn't at its limit." : "Хорошо, что это не на пределе.";

    public static string UrgentBridge(bool english) => english
        ? "Let's help your body settle first, then we can talk."
        : "Давайте сначала поможем телу успокоиться, а потом поговорим.";

    public static string PracticeStarted(bool english) => english
        ? "Okay, let's begin. When you're done, come back here and tell me how you are."
        : "Хорошо, начинаем. Когда закончите, вернитесь сюда и расскажите, как вы.";

    public static string EmotionPrompt(bool english) => english
        ? "I want to understand you better. Which of these is closest?"
        : "Хочу понять вас точнее. Что из этого ближе всего?";

    public static string FollowUpAfterPractice(bool english) => english
        ? "Welcome back. How are you now? Rate the tension from 0 to 10."
        : "С возвращением. Как вы сейчас? Оцените напряжение от 0 до 10.";

    public static string PostPracticeBetter(int before, int after, bool english) => english
        ? $"It was {before}, now {after}: a real drop. What do you think helped?"
        : $"Было {before}, стало {after}. Это заметное снижение. Что, по-вашему, помогло?";

    public static string PostPracticeSame(int now, bool english) => english
        ? $"The tension is still around {now}. Sometimes one practice is not enough. Would you like to try another, or talk?"
        : $"Напряжение всё ещё около {now}. Иногда одной практики мало. Хотите попробовать другую или поговорить?";

    public static string PostPracticeWorse(int before, int after, bool english) => english
        ? $"It went up, from {before} to {after}. That happens when a hard topic is touched, and it is not your fault. If it stays heavy, please reach out to someone close or a professional."
        : $"Стало сильнее: было {before}, сейчас {after}. Так бывает, когда касаешься трудной темы, и это не ваша ошибка. Если тяжесть не уходит, обратитесь к близкому человеку или специалисту.";

    public static string TalkLabel(bool english) => english ? "Let's talk" : "Поговорить";

    public static string AnotherPracticeLabel(bool english) => english ? "Another practice" : "Другая практика";

    // ----- returning to the app: memory of the previous chat -----

    public static string ReturningGreeting(string previousTitle, int daysAgo, int? lastIntensity, bool english)
    {
        string when = RelativeDay(daysAgo, english);
        string intensity = lastIntensity is { } value
            ? (english ? $" Back then the tension was {value}/10." : $" Напряжение тогда было {value}/10.")
            : string.Empty;
        return english
            ? $"Welcome back. Last time ({when}) we talked about “{previousTitle}”.{intensity} How is it now?"
            : $"Здравствуйте снова. В прошлый раз ({when}) вы говорили о теме «{previousTitle}».{intensity} Как это сейчас?";
    }

    public static IReadOnlyList<ChatQuickReply> CheckInReplies(bool english) =>
    [
        new(ChatQuickReplyKinds.CheckIn, english ? "It got easier" : "Стало легче", "better"),
        new(ChatQuickReplyKinds.CheckIn, english ? "About the same" : "Без изменений", "same"),
        new(ChatQuickReplyKinds.CheckIn, english ? "It got harder" : "Стало тяжелее", "worse"),
        new(ChatQuickReplyKinds.CheckIn, english ? "Something else" : "О другом", "other")
    ];

    public static string CheckInResponse(string answer, bool english) => answer switch
    {
        "better" => english ? "That is good news. What do you think helped?" : "Это хорошая новость. Что, по-вашему, помогло?",
        "same" => english ? "I understand. What feels most noticeable right now?" : "Понимаю. Что сейчас ощущается сильнее всего?",
        "worse" => english ? "I'm sorry it got harder. Tell me what has happened since then." : "Мне жаль, что стало тяжелее. Расскажите, что случилось с тех пор.",
        _ => english ? "I'm listening. What would you like to talk about?" : "Слушаю вас. О чём хотите поговорить?"
    };

    public static string RelativeDay(int daysAgo, bool english) => daysAgo switch
    {
        <= 0 => english ? "today" : "сегодня",
        1 => english ? "yesterday" : "вчера",
        <= 6 => english ? $"{daysAgo} days ago" : $"{daysAgo} {DaysWord(daysAgo)} назад",
        <= 13 => english ? "last week" : "на прошлой неделе",
        _ => english ? "a while ago" : "давно"
    };

    private static string DaysWord(int n) => (n % 10, n % 100) switch
    {
        (1, not 11) => "день",
        (>= 2 and <= 4, not (>= 12 and <= 14)) => "дня",
        _ => "дней"
    };

    // ----- chat titles -----

    public static string Title(CompanionEmotion emotion, CompanionTheme? theme, string firstText, bool english)
    {
        if (emotion == CompanionEmotion.Unknown)
        {
            string trimmed = firstText.Trim().TrimEnd('.', '!', '?');
            return trimmed.Length <= 28 ? trimmed : trimmed[..28].TrimEnd() + "…";
        }

        string name = CompanionContent.EmotionName(emotion, english);
        string title = char.ToUpperInvariant(name[0]) + name[1..];
        return theme is { } t ? $"{title} · {CompanionContent.ThemeName(t, english)}" : title;
    }

    private static string Pick(Random random, string[] variants) => variants[random.Next(variants.Length)];
}
