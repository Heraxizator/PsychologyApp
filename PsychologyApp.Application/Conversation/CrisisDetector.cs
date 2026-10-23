namespace PsychologyApp.Application.Conversation;

public interface ICrisisDetector
{
    bool IsCrisis(string? text);
}

/// <summary>
/// Conservative phrase matcher that runs locally before any scenario logic or language model sees free text.
/// It favours false positives: a wrongly shown helpline card costs little, a missed one costs a lot.
/// Text is normalised (lower case, "ё" = "е", apostrophes removed, punctuation collapsed) so "Don't" and "dont" behave the same.
/// </summary>
public sealed class KeywordCrisisDetector : ICrisisDetector
{
    private static readonly string[] Markers =
    [
        // Russian
        "суицид", "самоубий", "покончить с собой", "покончу с собой", "убить себя", "убью себя", "покончить с жизнью", "уйти из жизни",
        "не хочу жить", "не хочется жить", "не хочу больше жить", "жить не хочу", "хочу умереть", "хочется умереть", "хочу сдохнуть", "хочу исчезнуть", "хочется исчезнуть",
        "не хочу быть живым", "не хочу быть живой", "не хочу просыпаться", "уснуть и не проснуться",
        "лучше бы я умер", "лучше умереть", "лучше бы меня не было", "жить не стоит", "смысла жить нет", "нет смысла жить", "не вижу смысла жить",
        "не вижу смысла в жизни", "жизнь не имеет смысла", "надоело жить", "устал жить", "устала жить",
        "свести счеты", "наложить на себя руки", "вскрыть вены", "повеситься", "спрыгнуть с крыши",
        "причинить себе вред", "навредить себе", "режу себя", "резать себя", "самоповрежд",
        "повешусь", "вешаться", "повесить себя", "вскрою вены", "порезать вены", "перерезать вены", "вскрыться", "вскроюсь",
        "выйти в окно", "выброситься из окна", "выпрыгнуть из окна", "спрыгнуть с моста", "прыгнуть с моста", "броситься под поезд",
        "наглотаться таблеток", "выпить все таблетки", "выпью все таблетки", "хочу покончить", "умереть хочу", "лучше сдохнуть",
        "меня не станет", "чтобы меня не стало", "лучше без меня", "лучше будет без меня", "не хочу существовать",
        "всем будет лучше без меня", "жизнь не стоит", "не стоит жить",
        "незачем жить", "лишняя в этом мире", "лишний в этом мире", "лучше бы не было меня", "лучше бы не было", "без меня всем было бы легче", "без меня было бы легче", "без меня будет лучше", "без меня всем лучше",
        "прыгнуть с крыши", "прыгнуть с балкона", "собираюсь прыгнуть", "стою на мосту", "стою на крыше", "стою на краю",
        "проглотила все таблетки", "проглотил все таблетки", "выпила все таблетки", "выпил все таблетки", "съела все таблетки", "съел все таблетки",
        "всё прекратить", "все прекратить", "чтобы все закончилось навсегда", "покончить со всем", "со всем покончить", "пора заканчивать", "уйти навсегда", "хочу уйти навсегда",
        "не хочу быть здесь", "не хочу тут быть", "себя убью", "себя убить", "накончу", "суецид", "суицыд", "сиуцид", "суицит",
        "режу руки", "порезала себя", "порезал себя", "причиняю себе боль", "делаю себе больно",
        // Ways of dying. "сброситься" alone is also "to chip in money", so only with where from.
        "застрелиться", "застрелюсь", "застрелить себя", "утопиться", "утоплюсь", "отравить себя", "отравлюсь насмерть", "хочу отравиться",
        "сброситься с крыши", "сброситься с моста", "сброситься с балкона", "сброшусь с крыши", "сброшусь с моста", "сброшусь с балкона",
        "выпрыгну из окна", "выпрыгну с балкона", "выпрыгну с крыши", "шагнуть из окна", "шагнуть с крыши", "броситься под машину", "брошусь под поезд", "брошусь под машину",
        "лечь на рельсы", "лягу на рельсы", "хочу убиться", "задушить себя",
        // English (apostrophes are stripped before matching)
        "kill myself", "suicide", "suicidal", "want to die", "end my life", "end it all", "take my own life", "hurt myself",
        "self harm", "selfharm", "no reason to live", "nothing to live for", "not worth living", "better off dead", "wish i was dead",
        "wish i were dead", "wish i was never born", "wish i wasnt alive", "want to disappear", "cant go on", "kill me",
        "dont want to live", "do not want to live", "dont want to be alive", "dont want to be here anymore", "no longer want to live",
        "cut myself", "cutting myself", "slit my wrists", "hang myself", "shoot myself", "overdose on purpose", "take all my pills",
        "jump off a bridge", "jump off the roof", "jump off a building", "hurting myself", "harm myself", "harming myself",
        "off myself", "unalive myself", "wish i werent alive", "wish i werent here", "want to be dead", "rather be dead",
        "no point in living", "no point living", "better without me", "everyone would be better off", "dont want to exist",
        "killing myself", "kill my self", "disappear forever", "bunch of pills", "too many pills", "all the pills", "no reason to keep", "dont see the point in living", "end everything",
        "dont want to wake up", "never wake up",
        "drown myself", "poison myself", "hang my self", "shoot my self", "jump in front of a train", "step in front of a train",
        "throw myself off", "throw myself under", "jump out of the window", "jump out the window",
        // French. Apostrophes are dropped before matching ("j'en ai marre" is "jen ai marre"); accents are kept, so both
        // spellings are listed where people often type without them.
        "me suicider", "suicidaire", "me tuer", "envie de mourir", "veux mourir", "voudrais mourir", "veux crever", "envie de crever",
        "mettre fin à mes jours", "mettre fin a mes jours", "mettre fin à ma vie", "mettre fin a ma vie", "en finir avec la vie", "envie den finir", "veux en finir",
        "plus envie de vivre", "pas envie de vivre", "veux plus vivre", "veux pas vivre", "marre de vivre", "aucune raison de vivre", "plus de raison de vivre",
        "mieux sans moi", "mieux mort", "mieux morte", "disparaître pour toujours", "disparaitre pour toujours", "plus me réveiller", "plus me reveiller",
        "me pendre", "me jeter sous", "me jeter du", "me jeter par la fenêtre", "me jeter par la fenetre", "sauter du pont", "sauter par la fenêtre", "sauter par la fenetre", "sauter du toit",
        "me couper les veines", "me taillader", "scarifi", "me mutiler", "automutil", "me faire du mal",
        "avaler tous mes médicaments", "avaler tous mes medicaments", "toute la boîte de", "toute la boite de", "me noyer", "mempoisonner", "me tirer une balle", "me flinguer",
        // German. Umlauts are kept; the ue/oe/ae spellings are listed too.
        "suizid", "selbstmord", "mich umbringen", "mich töten", "mich toeten", "will sterben", "möchte sterben", "moechte sterben", "will tot sein", "wäre lieber tot", "waere lieber tot", "lieber tot",
        "nicht mehr leben", "keinen sinn mehr zu leben", "kein grund zu leben", "keinen grund zu leben", "lebensmüde", "lebensmuede", "mein leben beenden", "allem ein ende setzen", "dem ganzen ein ende",
        "besser ohne mich", "besser tot", "für immer verschwinden", "fuer immer verschwinden", "nicht mehr aufwachen", "nicht mehr da sein",
        "mich ritzen", "ich ritze mich", "selbstverletz", "mir etwas antun", "mir was antun", "mich aufhängen", "mich aufhaengen", "mich erhängen", "mich erhaengen",
        "vor den zug", "springe von der brücke", "springe von der bruecke", "springe vom dach", "von der brücke springen", "von der bruecke springen", "vom dach springen", "aus dem fenster springen", "alle tabletten", "mich vergiften", "mich erschießen", "mich erschiessen", "pulsadern",
        // Spanish
        "suicid", "matarme", "quiero morir", "me quiero morir", "quisiera morir", "ganas de morir", "no quiero vivir", "no quiero seguir viviendo", "no tengo ganas de vivir",
        "acabar con mi vida", "quitarme la vida", "terminar con mi vida", "acabar con todo", "terminar con todo", "no vale la pena vivir", "no hay razón para vivir", "no hay razon para vivir",
        "mejor muerto", "mejor muerta", "mejor sin mí", "mejor sin mi", "desaparecer para siempre", "no quiero despertar", "no despertar nunca",
        "cortarme las venas", "me corto los brazos", "me corto las muñecas", "me corto las munecas", "autolesi", "hacerme daño", "hacerme dano", "ahorcarme",
        "tirarme de", "tirar del puente", "tirar de un puente", "tirar del edificio", "tirarme por la ventana", "tirarme del", "aventarme", "tomarme todas las pastillas", "todas las pastillas", "envenenarme", "pegarme un tiro",
        // Portuguese. A hyphen is a space after normalising ("matar-me" is "matar me").
        "me matar", "matar me", "quero morrer", "vontade de morrer", "não quero viver", "nao quero viver", "não quero mais viver", "nao quero mais viver",
        "acabar com a minha vida", "acabar com minha vida", "tirar minha vida", "tirar a minha vida", "tirar a minha própria vida", "dar fim à minha vida", "dar fim a minha vida",
        "acabar com tudo", "melhor sem mim", "melhor morto", "melhor morta", "sumir para sempre", "sumir pra sempre", "desaparecer para sempre", "cansado de viver", "cansada de viver",
        "não aguento mais viver", "nao aguento mais viver", "sem motivo pra viver", "sem motivo para viver", "sem razão para viver", "sem razao para viver", "não quero acordar", "nao quero acordar", "nunca mais acordar",
        "me cortar", "eu me corto", "cortar os pulsos", "me machucar", "me ferir", "automutila", "autoles", "me enforcar", "pular da ponte", "pular do prédio", "pular do predio", "pular da janela",
        "me jogar da", "me jogar na frente", "todos os remédios", "todos os remedios", "me envenenar", "dar um tiro em mim",
        // Chinese: no spaces between words, so these match inside a sentence. "跳楼" alone is also a sale ("跳楼价"), so only with intent.
        "自杀", "自殺", "轻生", "寻死", "不想活", "活不下去", "活着没意思", "活着没有意义", "活着没意义", "想死", "想去死", "不如死了", "死了算了", "一了百了",
        "结束自己的生命", "结束我的生命", "结束生命", "了结自己", "没有我会更好", "没有我大家会更好", "想消失", "永远消失", "不想醒来", "再也不醒",
        "割腕", "自残", "伤害自己", "想跳楼", "要跳楼", "去跳楼", "跳楼自杀", "站在楼顶", "跳河", "跳桥", "上吊", "吞药", "吃安眠药", "把药全吃了", "喝农药"
    ];

