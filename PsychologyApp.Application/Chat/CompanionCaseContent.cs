using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// The "case card": what the person told us happened, and the wording that answers the situation itself (a reflection and a question that fit it),
/// the question about what they need from the conversation, and follow-ups that depend on what was just asked.
/// </summary>
public static class CompanionCaseContent
{
    public const string GoalQuestionId = "goal";
    public const string GoalVent = "vent";
    public const string GoalUnderstand = "understand";
    public const string GoalCalm = "calm";

    private sealed record EventText(string LineRu, string LineEn, string AskRu, string AskEn, string YesRu, string YesEn, string NoRu, string NoEn);

    private static readonly Dictionary<CompanionEvent, EventText> Texts = new()
    {
        [CompanionEvent.Conflict] = new(
            "Ссора с близким человеком ранит сильнее, чем с чужим: за ней стоит многое.",
            "A quarrel with someone close hurts more than one with a stranger: there is a lot behind it.",
            "Это случилось впервые или такое повторяется?",
            "Was this the first time, or does it keep happening?",
            "Если повторяется, это объясняет, почему так тяжело: усталость копится. Что обычно запускает такие ссоры?",
            "If it keeps happening, that explains why it is so heavy: the weariness builds up. What usually sets these quarrels off?",
            "Тогда понятно, почему это так потрясло: это было неожиданно. Что, по-вашему, стояло за его словами?",
            "Then it makes sense that it shook you: it came out of the blue. What do you think was behind the words?"),
        [CompanionEvent.Humiliation] = new(
            "Когда это случается при людях, к боли добавляется стыд, и он держится дольше самой обиды.",
            "When it happens in front of people, shame joins the hurt and lasts longer than the hurt itself.",
            "Вы кому-нибудь уже рассказывали, как это было?",
            "Have you told anyone yet how it was?",
            "Хорошо, что есть с кем разделить. Что вам сейчас больше всего нужно от этого человека?",
            "Good that you have someone to share it with. What do you most need from that person right now?",
            "Значит, вы несли это в одиночку. Что вам хотелось бы сказать, если бы можно было всё повернуть назад?",
            "So you have been carrying this alone. What would you want to say if you could go back?"),
        [CompanionEvent.Breakup] = new(
            "Расставание похоже на потерю: вместе с человеком уходит привычная жизнь, и грустить об этом нормально.",
            "A breakup is a kind of loss: the familiar life goes with the person, and it is normal to grieve it.",
            "Как давно это произошло?",
            "How long ago did this happen?",
            "Спасибо. Что сейчас труднее всего: вечера, мысли о нём или неопределённость?",
            "Thank you. What is hardest right now: the evenings, thinking about them, or the uncertainty?",
            "Спасибо. Что сейчас труднее всего: вечера, мысли о нём или неопределённость?",
            "Thank you. What is hardest right now: the evenings, thinking about them, or the uncertainty?"),
        [CompanionEvent.JobLoss] = new(
            "Потеря работы бьёт сразу по нескольким опорам: деньги, распорядок и ощущение своей ценности.",
            "Losing a job hits several supports at once: money, routine, and the sense of your own worth.",
            "Что тревожит больше всего: деньги, будущее или то, что подумают другие?",
            "What worries you most: the money, the future, or what others will think?",
            "Понимаю. Давайте возьмём одну вещь из этого. Что из этого можно сделать в ближайшие два дня?",
            "I see. Let's take one piece of it. What could be done in the next two days?",
            "Понимаю. Давайте возьмём одну вещь из этого. Что из этого можно сделать в ближайшие два дня?",
            "I see. Let's take one piece of it. What could be done in the next two days?"),
        [CompanionEvent.HealthWorry] = new(
            "Ожидание результатов тяжелее самих результатов: воображение рисует самое страшное.",
            "Waiting for results is harder than the results themselves: the imagination draws the worst.",
            "Когда вы узнаете результат и есть ли рядом кто-то, кто побудет с вами до этого?",
            "When will you know the result, and is there someone who could be with you until then?",
            "Хорошо, что вы не одни. Что вы себе говорите в самые тревожные минуты ожидания?",
            "Good that you are not alone. What do you tell yourself in the most anxious minutes of waiting?",
            "Тогда здесь можно быть тем, с кем можно об этом говорить. Что тревожит больше всего?",
            "Then here you can talk about it with someone. What worries you most?"),
        [CompanionEvent.CaringForIll] = new(
            "Ухаживать за близким человеком — это и любовь, и огромная нагрузка, и усталость тут не предательство.",
            "Caring for someone close is both love and a huge load, and being tired is not a betrayal.",
            "Помогает ли вам кто-нибудь с уходом?",
            "Does anyone help you with the care?",
            "Это важно, ведь опора нужна и вам. Удаётся ли вам хоть немного отдыхать?",
            "That matters, since you need support too. Do you manage to rest at all?",
            "Тогда понятно, почему сил не остаётся. Как давно вы несёте это одни?",
            "Then it is clear why the strength runs out. How long have you been carrying this alone?"),
        [CompanionEvent.Failure] = new(
            "Неудача больно задевает, особенно когда человек много вложил.",
            "A failure stings, especially when you put a lot in.",
            "Что вы сейчас говорите себе об этом?",
            "What are you telling yourself about it now?",
            "Спасибо. А если бы это случилось с другом, вы сказали бы ему то же самое?",
            "Thank you. If it had happened to a friend, would you say the same to them?",
            "Спасибо. А если бы это случилось с другом, вы сказали бы ему то же самое?",
            "Thank you. If it had happened to a friend, would you say the same to them?"),
        [CompanionEvent.Betrayal] = new(
            "Предательство разрушает доверие, а вместе с ним и ощущение, что мир надёжен.",
            "Betrayal breaks trust, and with it the sense that the world is safe.",
            "Вы уже говорили с этим человеком о случившемся?",
            "Have you spoken to this person about what happened?",
            "Как он отреагировал? И что после этого разговора стало лучше или хуже?",
            "How did they react? And what got better or worse after that talk?",
            "Что вас останавливает: страх услышать ответ или не знаете, с чего начать?",
            "What holds you back: fear of the answer, or not knowing where to start?"),
        [CompanionEvent.Overload] = new(
            "Когда дел слишком много, кажется, что всё срочно, и это само по себе изматывает.",
            "When there is too much to do, everything feels urgent, and that alone wears you out.",
            "Можно ли что-то из этого отложить или передать другому?",
            "Is there anything that could be postponed or handed to someone else?",
            "Тогда начнём с этого: что из списка можно убрать первым?",
            "Then let's start there: what could come off the list first?",
            "Тогда давайте выберем самое важное. Какое одно дело нужно сделать до конца недели?",
            "Then let's pick what matters most. What one thing must be done by the end of the week?"),
        [CompanionEvent.Sleeplessness] = new(
            "Без сна всё кажется тяжелее: и мысли, и тело, и настроение.",
            "Without sleep everything feels heavier: thoughts, body and mood.",
            "Это уже давно или началось недавно?",
            "Has this been going on for a long time, or did it start recently?",
            "Тогда это стоит обсудить с врачом, а пока можно попробовать вечернюю практику. Хотите?",
            "Then it is worth talking to a doctor, and meanwhile you can try an evening practice. Want to?",
            "Тогда, возможно, дело в нынешних переживаниях. Что крутится в голове перед сном?",
            "Then it may be about what you are going through now. What spins in your head before sleep?"),
        [CompanionEvent.Performance] = new(
            "Перед важным выступлением волнуются почти все: это значит, что вам не всё равно.",
            "Almost everyone is nervous before something important: it means you care.",
            "Чего вы боитесь больше всего: забыть слова, оценки или не справиться?",
            "What are you most afraid of: forgetting the words, being judged, or not coping?",
            "Давайте тогда подготовим именно это. Что бы вам помогло почувствовать себя увереннее?",
            "Then let's prepare for exactly that. What would help you feel more confident?",
            "Давайте тогда подготовим именно это. Что бы вам помогло почувствовать себя увереннее?",
            "Then let's prepare for exactly that. What would help you feel more confident?")
    };

