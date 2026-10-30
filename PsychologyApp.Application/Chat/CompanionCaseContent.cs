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
            "Тогда понятно, почему это так потрясло: это было неожиданно. Что, по-вашему, стояло за этими словами?",
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

    private static readonly HashSet<CompanionEvent> YesNoQuestions = [CompanionEvent.Humiliation, CompanionEvent.CaringForIll, CompanionEvent.Betrayal, CompanionEvent.Overload];

    /// <summary>
    /// The line that answers what happened. A quarrel with a boss or a colleague is not "a quarrel with someone close": there it is
    /// the dependence and having to keep meeting that make it heavy.
    /// </summary>
    public static string? EventLine(CompanionEvent kind, bool english, CompanionPerson? person = null)
    {
        if (kind == CompanionEvent.Conflict && IsAtWork(person))
        {
            return english
                ? "A clash with a boss or a colleague is heavy in its own way: you cannot just walk away, and tomorrow you meet again."
                : "Конфликт с руководителем или коллегой тяжёл по-своему: от него не уйдёшь, а завтра снова встречаться.";
        }

        return Texts.TryGetValue(kind, out EventText? t) ? (english ? t.LineEn : t.LineRu) : null;
    }

    private static bool IsAtWork(CompanionPerson? person) => person is CompanionPerson.Boss or CompanionPerson.Colleague;

    public static string EventQuestionId(CompanionEvent kind) => $"e:{kind}";

    /// <summary>The id of the question that fits the situation, if it has not been asked yet.</summary>
    public static string? EventQuestionIdIfNew(CompanionEvent kind, IReadOnlyList<string> asked) =>
        Texts.ContainsKey(kind) && !asked.Contains(EventQuestionId(kind)) ? EventQuestionId(kind) : null;

    public static string? EventQuestion(string id, bool english) =>
        id.StartsWith("e:", StringComparison.Ordinal) && Enum.TryParse(id[2..], out CompanionEvent kind) && Enum.IsDefined(kind) && Texts.TryGetValue(kind, out EventText? t)
            ? (english ? t.AskEn : t.AskRu)
            : null;

    public static CompanionEvent ParseEvent(string? name) => Enum.TryParse(name, out CompanionEvent kind) && Enum.IsDefined(kind) ? kind : CompanionEvent.None;

    /// <summary>The question that fits the answer "yes" / "no" to the question just asked, or null when a generic follow-up will do.</summary>
    public static string? FollowUp(string? questionId, bool yes, bool english, bool force = false)
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

        if (questionId.StartsWith("e:", StringComparison.Ordinal) && Enum.TryParse(questionId[2..], out CompanionEvent kind) && Enum.IsDefined(kind) && Texts.TryGetValue(kind, out EventText? t))
        {
            // "Yes" to "what worries you most: money, the future or others?" answers nothing: ask for the choice, do not pretend it was understood.
            if (!force && !YesNoQuestions.Contains(kind))
            {
                return english ? $"Let me ask again, in a word or two: {Texts[kind].AskEn}" : $"Уточню: ответьте коротко. {Texts[kind].AskRu}";
            }

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

    private static readonly string[] VentWords = ["выговор", "поговорить", "высказ", "излить", "послушай", "выслуш", "послуша", "чтоб кто-то", "чтобы кто-то", "просто рассказ", "get it off", "vent", "just talk", "let it out", "off my chest"];
    private static readonly string[] UnderstandWords = ["разобрат", "понять", "разбор", "что делать", "sort", "figure", "understand", "make sense", "what to do"];
    private static readonly string[] CalmWords = ["успокоиться", "успокаиваться", "расслабиться", "полегч", "прийти в себя", "calm down", "calm myself", "relax", "settle", "ease"];

    private static readonly string[] WantPhrases = ["хочу", "хотела бы", "хотел бы", "хотела", "хотел", "нужно", "надо", "давай", "помоги", "i want", "i need", "help me", "let me", "can we"];

    private static readonly System.Text.RegularExpressions.Regex GoalClauseSeparators = new(
        "[,;.!?…\\r\\n]+",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

    private static readonly System.Text.RegularExpressions.Regex GoalTextDelimiters = new(
        @"[^\p{L}\p{N}|]+",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

    private static readonly System.Text.RegularExpressions.Regex OtherPersonCalmingPattern = new(
        @"(?:успокоить|calm(?:\s+down)?)\s+(?:его|ее|их|сына|дочь\w*|ребен\w*|мам\w*|пап\w*|друг\w*|подруг\w*|коллег\w*|партнер\w*|my\s+\w+|him|her|them|the\s+\w+)|(?:my\s+)?(?:son|daughter|child|friend|partner)\s+(?:(?:to|should|needs?\s+to)\s+)?calm(?:\s+down)?",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);

    private static readonly Dictionary<string, System.Text.RegularExpressions.Regex> GoalTermPatterns =
        VentWords.Concat(UnderstandWords).Concat(CalmWords).Concat(WantPhrases)
            .Distinct(StringComparer.Ordinal)
            .ToDictionary(term => term, CreateGoalTermPattern, StringComparer.Ordinal);

    /// <summary>Reads an explicit support preference, including a typed answer to the goal question.</summary>
    public static string? ParseGoal(string text, bool asked)
    {
        string normalized = NormalizeGoalText(text);
        if (normalized.Length == 0)
        {
            return null;
        }

        // A request to listen is explicit even before the companion asks what kind of support is wanted.
        if (ContainsGoalPhrase(normalized, DirectListeningRequests))
        {
            return GoalVent;
        }

        if (!asked && !ContainsUnnegatedGoalTerm(normalized, WantPhrases))
        {
            return null;
        }

        bool wantsToVent = ContainsUnnegatedGoalTerm(normalized, VentWords);
        bool wantsToUnderstand = ContainsUnnegatedGoalTerm(normalized, UnderstandWords);
        bool wantsToCalm = ContainsUnnegatedGoalTerm(normalized, CalmWords)
            && !MentionsCalmingSomeoneElse(normalized);

        // An explicit wish to be heard is stronger than a conflicting wish to calm down; otherwise do not guess between goals.
        if (wantsToVent)
        {
            return GoalVent;
        }

        if (wantsToUnderstand && wantsToCalm)
        {
            return null;
        }

        if (wantsToUnderstand)
        {
            return GoalUnderstand;
        }

        return wantsToCalm ? GoalCalm : null;
    }

    private static readonly string[] DirectListeningRequests =
    [
        "просто послушай", "просто выслушай", "послушай меня", "выслушай меня",
        "не надо советов", "не нужно советов", "не давай советов", "не хочу советов",
        "без советов", "не предлагай решений", "не предлагай практику",
        "just listen", "listen to me", "hear me out", "no advice", "dont give advice",
        "dont give me advice", "no solutions", "just be here"
    ];

    private static string NormalizeGoalText(string text)
    {
        string clauses = GoalClauseSeparators.Replace(text.ToLowerInvariant().Replace('ё', 'е'), " | ");
        return GoalTextDelimiters.Replace(clauses, " ").Trim();
    }

    private static bool ContainsGoalPhrase(string text, IEnumerable<string> phrases)
    {
        string padded = $" {text} ";
        return phrases.Any(phrase => padded.Contains($" {NormalizeGoalText(phrase)} ", StringComparison.Ordinal));
    }

    private static bool ContainsUnnegatedGoalTerm(string text, IEnumerable<string> terms)
    {
        foreach (string term in terms)
        {
            foreach (System.Text.RegularExpressions.Match match in GoalTermPatterns[term].Matches(text))
            {
                string context = text[..match.Index];
                int clauseStart = context.LastIndexOf('|');
                if (clauseStart >= 0)
                {
                    context = context[(clauseStart + 1)..];
                }

                string[] preceding = context.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (!preceding.TakeLast(3).Any(word => word is "не" or "ни" or "нет" or "not" or "never" or "dont"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static System.Text.RegularExpressions.Regex CreateGoalTermPattern(string term)
    {
        string[] parts = NormalizeGoalText(term).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string pattern = $@"(?<![\p{{L}}\p{{N}}]){string.Join(@"\s+", parts.Select(System.Text.RegularExpressions.Regex.Escape))}[\p{{L}}\p{{N}}]*(?![\p{{L}}\p{{N}}])";
        return new System.Text.RegularExpressions.Regex(
            pattern,
            System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled);
    }

    private static bool MentionsCalmingSomeoneElse(string text) => OtherPersonCalmingPattern.IsMatch(text);

    public static string GoalVentReply(bool english, Random? random = null)
    {
        string[] variants = english
            ? [
                "Then I will just listen. No questions unless you want them: tell me what you need to say.",
                "We do not have to solve anything right now. You can tell me what has been building up.",
                "Okay. No advice or decisions for now; take your time and tell me what is on your mind."
            ]
            : [
                "Тогда я просто слушаю. Без лишних вопросов: говорите всё, что нужно.",
                "Сейчас не обязательно что-то решать. Можно просто рассказать, что накопилось.",
                "Хорошо. Пока без советов и решений — рассказывайте в своём темпе."
            ];

        return variants[(random ?? Random.Shared).Next(variants.Length)];
    }

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

    // ----- the person has decided what to do -----

    private static readonly string[] PlanPhrases =
    [
        "поговорю", "попробую", "скажу ей", "скажу ему", "напишу ей", "напишу ему", "позвоню", "решила", "решил ", "пойду к", "запишусь",
        "i will talk", "i'll talk", "i'll try", "i will try", "i decided", "i'll call", "i'll write", "i'll tell"
    ];

    /// <summary>"Maybe I'll talk to her": a step the person has taken in their mind. The companion backs it instead of offering a practice.</summary>
    public static bool StatesAPlan(string text)
    {
        string t = text.ToLowerInvariant();
        return PlanPhrases.Any(t.Contains);
    }

    public static string PlanReply(bool english) => english
        ? "That sounds like a real step, and it is yours to take. What would make it a little easier to start?"
        : "Это похоже на настоящий шаг, и решать его вам. Что могло бы помочь начать чуть спокойнее?";

    private static readonly Dictionary<CompanionEvent, (string[] Ru, string[] En)> Acknowledgements = new()
    {
        [CompanionEvent.Conflict] = (["Слова близких запоминаются надолго.", "Когда ссорятся с близким, болит вдвойне."], ["Words from someone close stay with us.", "A quarrel with someone close hurts twice."]),
        [CompanionEvent.Breakup] = (["Расставание переворачивает привычный день.", "После такого трудно собраться."], ["A breakup turns the whole day upside down.", "It is hard to pull yourself together after that."]),
        [CompanionEvent.JobLoss] = (["Потерять работу — значит потерять почву под ногами.", "Такая неопределённость выматывает."], ["Losing a job feels like losing the ground under you.", "That uncertainty is draining."]),
        [CompanionEvent.CaringForIll] = (["Вы несёте очень много.", "Заботиться о другом и забывать о себе — тяжело."], ["You are carrying a lot.", "Caring for someone and forgetting yourself is hard."]),
        [CompanionEvent.HealthWorry] = (["Ждать результатов бывает тяжелее всего.", "Неизвестность пугает сильнее фактов."], ["Waiting for results can be the hardest part.", "Not knowing is scarier than the facts."]),
        [CompanionEvent.Failure] = (["Обидно, когда вложено много сил.", "Неудача ранит, особенно когда вы к себе строги."], ["It stings when you put a lot in.", "A failure hurts, especially when you are hard on yourself."]),
        [CompanionEvent.Betrayal] = (["Когда обманывает близкий, трудно снова доверять.", "Такое доверие не вернуть за один день."], ["When someone close lies, trust is hard to give again.", "That kind of trust does not come back in a day."])
    };

    /// <summary>A short acknowledgement that belongs to what happened, so that "That is not easy" is not said to everyone about everything; null when the event has none.</summary>
    public static string? EventAcknowledgement(CompanionEvent kind, bool english, Random random, CompanionPerson? person = null)
    {
        if (kind == CompanionEvent.Conflict && IsAtWork(person))
        {
            string[] work = english
                ? ["Words like that at work stay with you long after the day ends.", "It is hard when so much depends on that person."]
                : ["Такие слова на работе не отпускают и после конца дня.", "Тяжело, когда от этого человека многое зависит."];
            return work[random.Next(work.Length)];
        }

        if (!Acknowledgements.TryGetValue(kind, out (string[] Ru, string[] En) lines))
        {
            return null;
        }

        string[] bank = english ? lines.En : lines.Ru;
        return bank[random.Next(bank.Length)];
    }

    private static readonly string[] RepeatedCues = ["не первый", "повторя", "часто", "постоянно", "опять", "снова", "каждый раз", "всегда", "not the first", "again", "keeps", "always", "often"];
    private static readonly string[] FirstTimeCues = ["впервые", "первый раз", "первый случай", "раньше не", "first time", "never before"];
    private static readonly string[] LongCues = ["давно", "месяц", "недел", "годами", "долго", "long time", "months", "weeks", "years"];
    private static readonly string[] RecentCues = ["недавно", "вчера", "на днях", "с недавних", "recently", "yesterday", "lately"];

    /// <summary>
    /// A short answer to a "this or that" question ("it is not the first time", "for a long time") read as the yes or the no of that question,
    /// so the follow-up fits what was said. Only for the two questions whose answer is a plain "again / first time" or "long / recent".
    /// </summary>
    public static bool? ReadChoice(string? questionId, string text)
    {
        string t = text.ToLowerInvariant();
        switch (questionId)
        {
            case "e:Conflict":
                return RepeatedCues.Any(t.Contains) ? true : FirstTimeCues.Any(t.Contains) ? false : null;
            case "e:Sleeplessness":
                return RecentCues.Any(t.Contains) ? false : LongCues.Any(t.Contains) ? true : null;
            default:
                return null;
        }
    }

    // ----- the person's own words -----

    private static readonly System.Text.RegularExpressions.Regex SaidPattern = new(
        @"(?:сказал[аи]?|назвал[аи]?|обозвал[аи]?|крикнул[аи]?|написал[аи]?|заявил[аи]?|said|called me|told me|texted)\s+(?:что\s+|мне\s+|меня\s+|that\s+)*([^,.!?;]{3,40})",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    /// <summary>What someone said to the person ("she called me ungrateful"), to give it back in the person's own words; null if no such phrase.</summary>
    public static string? SaidPhrase(string text)
    {
        System.Text.RegularExpressions.Match m = SaidPattern.Match(text);
        if (!m.Success)
        {
            return null;
        }

        string phrase = m.Groups[1].Value.Trim();
        string[] words = phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length is >= 1 and <= 6 ? phrase : string.Join(' ', words.Take(6));
    }

    public static string SaidLine(string phrase, bool english, Random random) => random.Next(2) == 0
        ? (english ? $"Words like “{phrase}” can stay in the head for a long time." : $"Слова «{phrase}» могут долго звучать в голове.")
        : (english ? $"“{phrase}” — it is understandable that this stung." : $"«{phrase}» — понятно, что это задело.");

    private static readonly string[] HoldRu = ["Я здесь, продолжайте, если хочется.", "Можно не торопиться. Я слушаю.", "Спасибо, что так подробно рассказали. Я с вами."];
    private static readonly string[] HoldEn = ["I am here, go on if you like.", "There is no hurry. I am listening.", "Thank you for telling it in such detail. I am with you."];

    /// <summary>After a long, heavy message a question can feel like being sent to a form: just stay with the person.</summary>
    public static string HoldSpace(bool english, Random random)
    {
        string[] bank = english ? HoldEn : HoldRu;
        return bank[random.Next(bank.Length)];
    }

    // ----- compared with the last chat -----

    /// <summary>"Last time it was 8, now it is 5": the first rating of a chat set against the last rating of the previous one.</summary>
    public static string CompareWithLastTime(int lastTime, int now, bool english)
    {
        if (now < lastTime)
        {
            return english ? $"Last time you rated the tension {lastTime}, now it is {now}. That is a real change." : $"В прошлый раз вы оценивали напряжение в {lastTime}, сейчас {now}. Это заметное изменение.";
        }

        if (now > lastTime)
        {
            return english ? $"Last time it was {lastTime}, now {now}. It is harder today, and it is good that you came back." : $"В прошлый раз было {lastTime}, сейчас {now}. Сегодня тяжелее, и хорошо, что вы вернулись.";
        }

        return english ? $"It is {now} again, as last time. The feeling has stayed with you." : $"Снова {now}, как и в прошлый раз. Это состояние осталось с вами.";
    }
}