    /// <summary>
    /// Everyday phrases that contain a marker without meaning it, removed before matching. Each is checked in the tests both
    /// ways: alone it is not a crisis, with a real marker next to it it still is.
    /// </summary>
    // "我想死你了" is "I missed you so much"; "想死我了" is "I missed (it) terribly".
    private static readonly string[] Idioms = ["想死你", "想死我了"];

    /// <summary>
    /// Slang that is only a signal as a whole word: as a substring "kms" would match "5 kms" and "unalive" is harmless inside
    /// longer words. Matched against the normalised tokens instead of the text.
    /// </summary>
    private static readonly HashSet<string> TokenMarkers = new(StringComparer.Ordinal) { "kms", "unalive", "unaliving", "kys" };

    public bool IsCrisis(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string normalized = UtteranceText.Normalize(text);
        foreach (string idiom in Idioms)
        {
            normalized = normalized.Replace(idiom, " ", StringComparison.Ordinal);
        }

        foreach (string marker in Markers)
        {
            if (normalized.Contains(marker, StringComparison.Ordinal))
            {
                return true;
            }
        }

        string[] tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return HasSlangMarker(tokens) || NegatedWishToLive(tokens);
    }

    /// <summary>
    /// Catches loosely worded variants a fixed phrase list cannot, e.g. "не хочу уже совсем жить" or "I really don't want to keep living":
    /// a negation within three words before "want", and a "live" word within four words after it.
    /// </summary>
    // "5 kms" is distance, not slang: a bare number before the token rules it out.
    private static bool HasSlangMarker(string[] tokens)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            if (TokenMarkers.Contains(tokens[i]) && !(i > 0 && tokens[i - 1].All(char.IsDigit)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool NegatedWishToLive(string[] tokens)
    {
        // The other word order: "жить не хочется совсем", "жить мне больше не хочется".
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] is not ("жить" or "live" or "living"))
            {
                continue;
            }

            for (int k = i + 1; k < Math.Min(tokens.Length - 1, i + 4); k++)
            {
                if (tokens[k] == "не" && tokens[k + 1] is "хочется" or "хочу" or "хотелось" or "хотела" or "хотел")
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] is not ("хочу" or "хочется" or "want" or "wanna"))
            {
                continue;
            }

            bool negated = Enumerable.Range(Math.Max(0, i - 3), Math.Min(3, i))
                .Any(k => tokens[k] is "не" or "dont" or "not" or "never");
            if (!negated)
            {
                continue;
            }

            bool livesSoon = Enumerable.Range(i + 1, Math.Min(4, tokens.Length - i - 1))
                .Any(k => tokens[k] is "жить" or "live" or "living" or "alive");
            if (livesSoon)
            {
                return true;
            }
        }

        return false;
    }
}
