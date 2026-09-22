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
    ComplainsAboutCompanion,
    /// <summary>Insults, slurs or a crude command aimed at the companion ("ты дебил", "иди нахуй", "ты пидор").
    /// One calm boundary, not a lecture and not a matching tone.</summary>
    ProvokesOrInsultsCompanion,
    /// <summary>"What is a panic attack?", "explain CBT".</summary>
    AsksToExplain,
    /// <summary>"Why do I feel like this?".</summary>
    AsksWhy,
    /// <summary>"Is this normal?", "is something wrong with me?".</summary>
    AsksIfNormal,
    /// <summary>"ok", "I see", "понятно": a backchannel, not an answer.</summary>
    Acknowledge,
    /// <summary>Laughter, emoji or dots only: "ахах", "))", "😢", "...".</summary>
    Reaction,
    /// <summary>"I feel better", "it worked", "всё хорошо": good news to share, not a problem to solve.</summary>
    SharesGoodNews,
    AsksHowAreYou,
    AsksName,
    /// <summary>"My name is Anna".</summary>
    IntroducesSelf,
    AsksCapabilities,
    /// <summary>"What?", "repeat", "I didn't get it".</summary>
    AsksToRepeat,
    /// <summary>"Skip this", "ask something else".</summary>
    SkipsQuestion
}

/// <summary>
/// Rule-based recognition of conversational moves ("hi", "thanks", "I don't know", "who are you?", "what should I do?").
/// Short social phrases are only recognised when the message carries no feeling, so "thanks, but I'm still anxious" stays a statement.
/// </summary>
public static class UtteranceClassifier
{
    private const int ShortGreetingLength = 3;

    private static readonly string[] GreetingStarts = ["привет", "здравствуй", "добрый", "доброе", "приветствую", "хай", "hi", "hello", "hey", "good morning", "good evening", "good afternoon"];
    private static readonly string[] ThanksTerms = ["спасибо", "благодарю", "благодарствую", "thanks", "thank you", "thx"];
    private static readonly string[] GoodbyeTerms = ["до свидания", "до встречи", "всего доброго", "спокойной ночи", "до завтра", "bye", "goodbye", "good night", "see you", "talk later"];
    private static readonly string[] ByeWordTerms = ["пока "];
    private static readonly string[] DontKnowTerms = ["не знаю", "хз", "сложно сказать", "затрудняюсь", "не уверен", "не уверена", "трудно сказать", "dont know", "do not know", "no idea", "not sure", "hard to say"];
    private static readonly string[] RefuseTerms =
    [
        "не хочу об этом", "не хочу говорить", "не хочу рассказывать", "не буду об этом", "не хочется об этом", "не хочу про это", "не хочу это обсуждать",
        "dont want to talk", "do not want to talk", "rather not", "dont want to discuss", "not going to talk"
    ];

    private static readonly string[] AskBotTerms =
    [
        "ты кто", "ты вообще кто", "а ты кто", "вы вообще кто", "кто ты", "ты бот", "ты робот", "ты человек", "ты настоящ", "ты живой", "ты живая", "ты нейросеть", "ты ии", "вы бот", "вы робот", "вы человек", "вы кто", "кто вы",
        "ты гей", "ты гетеро", "ты натурал", "ты трансгендер", "ты парень или девушка", "у тебя есть пол", "какой у тебя пол", "ты мужчина или женщина", "ты мальчик или девочка", "сколько тебе лет", "у тебя есть тело",
        "are you a bot", "are you human", "are you real", "are you a person", "are you an ai", "who are you", "what are you",
        "are you gay", "are you straight", "are you trans", "what gender are you", "do you have a gender", "how old are you", "do you have a body"
    ];

    private static readonly string[] ComplaintTerms =
    [
        "ты не понимаешь", "ты меня не понимаешь", "ты ничего не понимаешь", "вы не понимаете", "вы меня не понимаете", "бесполезно", "ерунда какая", "какая ерунда", "тупой бот", "глупый бот",
        "не помогает", "ты тупой", "ты тупая", "толку нет", "you dont understand", "you do not understand", "useless", "this is stupid", "doesnt help", "does not help", "this is pointless"
    ];

