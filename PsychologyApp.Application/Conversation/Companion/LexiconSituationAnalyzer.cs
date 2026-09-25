namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// Offline, dependency-free analyzer: weighted stem lexicon (Russian and English) with simple negation handling.
/// Term syntax: <c>stem</c> matches any word that starts with it, <c>=word</c> matches exactly, and a term containing a space is a phrase.
/// </summary>
public sealed partial class LexiconSituationAnalyzer : ISituationAnalyzer
{
    private const double MinScore = 1.0;

    private static readonly HashSet<string> Negations = ["не", "нет", "ни", "no", "not", "never", "dont", "cant", "cannot"];

    // A negation is still recognised across one of these ("не сильно тревожусь"), but not across an arbitrary word —
    // normalization drops sentence punctuation, so an unbounded look-back would misread "Не знаю. Тревожусь" as negated.
    private static readonly HashSet<string> NegationModifiers = ["сильно", "очень", "так", "особо", "слишком", "совсем", "прямо", "really", "very", "so", "too", "that"];

    private sealed record Entry(int Weight, string[] Terms);

    private static readonly Dictionary<CompanionEmotion, Entry[]> Emotions = new()
    {
        [CompanionEmotion.Panic] =
        [
            new(2, ["паник", "паническ", "задыха", "не могу дышать", "трудно дышать", "не хватает воздуха", "сердце колотит", "сердце бьется", "сердце выскакива", "накрыло", "накрывает", "умру", "умираю", "приступ", "дереализ", "нереальн",
                    "panic", "cant breathe", "cannot breathe", "heart racing", "heart pounding", "hyperventilat", "attack"])
        ],
        [CompanionEmotion.Anxiety] =
        [
            new(2, ["тревог", "тревож", "боюсь", "страшно", "anxi", "afraid", "scared"]),
            new(1, ["волную", "волнов", "беспокой", "страх", "бояться", "боязн", "переживаю", "нервнича", "неуверен", "не по себе", "worr", "nervous", "fear", "uneasy", "on edge", "stress"])
        ],
        [CompanionEmotion.Overthinking] =
        [
            new(2, ["накручива", "прокручива", "зациклил", "не могу перестать думать", "мысли крутятся", "крутятся мысли", "навязчив", "лезут мысли", "мысли лезут", "постоянно думаю", "все думаю", "не выходит из головы",
                    "overthink", "ruminat", "cant stop thinking", "cannot stop thinking", "replaying", "spiral", "racing thoughts", "obsess"])
        ],
        [CompanionEmotion.Anger] =
        [
            new(2, ["злюсь", "злит", "злост", "=зол", "=зла", "=злой", "бесит", "бешу", "ярост", "ненавиж", "взбес", "выбесил", "angry", "anger", "furious", "rage", "pissed", "hate"]),
            new(1, ["раздража", "достал", "достаёт", "достает", "irritat", "annoy", "=mad", "mad at"])
        ],
        [CompanionEmotion.Resentment] =
        [
            new(2, ["обид", "обиж", "несправедлив", "предал", "унизил", "унижен", "resent", "unfair", "betray", "humiliat"]),
            new(1, ["игнорир", "не ценят", "не уважа", "оскорбил", "обесцен", "пренебрег", "ignored", "disrespect", "=hurt", "offended", "unappreciated"])
        ],
        [CompanionEmotion.Guilt] =
        [
            new(2, ["виноват", "чувство вины", "чувствую вину", "=вину", "винов", "стыд", "guilt", "ashamed", "shame", "my fault"]),
            new(1, ["жалею", "ошибся", "ошиблась", "косяк", "подвел", "подвела", "не оправдал", "regret", "let down", "screwed up"])
        ],
        [CompanionEmotion.Sadness] =
        [
            new(2, ["груст", "тоск", "печал", "безнадеж", "депресс", "ничего не радует", "хочется плакать", "sad", "hopeless", "depress", "grief"]),
            new(1, ["плач", "слез", "пустот", "=пусто", "тяжело на душе", "унын", "подавлен", "cry", "tears", "empty", "=down"])
        ],
        [CompanionEmotion.Exhaustion] =
        [
            new(2, ["устал", "выгор", "нет сил", "сил нет", "измотан", "вымотан", "истощ", "tired", "exhaust", "burnout", "burn out", "drained", "no energy"]),
            new(1, ["не хочу ничего", "апати", "перегруз", "не высыпа", "бессонниц", "не могу уснуть", "не сплю", "не спится", "cant sleep", "cannot sleep", "insomnia", "overwhelm"])
        ],
        [CompanionEmotion.Loneliness] =
        [
            new(2, ["одинок", "никому не нужен", "никому не нужна", "lonely", "isolated"]),
            new(1, ["я одна", "я один", "никого нет", "никто не понима", "не с кем", "alone", "nobody", "no one cares"])
        ],
        [CompanionEmotion.Procrastination] =
        [
            new(2, ["прокрастин", "откладыва", "не могу заставить", "не могу начать", "не могу собраться", "procrastinat", "putting off", "cant start", "cannot start"]),
            new(1, ["=лень", "лениться", "дедлайн", "сроки горят", "ничего не делаю", "не получается начать", "cant focus", "deadline", "lazy"])
        ]
    };