    public static string? EventLine(CompanionEvent kind, bool english) =>
        Texts.TryGetValue(kind, out EventText? t) ? (english ? t.LineEn : t.LineRu) : null;

    public static string EventQuestionId(CompanionEvent kind) => $"e:{kind}";

    /// <summary>The id of the question that fits the situation, if it has not been asked yet.</summary>
    public static string? EventQuestionIdIfNew(CompanionEvent kind, IReadOnlyList<string> asked) =>
        Texts.ContainsKey(kind) && !asked.Contains(EventQuestionId(kind)) ? EventQuestionId(kind) : null;

    public static string? EventQuestion(string id, bool english) =>
        id.StartsWith("e:", StringComparison.Ordinal) && Enum.TryParse(id[2..], out CompanionEvent kind) && Texts.TryGetValue(kind, out EventText? t)
            ? (english ? t.AskEn : t.AskRu)
            : null;

    public static CompanionEvent ParseEvent(string? name) => Enum.TryParse(name, out CompanionEvent kind) ? kind : CompanionEvent.None;

    /// <summary>The question that fits the answer "yes" / "no" to the question just asked, or null when a generic follow-up will do.</summary>
    public static string? FollowUp(string? questionId, bool yes, bool english)
    {
        if (questionId is null)
        {
            return null;
        }

        if (questionId == "support")
        {
            return yes
                ? (english ? "Good that there is someone. What would you tell them first?" : "Хорошо, что есть кто-то рядом. Что бы вы сказали ему в первую очередь?")
                : (english ? "Then it matters even more that you are saying it here. What would help you get through today?" : "Тогда тем важнее, что вы говорите об этом здесь. Что помогло бы вам пережить сегодняшний день?");
        }

        if (questionId.StartsWith("e:", StringComparison.Ordinal) && Enum.TryParse(questionId[2..], out CompanionEvent kind) && Texts.TryGetValue(kind, out EventText? t))
        {
            return english ? (yes ? t.YesEn : t.NoEn) : (yes ? t.YesRu : t.NoRu);
        }

        return null;
    }