    // Aimed at the companion specifically, not at a third party or at themselves: "ты дебил" counts, "какой я дебил" does not.
    private static readonly string[] InsultPhraseTerms =
    [
        "ты дебил", "ты дура", "ты дурак", "ты идиот", "ты идиотка", "ты придурок", "ты придурочная", "ты урод", "ты уродка", "ты сволочь", "ты мудак",
        "ты гандон", "ты тварь", "ты скотина", "ты ублюдок", "ты долбоеб", "ты конченый", "ты конченная", "ты чмо", "вы дебилы", "вы идиоты", "вы тупые",
        "you are stupid", "youre stupid", "you are an idiot", "youre an idiot", "you are useless", "youre useless", "you are dumb", "youre dumb", "you are a moron", "youre a moron", "you suck"
    ];

    // Short, crude commands: essentially never self-referential, so no "ты"/"вы" prefix is needed to stay safe.
    private static readonly string[] InsultCommandTerms =
    [
        "пошел нахуй", "пошла нахуй", "пошел на хуй", "пошла на хуй", "пошел ты нахуй", "пошла ты нахуй", "пошел ты на хуй", "иди нахуй", "иди на хуй", "иди отсюда нахуй", "отъебись", "отвали от меня", "заткнись", "иди в жопу", "иди к черту",
        "fuck you", "screw you", "go to hell", "shut the fuck up", "piss off"
    ];

    // Slur forms used as an insult or a provocation, in any phrasing; kept short so they cannot fire inside an unrelated long story.
    private static readonly string[] IdentitySlurTerms = ["педр", "пидор", "пидр", "пидорас", "пидрил", "гомосек", "fag", "faggot"];

    private static readonly string[] AdviceTerms =
    [
        "что делать", "что мне делать", "как быть", "посоветуй", "посоветуйте", "подскажи что", "подскажите что", "дай совет", "дайте совет", "что посоветуешь", "что посоветуете",
        "что мне теперь делать", "как мне быть", "what should i do", "what do i do", "any advice", "give me advice", "help me", "what can i do"
    ];

    private static readonly string[] ExplainTerms =
    [
        "что такое", "что значит", "что это такое", "что за", "объясни ", "объясните ", "расскажи про", "расскажи о", "расскажите про", "расскажите о", "как работает", "зачем нужн", "чем отличается",
        "what is", "what are", "what does", "explain", "tell me about", "how does", "how do"
    ];

    private static readonly string[] WhyTerms =
    [
        "почему я", "почему меня", "почему у меня", "почему мне", "откуда это", "откуда у меня", "отчего у меня", "отчего я",
        "why do i", "why am i", "why does this", "why is it that i", "why do i feel"
    ];

    private static readonly string[] NormalTerms =
    [
        "это нормально", "нормально ли", "разве это нормально", "со мной что то не так", "со мной что нибудь не так", "я ненормальн", "это ненормально", "я нормальн", "я схожу с ума",
        "is this normal", "is it normal", "something wrong with me", "am i crazy", "am i normal", "is that normal"
    ];

    private static readonly string[] IntroTerms = ["меня зовут ", "зови меня ", "называй меня ", "можешь звать меня ", "my name is ", "call me ", "im called "];
    private static readonly string[] AskNameTerms = ["как тебя зовут", "как вас зовут", "твое имя", "ваше имя", "как тебя называть", "what is your name", "whats your name", "what should i call you", "your name"];
    private static readonly string[] HowAreYouTerms = ["как дела", "как ты", "как вы", "как поживаешь", "как поживаете", "как жизнь", "как настроение", "how are you", "how are things", "how do you do", "hows it going"];
    private static readonly string[] CapabilityTerms =
    [
        "что ты умеешь", "что вы умеете", "что ты можешь", "что вы можете", "чем ты можешь помочь", "чем можешь помочь", "чем вы можете помочь", "как ты работаешь", "как это работает", "для чего ты", "зачем ты нужен",
        "what can you do", "how can you help", "how do you work", "what do you do", "what are you for"
    ];

