namespace PsychologyApp.Application.Chat;

/// <summary>
/// Wording of the trauma-informed part of the companion: how it answers when a hard event is mentioned, the work on beliefs that got stuck
/// after it (the themes and questions of cognitive processing therapy, without recounting the event), the trigger map, and stabilising
/// first when it is too hard. Psychologist review (trauma specialist) before release; see docs/trauma-module.md.
/// </summary>
public static class CompanionTraumaContent
{
    public const string Impact = "impact";
    public const string Triggers = "triggers";
    public const string Topic = "trauma";

    /// <summary>From this distress on, the work waits: first back to the present.</summary>
    public const int TooHard = 7;

    /// <summary>The themes after trauma in cognitive processing therapy, with blame first: it is the most common stuck point.</summary>
    public static readonly IReadOnlyList<string> Themes = ["guilt", "safety", "trust", "control", "esteem", "intimacy"];

    // Impact steps: distress, theme, stuck phrase, belief, first question, second question, fairer wording, belief again.
    public const int ImpactDistress = 0;
    public const int ImpactTheme = 1;
    public const int ImpactPhrase = 2;
    public const int ImpactBeliefBefore = 3;
    public const int ImpactBalanced = 6;
    public const int ImpactBeliefAfter = 7;

    public const int TriggerSteps = 3;

    public static string ThemeName(string theme, bool english) => (theme, english) switch
    {
        ("guilt", false) => "Вина",
        ("safety", false) => "Безопасность",
        ("trust", false) => "Доверие",
        ("control", false) => "Контроль",
        ("esteem", false) => "Отношение к себе",
        ("intimacy", false) => "Близость",
        ("guilt", true) => "Blame",
        ("safety", true) => "Safety",
        ("trust", true) => "Trust",
        ("control", true) => "Control",
        ("esteem", true) => "How I see myself",
        ("intimacy", true) => "Closeness",
        _ => theme
    };

    /// <summary>A theme named in typed words, or null.</summary>
    public static string? ParseTheme(string text)
    {
        string t = text.ToLowerInvariant().Replace('ё', 'е');
        return t switch
        {
            _ when t.Contains("вин") || t.Contains("blame") || t.Contains("guilt") || t.Contains("fault") => "guilt",
            _ when t.Contains("безопас") || t.Contains("опас") || t.Contains("safe") || t.Contains("danger") => "safety",
            _ when t.Contains("довер") || t.Contains("trust") => "trust",
            _ when t.Contains("контрол") || t.Contains("control") => "control",
            _ when t.Contains("себ") || t.Contains("сломан") || t.Contains("self") || t.Contains("myself") || t.Contains("broken") => "esteem",
            _ when t.Contains("близ") || t.Contains("отношен") || t.Contains("close") || t.Contains("intima") => "intimacy",
            _ => null
        };
    }

    // ----- when a hard event is mentioned -----

    public static string Acknowledge(bool english) => english
        ? "Thank you for telling me. What happened to you is hard, and your reactions to it make sense. You don't have to tell me the details if you don't want to: what matters more is how it echoes now. I can help in a few ways."
        : "Спасибо, что рассказали. То, что с вами случилось, тяжело, и ваши реакции на это понятны. Подробности рассказывать не нужно, если не хочется: важнее, как это отзывается сейчас. Я могу помочь по-разному.";

    public static string TopicGreeting(bool english) => english
        ? "Here we can look at how a hard event echoes in you now, without retelling what happened. You set the pace and can stop at any moment. What is closer to you?"
        : "Здесь можно разобраться, как тяжёлое событие отзывается в вас сейчас, не пересказывая, что случилось. Темп задаёте вы, и остановиться можно в любой момент. Что вам ближе?";

    public static IReadOnlyList<ChatQuickReply> Choices(bool english) =>
    [
        new(ChatQuickReplyKinds.Act, english ? "What it changed in me" : "Что это во мне изменило", "protocol:" + Impact),
        new(ChatQuickReplyKinds.Act, english ? "What sets it off" : "Что это запускает", "protocol:" + Triggers),
        new(ChatQuickReplyKinds.Practice, english ? "If it takes over: back to the present" : "Если накрывает: вернуться в настоящее", nameof(TechniqueId.Flashback)),
        new(ChatQuickReplyKinds.Act, english ? "Just talk" : "Просто выговориться", "vent")
    ];

    // ----- stabilising first -----

    public static string TooHardNow(bool english) => english
        ? "It's very hard right now, so first let's come back to the present. This work can wait: it isn't going anywhere, and it's better done when there is a little more ground under your feet."
        : "Сейчас очень тяжело, значит, сначала вернёмся в настоящее. Разбор подождёт: он никуда не денется, и делать его лучше, когда под ногами чуть больше опоры.";

    public static IReadOnlyList<ChatQuickReply> StabiliseChips(bool english) =>
    [
        new(ChatQuickReplyKinds.Practice, english ? "Back to the present" : "Вернуться в настоящее", nameof(TechniqueId.Flashback)),
        new(ChatQuickReplyKinds.Practice, english ? "5-4-3-2-1 grounding" : "Заземление 5-4-3-2-1", nameof(TechniqueId.Grounding))
    ];