    private static readonly string[] BodyTerms =
    [
        "сердц", "давит", "=грудь", "груд", "живот", "желуд", "голов", "тошн", "комок", "зажим", "мышц", "плеч", "дрож", "трясет", "трясу", "потею", "горл", "сжима", "спазм",
        "chest", "stomach", "heart", "shaking", "trembl", "nausea", "headache", "tight", "lump", "shoulders", "throat", "sweat"
    ];

    private static readonly string[] IntensityTerms =
    [
        "очень", "сильн", "невыносим", "ужасн", "жутк", "крайне", "совсем", "постоянно", "все время", "не выдерживаю", "на пределе", "хуже некуда", "катастроф",
        "very", "extremely", "unbearable", "terrible", "awful", "constantly", "all the time", "at my limit", "worst"
    ];

    private static readonly Dictionary<CompanionTheme, string[]> Themes = new()
    {
        [CompanionTheme.Work] = ["работ", "начальник", "коллег", "проект", "зарплат", "офис", "увол", "собеседован", "=boss", "=work", "=job", "colleague", "office", "interview"],
        [CompanionTheme.Relationships] = ["отношен", "парень", "парня", "девушк", "=муж", "мужа", "жена", "жену", "партнер", "расстал", "бывш", "свидан", "boyfriend", "girlfriend", "husband", "=wife", "partner", "breakup", "relationship", "dating"],
        [CompanionTheme.Family] = ["мама", "мамы", "маму", "=папа", "родител", "ребен", "=дети", "семь", "=брат", "сестр", "mother", "father", "parents", "=child", "=kids", "family", "sister", "brother"],
        [CompanionTheme.Health] = ["здоров", "болезн", "врач", "диагноз", "болит", "=боль", "анализ", "health", "doctor", "illness", "diagnos", "=pain"],
        [CompanionTheme.Money] = ["=деньги", "денег", "=долг", "долги", "кредит", "ипотек", "=money", "=debt", "=loan", "=rent", "=bills"],
        [CompanionTheme.Study] = ["экзамен", "учеб", "сесси", "универ", "школ", "диплом", "зачет", "=exam", "=study", "university", "=school", "thesis", "=grade"]
    };

    public SituationAnalysis Analyze(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return SituationAnalysis.Empty;
        }

        string normalized = Normalize(text);
        string[] tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        int[] offsets = new int[tokens.Length];
        for (int i = 1; i < tokens.Length; i++)
        {
            offsets[i] = offsets[i - 1] + tokens[i - 1].Length + 1;
        }

        Dictionary<CompanionEmotion, double> scores = [];
        Dictionary<CompanionEmotion, int> firstMention = [];
        foreach ((CompanionEmotion emotion, Entry[] baseEntries) in Emotions)
        {
            Entry[] entries = [.. baseEntries, .. Colloquial.GetValueOrDefault(emotion, [])];
            double score = 0;
            int first = int.MaxValue;
            foreach (Entry entry in entries)
            {
                foreach (string term in entry.Terms)
                {
                    int at = IndexOf(term, normalized, tokens, offsets, respectNegation: true);
                    if (at >= 0)
                    {
                        score += entry.Weight;
                        first = Math.Min(first, at);
                    }
                }
            }

            if (score > 0)
            {
                scores[emotion] = score;
                firstMention[emotion] = first;
            }
        }

        bool Matches(string term, bool respectNegation) => IndexOf(term, normalized, tokens, offsets, respectNegation) >= 0;

        bool hasBody = BodyTerms.Any(term => Matches(term, respectNegation: false));
        if (hasBody && scores.ContainsKey(CompanionEmotion.Anxiety))
        {
            scores[CompanionEmotion.Panic] = scores.GetValueOrDefault(CompanionEmotion.Panic) + 1;
            firstMention.TryAdd(CompanionEmotion.Panic, firstMention[CompanionEmotion.Anxiety]);
        }