    private static readonly string[] RepeatTerms =
    [
        "повтори", "повторите", "не понял", "не поняла", "не поняли", "в смысле", "что ты имеешь в виду", "что вы имеете в виду", "не расслышал", "что что", "переформулируй", "спроси иначе",
        "what do you mean", "say that again", "repeat that", "i didnt get that", "i dont get it", "rephrase", "come again"
    ];

    private static readonly string[] RepeatWholeMessage = ["что", "чего", "а что", "а", "ась", "what", "huh", "sorry"];

    private static readonly string[] SkipTerms =
    [
        "пропустим", "пропусти", "пропустить", "другой вопрос", "следующий вопрос", "задай другой", "спроси другое", "спроси что нибудь другое", "не хочу отвечать", "не буду отвечать", "давай дальше", "пропустим этот",
        "skip", "next question", "another question", "ask something else", "dont want to answer"
    ];

    private static readonly string[] AcknowledgeTerms = ["ок", "окей", "ясно", "ясненько", "понятно", "ладно", "ну ладно", "понял", "поняла", "принято", "ну ок", "okay", "ok", "i see", "got it", "alright", "understood", "hm", "хм", "мм", "ммм", "гм"];

    private static readonly string[] GoodShortTerms = ["хорошо", "нормально", "отлично", "супер", "прекрасно", "здорово", "good", "great", "fine", "excellent", "awesome"];

    private static readonly string[] GoodNewsTerms =
    [
        "получилось", "справилась", "справился", "стало легче", "полегчало", "мне лучше", "мне хорошо", "мне стало лучше", "все хорошо", "все отлично", "отпустило", "я рад", "я рада", "счастлив", "счастлива", "хороший день", "прекрасный день",
        "good news", "i feel better", "im better", "im happy", "im fine", "feeling good", "it worked", "went well", "i did it", "im proud", "feeling better"
    ];

    private static readonly string[] LaughStarts = ["ахах", "хаха", "хехе", "ахаха", "хихи", "ржу", "haha", "hehe", "lol", "lmao", "лол", "кек"];

    private static readonly string[] SadEmoji = ["😢", "😭", "☹", "😞", "😔", "💔", "🥺", "😟", "😥", "😰"];

    private static readonly string[] YesTerms = ["да", "ага", "угу", "конечно", "наверное", "пожалуй", "возможно", "верно", "точно", "yes", "yeah", "yep", "sure", "maybe", "probably", "right", "exactly"];
    private static readonly string[] NoTerms = ["нет", "неа", "вряд ли", "no", "nope", "not really"];

