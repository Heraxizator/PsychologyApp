namespace PsychologyApp.Application.Chat;

/// <summary>What the person is doing with their message, as opposed to what they feel. Not every message describes a feeling.</summary>
public enum Utterance
{
    /// <summary>Describes a situation or a feeling: the normal case.</summary>
    Statement,
    Greeting,
    Thanks,
    Goodbye,
    Yes,
    No,
    DontKnow,
    RefusesToTalk,
    AsksForAdvice,
    AsksAboutCompanion,
    ComplainsAboutCompanion
}

/// <summary>
/// Rule-based recognition of conversational moves ("hi", "thanks", "I don't know", "who are you?", "what should I do?").
/// Short social phrases are only recognised when the message carries no feeling, so "thanks, but I'm still anxious" stays a statement.
/// </summary>
public static class UtteranceClassifier
{
    private static readonly string[] GreetingStarts = ["привет", "здравствуй", "добрый", "доброе", "приветствую", "хай", "hi", "hello", "hey", "good morning", "good evening", "good afternoon"];
    private static readonly string[] ThanksTerms = ["спасибо", "благодарю", "благодарствую", "thanks", "thank you", "thx"];
    private static readonly string[] GoodbyeTerms = ["до свидания", "до встречи", "всего доброго", "спокойной ночи", "до завтра", "bye", "goodbye", "good night", "see you", "talk later"];
    private static readonly string[] DontKnowTerms = ["не знаю", "хз", "сложно сказать", "затрудняюсь", "не уверен", "не уверена", "трудно сказать", "dont know", "do not know", "no idea", "not sure", "hard to say"];
    private static readonly string[] RefuseTerms =
    [
        "не хочу об этом", "не хочу говорить", "не хочу рассказывать", "не буду об этом", "не хочется об этом", "не хочу про это", "не хочу это обсуждать",
        "dont want to talk", "do not want to talk", "rather not", "dont want to discuss", "not going to talk"
    ];

    private static readonly string[] AskBotTerms =
    [
        "ты кто", "ты вообще кто", "а ты кто", "вы вообще кто", "кто ты", "ты бот", "ты робот", "ты человек", "ты настоящ", "ты живой", "ты живая", "ты нейросеть", "ты ии", "вы бот", "вы робот", "вы человек", "вы кто", "кто вы",
        "are you a bot", "are you human", "are you real", "are you a person", "are you an ai", "who are you", "what are you"
    ];

    private static readonly string[] ComplaintTerms =
    [
        "ты не понимаешь", "ты меня не понимаешь", "ты ничего не понимаешь", "вы не понимаете", "вы меня не понимаете", "бесполезно", "ерунда какая", "какая ерунда", "тупой бот", "глупый бот",
        "не помогает", "ты тупой", "ты тупая", "толку нет", "you dont understand", "you do not understand", "useless", "this is stupid", "doesnt help", "does not help", "this is pointless"
    ];

    private static readonly string[] AdviceTerms =
    [
        "что делать", "что мне делать", "как быть", "посоветуй", "посоветуйте", "подскажи что", "подскажите что", "дай совет", "дайте совет", "что посоветуешь", "что посоветуете",
        "что мне теперь делать", "как мне быть", "what should i do", "what do i do", "any advice", "give me advice", "help me", "what can i do"
    ];

    private static readonly string[] YesTerms = ["да", "ага", "угу", "конечно", "наверное", "пожалуй", "возможно", "верно", "точно", "yes", "yeah", "yep", "sure", "maybe", "probably", "right", "exactly"];
    private static readonly string[] NoTerms = ["нет", "неа", "вряд ли", "no", "nope", "not really"];

    /// <param name="text">Raw message.</param>
    /// <param name="hasFeeling">The message already carries a recognised feeling; social phrases are then ignored.</param>
    public static Utterance Classify(string text, bool hasFeeling)
    {
        string normalized = Normalize(text);
        if (normalized.Length == 0)
        {
            return Utterance.Statement;
        }

        int words = normalized.Split(' ').Length;

        // Talking about the companion itself is meaningful even next to a feeling.
        if (ContainsAny(normalized, AskBotTerms) && words <= 6)
        {
            return Utterance.AsksAboutCompanion;
        }

        if (ContainsAny(normalized, ComplaintTerms) && words <= 10)
        {
            return Utterance.ComplainsAboutCompanion;
        }

        if (ContainsAny(normalized, RefuseTerms))
        {
            return Utterance.RefusesToTalk;
        }

        if (ContainsAny(normalized, AdviceTerms) && words <= 9)
        {
            return Utterance.AsksForAdvice;
        }

        if (hasFeeling)
        {
            return Utterance.Statement;
        }

        if (words <= 6 && ContainsAny(normalized, ThanksTerms))
        {
            return Utterance.Thanks;
        }

        if ((words <= 6 && ContainsAny(normalized, GoodbyeTerms)) || (words <= 2 && normalized.Split(' ').Contains("пока")))
        {
            return Utterance.Goodbye;
        }

        if (words <= 4 && GreetingStarts.Any(g => normalized.StartsWith(g, StringComparison.Ordinal)))
        {
            return Utterance.Greeting;
        }

        if (words <= 6 && ContainsAny(normalized, DontKnowTerms))
        {
            return Utterance.DontKnow;
        }

        if (words <= 3 && StartsWithAny(normalized, YesTerms))
        {
            return Utterance.Yes;
        }

        if (words <= 3 && StartsWithAny(normalized, NoTerms))
        {
            return Utterance.No;
        }

        return Utterance.Statement;
    }

    private static bool ContainsAny(string normalized, string[] terms)
    {
        string padded = " " + normalized + " ";
        return terms.Any(term => padded.Contains(" " + term, StringComparison.Ordinal));
    }

    private static bool StartsWithAny(string normalized, string[] terms) =>
        terms.Any(term => normalized == term || normalized.StartsWith(term + " ", StringComparison.Ordinal));

    private static string Normalize(string text)
    {
        char[] chars = text.ToLowerInvariant().Replace('ё', 'е').Replace("'", string.Empty).Replace("’", string.Empty)
            .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
            .ToArray();
        return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