    // ----- what the event changed (stuck points) -----

    public static string DistressQuestion(bool english) => english
        ? "Before we start: how hard is it for you right now, from 0 to 10?"
        : "Прежде чем начать: насколько вам сейчас тяжело, от 0 до 10?";

    public static string ThemeQuestion(bool english) => english
        ? "After hard events people often start thinking differently about themselves and the world. We won't go into what happened, only into these conclusions. Which theme feels heaviest?"
        : "После тяжёлых событий люди часто начинают иначе думать о себе и о мире. В само событие мы не пойдём, только в эти выводы. Какая тема отзывается сильнее всего?";

    public static IReadOnlyList<ChatQuickReply> ThemeChips(bool english) =>
        [.. Themes.Select(theme => new ChatQuickReply(ChatQuickReplyKinds.Act, ThemeName(theme, english), "say:" + theme))];

    public static string PhraseQuestion(string theme, bool english) => english
        ? $"{ThemeName(theme, true)}. What do you tell yourself about it? Write the phrase the way it sounds in your head; the examples below are only a hint."
        : $"{ThemeName(theme, false)}. Что вы себе об этом говорите? Запишите фразу так, как она звучит в голове; примеры ниже — только подсказка.";

    public static IReadOnlyList<string> PhraseExamples(string theme, bool english) => (theme, english) switch
    {
        ("guilt", false) => ["Это моя вина", "Я должен(на) был(а) это предотвратить", "Надо было сопротивляться сильнее"],
        ("safety", false) => ["Мир опасен", "Это случится снова", "Нигде не безопасно"],
        ("trust", false) => ["Никому нельзя доверять", "Все рано или поздно предадут", "Я не могу верить своим решениям"],
        ("control", false) => ["Я ничего не контролирую", "Если отпущу контроль, будет беда"],
        ("esteem", false) => ["Со мной что-то не так", "Я сломан(а)", "Я не заслуживаю хорошего"],
        ("intimacy", false) => ["Мне нельзя ни к кому приближаться", "Если подпущу, будет больно"],
        ("guilt", true) => ["It was my fault", "I should have prevented it", "I should have fought harder"],
        ("safety", true) => ["The world is dangerous", "It will happen again", "Nowhere is safe"],
        ("trust", true) => ["No one can be trusted", "Everyone betrays sooner or later", "I can't trust my own judgement"],
        ("control", true) => ["I have no control over anything", "If I let go of control, disaster will follow"],
        ("esteem", true) => ["Something is wrong with me", "I'm broken", "I don't deserve good things"],
        ("intimacy", true) => ["I can't let anyone close", "If I let someone in, it will hurt"],
        _ => []
    };

    public static IReadOnlyList<ChatQuickReply> PhraseChips(string theme, bool english) =>
        [.. PhraseExamples(theme, english).Select(phrase => new ChatQuickReply(ChatQuickReplyKinds.Act, phrase, "say:" + phrase))];

    public static string BeliefQuestion(bool english) => english
        ? "How much do you believe this phrase right now, from 0 to 10?"
        : "Насколько вы верите этой фразе прямо сейчас, от 0 до 10?";

    /// <summary>Two Socratic questions per theme, as in cognitive processing therapy: they question the conclusion, not the memory.</summary>
    public static string ThemeQuestion(string theme, int which, bool english) => (theme, which, english) switch
    {
        ("guilt", 0, false) => "Что вы знали и что могли сделать тогда — в ту минуту, а не сейчас, когда известно, чем всё кончилось?",
        ("guilt", _, false) => "Кто на самом деле отвечает за то, что произошло? Не смочь предотвратить и быть виноватым — это одно и то же?",
        ("safety", 0, false) => "Если честно оценить: насколько вероятно, что это повторится сегодня, здесь? Что в вашей жизни сейчас иначе, чем тогда?",
        ("safety", _, false) => "Что помогает вам быть в безопасности сейчас — люди, места, то, что вы умеете?",
        ("trust", 0, false) => "Есть ли хоть один человек, которому вы можете доверять хоть в чём-то? Чем он отличается от того, кто причинил боль?",
        ("trust", _, false) => "Можно ли доверять не «полностью или никому», а понемногу, проверяя на деле?",
        ("control", 0, false) => "Что тогда было в вашей власти, а что нет?",
        ("control", _, false) => "Что в вашей власти сейчас, хотя бы немного?",
        ("esteem", 0, false) => "Если бы это случилось с близким вам человеком, считали бы вы его сломанным или недостойным?",
        ("esteem", _, false) => "Почему к себе вы применяете другое, более жёсткое правило? Что вы на самом деле знаете о себе, кроме этого события?",
        ("intimacy", 0, false) => "Где граница между тем, чтобы беречь себя, и тем, чтобы остаться совсем одному?",
        ("intimacy", _, false) => "Какой маленький шаг навстречу можно позволить с тем, кому вы хоть немного доверяете?",
        ("guilt", 0, true) => "What did you know, and what could you do, back then — in that minute, not now that you know how it ended?",
        ("guilt", _, true) => "Who is really responsible for what happened? Is not being able to prevent something the same as being to blame?",
        ("safety", 0, true) => "Honestly: how likely is it to happen again today, here? What in your life now is different from back then?",
        ("safety", _, true) => "What helps you be safe now — people, places, things you know how to do?",
        ("trust", 0, true) => "Is there at least one person you can trust with at least something? How are they different from the one who hurt you?",
        ("trust", _, true) => "Could trust be not \"all or no one\" but little by little, checked in practice?",
        ("control", 0, true) => "What was in your power back then, and what wasn't?",
        ("control", _, true) => "What is in your power now, even a little?",
        ("esteem", 0, true) => "If this had happened to someone close to you, would you think of them as broken or unworthy?",
        ("esteem", _, true) => "Why do you apply a different, harsher rule to yourself? What do you actually know about yourself besides this event?",
        ("intimacy", 0, true) => "Where is the line between protecting yourself and ending up completely alone?",
        ("intimacy", _, true) => "What small step towards someone you trust even a little could you allow?",
        _ => english ? "What goes against this phrase?" : "Что говорит против этой фразы?"
    };