    /// <param name="text">Raw message.</param>
    /// <param name="hasFeeling">The message already carries a recognised feeling; social phrases are then ignored.</param>
    public static Utterance Classify(string text, bool hasFeeling)
    {
        string normalized = Normalize(text);
        if (normalized.Length == 0)
        {
            return IsReactionOnly(text) ? Utterance.Reaction : Utterance.Statement;
        }

        int words = CountWords(normalized);
        string padded = normalized + " ";

        // Talking about the companion itself is meaningful even next to a feeling.
        if (words <= 6 && ContainsAny(padded, AskBotTerms))
        {
            return Utterance.AsksAboutCompanion;
        }

        if (words <= 8 && (ContainsAny(padded, InsultPhraseTerms) || ContainsAny(padded, InsultCommandTerms) || (words <= 6 && ContainsAny(padded, IdentitySlurTerms))))
        {
            return Utterance.ProvokesOrInsultsCompanion;
        }

        if (words <= 6 && ContainsAny(padded, IntroTerms) && TryExtractName(text, out _))
        {
            return Utterance.IntroducesSelf;
        }

        if (words <= 6 && ContainsAny(padded, AskNameTerms))
        {
            return Utterance.AsksName;
        }

        if (words <= 5 && StartsShortQuestion(normalized, HowAreYouTerms))
        {
            return Utterance.AsksHowAreYou;
        }

        if (words <= 8 && ContainsAny(padded, CapabilityTerms))
        {
            return Utterance.AsksCapabilities;
        }

        if (words <= 6 && ContainsAny(padded, SkipTerms))
        {
            return Utterance.SkipsQuestion;
        }

        if (words <= 10 && ContainsAny(padded, ComplaintTerms))
        {
            return Utterance.ComplainsAboutCompanion;
        }

        if (ContainsAny(padded, RefuseTerms))
        {
            return Utterance.RefusesToTalk;
        }

        if (words <= 12 && ContainsAny(padded, ExplainTerms))
        {
            return Utterance.AsksToExplain;
        }

        if (words <= 12 && ContainsAny(padded, WhyTerms))
        {
            return Utterance.AsksWhy;
        }

        if (words <= 12 && ContainsAny(padded, NormalTerms))
        {
            return Utterance.AsksIfNormal;
        }

        if (words <= 9 && ContainsAny(padded, AdviceTerms))
        {
            return Utterance.AsksForAdvice;
        }

        if (hasFeeling)
        {
            return Utterance.Statement;
        }

        if (words <= 6 && ContainsAny(padded, ThanksTerms))
        {
            return Utterance.Thanks;
        }

        if ((words <= 6 && ContainsAny(padded, GoodbyeTerms)) || (words <= 2 && ContainsAny(padded, ByeWordTerms)))
        {
            return Utterance.Goodbye;
        }

        if (words <= 4 && IsGreeting(normalized))
        {
            return Utterance.Greeting;
        }

        if (words <= 6 && ContainsAny(padded, DontKnowTerms))
        {
            return Utterance.DontKnow;
        }

        if ((words <= 6 && ContainsAny(padded, RepeatTerms)) || RepeatWholeMessage.Contains(normalized))
        {
            return Utterance.AsksToRepeat;
        }

        if (words <= 2 && StartsWithAny(normalized, LaughStarts))
        {
            return Utterance.Reaction;
        }

        if (IsShareOfGoodNews(normalized, padded, words))
        {
            return Utterance.SharesGoodNews;
        }

        if (AcknowledgeTerms.Contains(normalized) || (words <= 2 && StartsWithAny(normalized, AcknowledgeTerms)))
        {
            return Utterance.Acknowledge;
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

    /// <summary>The message starts with a greeting. Short greetings ("hi", "hey", "хай") must be the whole first word, so "high" or "хайп" do not count.</summary>
    private static bool IsGreeting(string normalized)
    {
        int end = normalized.IndexOf(' ');
        ReadOnlySpan<char> first = end < 0 ? normalized : normalized.AsSpan(0, end);
        foreach (string greeting in GreetingStarts)
        {
            bool matches = greeting.Length > ShortGreetingLength
                ? normalized.StartsWith(greeting, StringComparison.Ordinal)
                : first.SequenceEqual(greeting);
            if (matches)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Text with no letters at all: dots, brackets, emoji. Laughter and sad faces are reactions, not empty messages.</summary>
    public static bool IsReactionOnly(string text) =>
        text.Trim().Length > 0 && !text.Any(char.IsLetterOrDigit);

    /// <summary>The message is only a sad face or tears: worth a gentle question rather than a joke.</summary>
    public static bool IsSadReaction(string text) => SadEmoji.Any(text.Contains);

    /// <summary>Picks the name out of "меня зовут Аня" / "my name is Anna". Accepts one or two plain words of letters.</summary>
    public static bool TryExtractName(string text, out string name)
    {
        name = string.Empty;
        string lower = text.ToLowerInvariant().Replace('ё', 'е');
        foreach (string term in IntroTerms)
        {
            int at = lower.IndexOf(term, StringComparison.Ordinal);
            if (at < 0 || (at > 0 && char.IsLetter(lower[at - 1])))
            {
                continue;
            }

            string[] words = text[(at + term.Length)..]
                .Split([' ', ',', '.', '!', '?', ';', ':'], StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0 || words[0].Length is < 2 or > 20 || !words[0].All(c => char.IsLetter(c) || c == '-'))
            {
                return false;
            }

            name = char.ToUpperInvariant(words[0][0]) + words[0][1..].ToLowerInvariant();
            return true;
        }

        return false;
    }

    /// <summary>A short reply that may itself be a name, right after the companion asked for one.</summary>
    public static bool LooksLikeBareName(string text, out string name)
    {
        name = string.Empty;
        string[] words = text.Split([' ', ',', '.', '!'], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length is 0 or > 2 || !words.All(w => w.Length >= 2 && w.All(c => char.IsLetter(c) || c == '-')))
        {
            return false;
        }

        name = char.ToUpperInvariant(words[0][0]) + words[0][1..].ToLowerInvariant();
        return true;
    }

    /// <summary>"How are you?" is only that when little follows it: "как дела у тебя", but not "как ты думаешь, что делать".</summary>
    private static bool StartsShortQuestion(string normalized, string[] terms)
    {
        string padded = normalized + " ";
        foreach (string term in terms)
        {
            int at = padded.IndexOf(term, StringComparison.Ordinal);
            if (at < 0 || (at > 0 && padded[at - 1] != ' '))
            {
                continue;
            }

            string tail = padded[(at + term.Length)..].Trim();
            if (tail.Length == 0 || CountWords(tail) <= 1)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsShareOfGoodNews(string normalized, string padded, int words)
    {
        if (words > 14 || new[] { "не", "но", "однако", "хотя", "только", "лишь", "когда", "если", "пока", "иногда", "but", "still", "not", "cant", "dont", "only", "when", "if", "sometimes" }.Any(w => (" " + padded).Contains(" " + w + " ", StringComparison.Ordinal)))
        {
            return false;
        }

        return (words <= 3 && GoodShortTerms.Contains(normalized))
            || (words <= 5 && GoodShortTerms.Any(t => normalized.StartsWith(t + " ", StringComparison.Ordinal)))
            || ContainsAny(padded, GoodNewsTerms);
    }

    /// <summary>
    /// True when a term occurs starting at a word boundary ("что такое" in "а что такое кпт"). <paramref name="padded"/> is the
    /// normalized text plus one trailing space, so a term with a trailing space matches the last word too.
    /// </summary>
    private static bool ContainsAny(string padded, string[] terms)
    {
        foreach (string term in terms)
        {
            int from = 0;
            while ((from = padded.IndexOf(term, from, StringComparison.Ordinal)) >= 0)
            {
                if (from == 0 || padded[from - 1] == ' ')
                {
                    return true;
                }

                from++;
            }
        }

        return false;
    }

    private static bool StartsWithAny(string normalized, string[] terms)
    {
        foreach (string term in terms)
        {
            if (normalized == term || normalized.StartsWith(term + " ", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static int CountWords(string normalized)
    {
        int words = 1;
        foreach (char c in normalized)
        {
            if (c == ' ')
            {
                words++;
            }
        }

        return words;
    }

    /// <summary>Lower-cases, folds "ё" into "е", drops apostrophes and turns every run of non-letters into one space.</summary>
    private static string Normalize(string text)
    {
        Span<char> buffer = text.Length <= 512 ? stackalloc char[text.Length] : new char[text.Length];
        int length = 0;
        bool pendingSpace = false;

        foreach (char raw in text)
        {
            char c = char.ToLowerInvariant(raw);
            if (c is '\'' or '’')
            {
                continue;
            }

            if (!char.IsLetterOrDigit(c))
            {
                pendingSpace = true;
                continue;
            }

            if (pendingSpace && length > 0)
            {
                buffer[length++] = ' ';
            }

            pendingSpace = false;
            buffer[length++] = c == 'ё' ? 'е' : c;
        }

        return new string(buffer[..length]);
    }
}