        bool intense = IntensityTerms.Any(term => Matches(term, respectNegation: false));
        IReadOnlyList<CompanionTheme> themes = Themes
            .Select(pair => (Theme: pair.Key, Hits: pair.Value.Count(term => Matches(term, respectNegation: false))))
            .Where(x => x.Hits > 0)
            .OrderByDescending(x => x.Hits)
            .Take(2)
            .Select(x => x.Theme)
            .ToArray();

        IReadOnlyList<CompanionPerson> persons = PersonTerms
            .Select(pair => (Person: pair.Key, Hits: pair.Value.Count(term => Matches(term, respectNegation: false))))
            .Where(x => x.Hits > 0)
            .OrderByDescending(x => x.Hits)
            .Take(2)
            .Select(x => x.Person)
            .ToArray();

        // Equal scores are broken by whichever state the person mentioned first.
        List<KeyValuePair<CompanionEmotion, double>> ranked = scores
            .OrderByDescending(p => p.Value)
            .ThenBy(p => firstMention[p.Key])
            .ToList();
        if (ranked.Count == 0 || ranked[0].Value < MinScore)
        {
            return new SituationAnalysis(CompanionEmotion.Unknown, 0, hasBody, intense, themes, CompanionEmotion.Unknown, persons);
        }

        double top = ranked[0].Value;
        double second = ranked.Count > 1 ? ranked[1].Value : 0;
        CompanionEmotion secondary = ranked.Count > 1 && ranked[1].Value >= 2 && ranked[1].Value >= top * 0.6 ? ranked[1].Key : CompanionEmotion.Unknown;
        return new SituationAnalysis(ranked[0].Key, top / (top + second + 1), hasBody, intense, themes, secondary, persons);
    }

    private static string Normalize(string text)
    {
        char[] chars = text.ToLowerInvariant().Replace('ё', 'е').Replace("'", string.Empty).Replace("’", string.Empty)
            .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
            .ToArray();
        return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private const int PhraseWordGap = 2;

    /// <returns>Character position of the earliest match in the normalised text, or -1.</returns>
    private static int IndexOf(string term, string normalized, string[] tokens, int[] offsets, bool respectNegation)
    {
        string t = term.Replace('ё', 'е');

        if (t.Contains(' '))
        {
            return IndexOfPhrase(t.Split(' ', StringSplitOptions.RemoveEmptyEntries), tokens, offsets, respectNegation);
        }

        bool exact = t.StartsWith('=');
        string key = exact ? t[1..] : t;
        for (int i = 0; i < tokens.Length; i++)
        {
            bool hit = exact ? tokens[i] == key : tokens[i].StartsWith(key, StringComparison.Ordinal);
            if (!hit)
            {
                continue;
            }

            if (respectNegation && IsNegated(tokens, i))
            {
                continue;
            }

            return offsets[i];
        }

        return -1;
    }

    /// <summary>A negation word right before the match, or one <see cref="NegationModifiers"/> word away from it
    /// ("не сильно тревожусь"). Not checked after the match: normalization drops sentence punctuation, so "тревожусь.
    /// Не могу спать" and "тревожусь не могу спать" become indistinguishable and a look-ahead would misfire on the first.</summary>
    private static bool IsNegated(string[] tokens, int index)
    {
        if (index >= 1 && Negations.Contains(tokens[index - 1]))
        {
            return true;
        }

        return index >= 2 && NegationModifiers.Contains(tokens[index - 1]) && Negations.Contains(tokens[index - 2]);
    }

    /// <summary>Matches a multi-word term even when up to <see cref="PhraseWordGap"/> extra words are inserted between
    /// its words ("не могу нормально дышать" still matches "не могу дышать"), and skips it when immediately negated.</summary>
    /// <returns>Character position of the phrase's first word, or -1.</returns>
    private static int IndexOfPhrase(string[] words, string[] tokens, int[] offsets, bool respectNegation)
    {
        for (int start = 0; start < tokens.Length; start++)
        {
            if (!tokens[start].StartsWith(words[0], StringComparison.Ordinal))
            {
                continue;
            }

            if (respectNegation && start > 0 && Negations.Contains(tokens[start - 1]))
            {
                continue;
            }

            int cursor = start;
            bool matched = true;
            for (int w = 1; w < words.Length; w++)
            {
                int next = -1;
                int limit = Math.Min(tokens.Length - 1, cursor + PhraseWordGap + 1);
                for (int k = cursor + 1; k <= limit; k++)
                {
                    if (tokens[k].StartsWith(words[w], StringComparison.Ordinal))
                    {
                        next = k;
                        break;
                    }
                }

                if (next < 0)
                {
                    matched = false;
                    break;
                }

                cursor = next;
            }

            if (matched)
            {
                return offsets[start];
            }
        }

        return -1;
    }
}