    // ----- what the person needs from the conversation -----

    public static string GoalQuestion(bool english) => english
        ? "Before we go on: what do you need most right now: to get it off your chest, to sort things out, or to calm down?"
        : "Прежде чем двигаться дальше: что вам сейчас нужнее: выговориться, разобраться или успокоиться?";

    public static IReadOnlyList<ChatQuickReply> GoalChips(bool english) =>
    [
        new(ChatQuickReplyKinds.Act, english ? "Get it off my chest" : "Выговориться", $"goal:{GoalVent}"),
        new(ChatQuickReplyKinds.Act, english ? "Sort things out" : "Разобраться", $"goal:{GoalUnderstand}"),
        new(ChatQuickReplyKinds.Act, english ? "Calm down" : "Успокоиться", $"goal:{GoalCalm}")
    ];

    private static readonly string[] VentWords = ["выговор", "поговорить", "высказ", "излить", "послушай", "просто рассказ", "get it off", "vent", "just talk", "let it out", "off my chest"];
    private static readonly string[] UnderstandWords = ["разобрат", "понять", "разбор", "что делать", "sort", "figure", "understand", "make sense", "what to do"];
    private static readonly string[] CalmWords = ["успокоит", "расслабит", "полегч", "прийти в себя", "calm", "relax", "settle", "ease"];

    /// <summary>Reads a typed answer to the goal question.</summary>
    public static string? ParseGoal(string text)
    {
        string t = text.ToLowerInvariant();
        if (CalmWords.Any(t.Contains))
        {
            return GoalCalm;
        }

        if (UnderstandWords.Any(t.Contains))
        {
            return GoalUnderstand;
        }

        return VentWords.Any(t.Contains) ? GoalVent : null;
    }

    public static string GoalVentReply(bool english) => english
        ? "Then I will just listen. No questions unless you want them: tell me what you need to say."
        : "Тогда я просто слушаю. Без лишних вопросов: говорите всё, что нужно.";

    public static string GoalUnderstandReply(bool english) => english
        ? "Then let's go through it step by step."
        : "Тогда давайте разбираться по шагам.";

    public static string GoalCalmReply(bool english) => english
        ? "Then let's start with the body: it is quicker than words."
        : "Тогда начнём с тела: так быстрее, чем словами.";

    private static readonly string[] DurationCues =
    [
        "вчера", "сегодня", "утром", "на днях", "недавно", "давно", "месяц", "недел", "годами", "лет ", "минут", "часа",
        "yesterday", "today", "this morning", "recently", "last week", "months", "weeks", "for years", "ago"
    ];

    /// <summary>The message already says when it happened or how long it lasts, so "when did this start?" would be a question about nothing.</summary>
    public static bool MentionsWhen(string text)
    {
        string t = text.ToLowerInvariant();
        return DurationCues.Any(t.Contains);
    }
}