    public static string BalancedQuestion(bool english) => english
        ? "Taking all this into account, how could it be said more accurately and fairly to yourself?"
        : "С учётом всего этого, как можно сказать точнее и честнее по отношению к себе?";

    public static string BeliefAgain(bool english) => english
        ? "How much do you believe the first phrase now, from 0 to 10?"
        : "Насколько вы теперь верите первой фразе, от 0 до 10?";

    public static string ImpactSummary(StuckPointNote note, bool english)
    {
        string theme = ThemeName(note.Theme, english);
        string change = english
            ? note.After < note.Before
                ? "The phrase has loosened its grip. Such conclusions formed under the weight of the event, and they can be revised."
                : "The phrase is still holding on. That's normal: stuck points from hard events often need several rounds, and this is exactly what a trauma therapist helps with."
            : note.After < note.Before
                ? "Фраза ослабла. Такие выводы сложились под тяжестью события, и их можно пересмотреть."
                : "Фраза пока держится. Это нормально: застрявшие после тяжёлых событий убеждения часто требуют нескольких подходов, и именно с этим помогает терапевт, который работает с травмой.";
        return english
            ? $"{theme}: “{note.Phrase}”. Belief was {note.Before}, now {note.After}.\nMore accurate: “{note.Balanced}”.\n{change} I've noted this for the summary for a specialist."
            : $"{theme}: «{note.Phrase}». Верили на {note.Before}, сейчас на {note.After}.\nТочнее: «{note.Balanced}».\n{change} Записал(а) это для сводки специалисту.";
    }

    // ----- the trigger map -----

    public static string TriggerQuestion(int step, bool english) => step switch
    {
        0 => english
            ? "Think of the last time it got harder. Where were you and what was around? No details of the event itself are needed."
            : "Вспомните последний раз, когда стало хуже. Где вы были и что было вокруг? Подробности самого события не нужны.",
        1 => english
            ? "What did your body feel, and what thoughts came?"
            : "Что почувствовало тело и какие мысли пришли?",
        _ => english
            ? "What helped even a little — or what could have helped?"
            : "Что помогло хоть немного — или что могло бы помочь?"
    };

    public static string TriggerSummary(TriggerNote note, bool english) => english
        ? $"Your map: when “{note.Situation}”, the reaction is “{note.Reaction}”, and what helps is “{note.Helped}”.\nKnowing what sets it off means you can prepare instead of being caught off guard. I've noted this for the summary for a specialist."
        : $"Ваша карта: когда «{note.Situation}», реакция — «{note.Reaction}», а помогает «{note.Helped}».\nЗная, что запускает, можно подготовиться, а не быть застигнутым врасплох. Записал(а) это для сводки специалисту.";

    // ----- the summary for a specialist -----

    public static string SummaryTitle(bool english) => english
        ? "After a hard event (from the chat, without details of the event)"
        : "После тяжёлого события (из чата, без подробностей события)";

    public static string SummaryStuckPoint(StuckPointNote note, bool english) => english
        ? $"- {ThemeName(note.Theme, true)}: “{note.Phrase}”, belief {note.Before} → {note.After}; revised: “{note.Balanced}”"
        : $"- {ThemeName(note.Theme, false)}: «{note.Phrase}», вера {note.Before} → {note.After}; пересмотрено: «{note.Balanced}»";

    public static string SummaryTrigger(TriggerNote note, bool english) => english
        ? $"- Trigger: “{note.Situation}”; reaction: “{note.Reaction}”; helps: “{note.Helped}”"
        : $"- Триггер: «{note.Situation}»; реакция: «{note.Reaction}»; помогает: «{note.Helped}»";
}
