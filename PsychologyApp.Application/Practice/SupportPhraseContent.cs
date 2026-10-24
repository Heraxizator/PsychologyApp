using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

/// <summary>Reviewed ready phrases for balanced thoughts and kind words, matched to the kind of harsh thought the person wrote.</summary>
internal static class SupportPhraseContent
{
    public const int MaxPhraseLength = 120;

    /// <summary>A trap in thinking, recognised by word stems, and the balanced answers that fit it.</summary>
    private sealed record Pattern(string[] Stems, string[] Phrases);

    /// <summary>The phrases of one language; every language has the same five traps in the same order.</summary>
    private sealed record PhraseSet(Pattern[] Reframe, string[] ReframeGeneral, string[] Kindness, string[] SelfKindness);

    private static readonly PhraseSet Ru = new(
        [
            new(["всегда", "никогда", "всё ", "все ", "никто", "ничего"],
                ["«Всегда» и «никогда» — редко правда: бывало и по-другому", "Это одна ситуация, а не вся моя жизнь"]),
            new(["испорч", "не справл", "катастроф", "провал", "конец", "ужасн"],
                ["Даже если будет трудно, я уже справлялся(ась) с трудным", "Ошибку можно исправить — это не катастрофа"]),
            new(["подума", "осуд", "считают", "не уважа", "смеют", "игнор"],
                ["Я не знаю наверняка, что думают другие", "Возможно, люди заняты своими делами, а не мной"]),
            new(["нужен", "нужна", "одинок", "не люб", "никому", "брос"],
                ["Одно молчание не значит, что я никому не нужен(на)", "Есть люди, которым я дорог(а), даже если сейчас я этого не чувствую"]),
            new(["неудачник", "глуп", "плох", "ничтожеств", "бездар", "хуже всех"],
                ["Я человек, который ошибся, а не «неудачник»", "Один промах не определяет меня целиком"])
        ],
        [
            "Это мысль, а не факт",
            "Мне сейчас тяжело, и это пройдёт",
            "Другие на моём месте тоже бы растерялись",
            "Я могу сделать один маленький шаг",
            "Я справлялся(ась) с трудным раньше",
            "Чувства сильные, но они не опасны",
            "Через месяц это будет казаться меньше",
            "Мне не обязательно решать всё сразу"
        ],
        [
            "Ты старался(ась), и это уже много",
            "Ошибаться — по-человечески",
            "Это поправимо, давай разберёмся вместе",
            "Тебе сейчас тяжело, и это нормально",
            "Я рядом, ты не один(одна)",
            "Ты не обязан(а) быть идеальным(ой)",
            "Отдохни, ты это заслужил(а)",
            "Одна ошибка не перечёркивает всё хорошее",
            "Ты важен(на) мне таким(ой), какой(ая) ты есть"
        ],
        [
            "Я делаю всё, что могу",
            "Ошибаться — это по-человечески",
            "Я могу учиться на этом",
            "Мне сейчас тяжело, и я заслуживаю поддержки",
            "Я не обязан(а) быть идеальным(ой)",
            "Я имею право на отдых",
            "Одна ошибка не определяет меня",
            "Я учусь, и это нормально",
            "Я отношусь к себе так же бережно, как к другу"
        ]);

    private static readonly PhraseSet En = new(
        [
            new(["always", "never", "everyone", "nobody", "everything", "nothing"],
                ["\"Always\" and \"never\" are rarely true: it has gone differently before", "This is one situation, not my whole life"]),
            new(["ruin", "can't cope", "cannot cope", "disaster", "fail", "terrible"],
                ["Even if it's hard, I've coped with hard things before", "A mistake can be fixed; it isn't a disaster"]),
            new(["judge", "think i", "think of me", "respect", "laugh", "ignor"],
                ["I don't know for sure what others think", "People may be busy with their own lives, not with me"]),
            new(["needs me", "need me", "alone", "lonely", "unloved", "abandon"],
                ["One silence doesn't mean nobody needs me", "There are people who care about me, even if I can't feel it now"]),
            new(["loser", "stupid", "worthless", "useless", "not good enough", "the worst"],
                ["I'm a person who made a mistake, not a \"loser\"", "One slip doesn't define all of me"])
        ],
        [
            "This is a thought, not a fact",
            "It's hard right now, and it will pass",
            "Others in my place would feel lost too",
            "I can take one small step",
            "I have coped with hard things before",
            "The feelings are strong, but they are not dangerous",
            "In a month this will seem smaller",
            "I don't have to solve everything at once"
        ],
        [
            "You tried, and that already counts",
            "Making mistakes is human",
            "This can be fixed, let's sort it out together",
            "It's hard for you right now, and that's okay",
            "I'm here, you're not alone",
            "You don't have to be perfect",
            "Rest, you have earned it",
            "One mistake doesn't cancel all the good",
            "You matter to me just as you are"
        ],
        [
            "I'm doing the best I can",
            "Making mistakes is human",
            "I can learn from this",
            "It's hard right now, and I deserve support",
            "I don't have to be perfect",
            "I have a right to rest",
            "One mistake doesn't define me",
            "I'm learning, and that's okay",
            "I treat myself as kindly as I would a friend"
        ]);

    private static readonly PhraseSet Fr = new(
        [
            new(["toujours", "jamais", "personne", "tout le monde", "rien", "tout "],
                ["« Toujours » et « jamais » sont rarement vrais : ça s'est déjà passé autrement", "C'est une situation, pas toute ma vie"]),
            new(["gâch", "rater", "raté", "catastroph", "échec", "horrible"],
                ["Même si c'est difficile, j'ai déjà traversé des moments difficiles", "Une erreur peut se réparer, ce n'est pas une catastrophe"]),
            new(["pense", "juge", "se moqu", "respect", "ignor", "critiqu"],
                ["Je ne sais pas avec certitude ce que pensent les autres", "Les gens sont peut-être occupés par leur vie, pas par moi"]),
            new(["besoin de moi", "seul", "aime pas", "abandonn", "rejet", "personne ne"],
                ["Un silence ne veut pas dire que personne n'a besoin de moi", "Il y a des gens qui tiennent à moi, même si je ne le sens pas maintenant"]),
            new(["nul", "bête", "stupide", "incapable", "bon à rien", "le pire"],
                ["Je suis quelqu'un qui s'est trompé, pas un « raté »", "Un faux pas ne me définit pas tout entier"])
        ],
        [
            "C'est une pensée, pas un fait",
            "C'est difficile en ce moment, et ça va passer",
            "D'autres à ma place seraient perdus aussi",
            "Je peux faire un petit pas",
            "J'ai déjà traversé des choses difficiles",
            "Les émotions sont fortes, mais elles ne sont pas dangereuses",
            "Dans un mois, cela paraîtra plus petit",
            "Je n'ai pas à tout régler d'un coup"
        ],
        [
            "Tu as essayé, et c'est déjà beaucoup",
            "Se tromper, c'est humain",
            "Ça peut se réparer, voyons ça ensemble",
            "C'est dur pour toi en ce moment, et c'est normal",
            "Je suis là, tu n'es pas seul",
            "Tu n'as pas à être parfait",
            "Repose-toi, tu l'as mérité",
            "Une erreur n'efface pas tout le bon",
            "Tu comptes pour moi tel que tu es"
        ],
        [
            "Je fais de mon mieux",
            "Se tromper, c'est humain",
            "Je peux apprendre de cela",
            "C'est difficile en ce moment, et je mérite du soutien",
            "Je n'ai pas à être parfait",
            "J'ai le droit de me reposer",
            "Une erreur ne me définit pas",
            "J'apprends, et c'est normal",
            "Je me traite avec la même douceur qu'un ami"
        ]);

    private static readonly PhraseSet De = new(
        [
            new(["immer", "nie ", "niemals", "niemand", "nichts", "alle "],
                ["„Immer“ und „nie“ stimmen selten: Es ist auch schon anders gelaufen", "Das ist eine Situation, nicht mein ganzes Leben"]),
            new(["kaputt", "vermassel", "katastroph", "versag", "schaffe es nicht", "schrecklich"],
                ["Auch wenn es schwer wird, habe ich schon Schweres geschafft", "Ein Fehler lässt sich beheben, das ist keine Katastrophe"]),
            new(["denken", "urteil", "lachen", "respekt", "ignor", "halten mich"],
                ["Ich weiß nicht sicher, was andere denken", "Vielleicht sind die Leute mit sich beschäftigt, nicht mit mir"]),
            new(["braucht mich", "allein", "einsam", "nicht geliebt", "verlass", "niemand mag"],
                ["Ein Schweigen heißt nicht, dass mich niemand braucht", "Es gibt Menschen, denen ich wichtig bin, auch wenn ich es gerade nicht spüre"]),
            new(["versager", "dumm", "wertlos", "nutzlos", "nicht gut genug", "der schlechteste"],
                ["Ich bin ein Mensch, der einen Fehler gemacht hat, kein „Versager“", "Ein Ausrutscher bestimmt nicht, wer ich bin"])
        ],
        [
            "Das ist ein Gedanke, keine Tatsache",
            "Es ist gerade schwer, und es geht vorbei",
            "Andere an meiner Stelle wären auch ratlos",
            "Ich kann einen kleinen Schritt machen",
            "Ich habe schon früher Schweres geschafft",
            "Die Gefühle sind stark, aber nicht gefährlich",
            "In einem Monat wird das kleiner wirken",
            "Ich muss nicht alles auf einmal lösen"
        ],
        [
            "Du hast es versucht, und das zählt schon",
            "Fehler zu machen ist menschlich",
            "Das lässt sich beheben, lass es uns zusammen anschauen",
            "Es ist gerade schwer für dich, und das ist okay",
            "Ich bin da, du bist nicht allein",
            "Du musst nicht perfekt sein",
            "Ruh dich aus, du hast es dir verdient",
            "Ein Fehler löscht nicht all das Gute aus",
            "Du bist mir wichtig, so wie du bist"
        ],
        [
            "Ich tue, was ich kann",
            "Fehler zu machen ist menschlich",
            "Ich kann daraus lernen",
            "Es ist gerade schwer, und ich verdiene Unterstützung",
            "Ich muss nicht perfekt sein",
            "Ich habe ein Recht auf Ruhe",
            "Ein Fehler bestimmt nicht, wer ich bin",
            "Ich lerne, und das ist in Ordnung",
            "Ich behandle mich so freundlich wie einen Freund"
        ]);

    private static readonly PhraseSet Es = new(
        [
            new(["siempre", "nunca", "nadie", "todo el mundo", "nada", "todo "],
                ["«Siempre» y «nunca» rara vez son verdad: antes salió de otra manera", "Es una situación, no toda mi vida"]),
            new(["estropea", "arruin", "no puedo con", "desastre", "fracas", "horrible"],
                ["Aunque sea difícil, ya he salido adelante en cosas difíciles", "Un error se puede arreglar, no es un desastre"]),
            new(["pensar", "piensan", "juzg", "se ríen", "respet", "ignor"],
                ["No sé con seguridad lo que piensan los demás", "Quizá la gente está ocupada con su vida, no conmigo"]),
            new(["me necesita", "solo", "sola", "no me quiere", "abandon", "rechaz"],
                ["Un silencio no significa que nadie me necesite", "Hay personas que me quieren, aunque ahora no lo sienta"]),
            new(["fracasado", "tonto", "inútil", "no valgo", "no soy suficiente", "el peor"],
                ["Soy una persona que se equivocó, no un «fracasado»", "Un tropiezo no me define entero"])
        ],
        [
            "Es un pensamiento, no un hecho",
            "Ahora es difícil, y va a pasar",
            "Otros en mi lugar también se sentirían perdidos",
            "Puedo dar un pequeño paso",
            "Ya he salido adelante en cosas difíciles",
            "Las emociones son fuertes, pero no son peligrosas",
            "Dentro de un mes esto parecerá más pequeño",
            "No tengo que resolverlo todo de una vez"
        ],
        [
            "Lo intentaste, y eso ya cuenta",
            "Equivocarse es humano",
            "Esto tiene arreglo, veámoslo juntos",
            "Ahora lo estás pasando mal, y está bien",
            "Estoy aquí, no estás solo",
            "No tienes que ser perfecto",
            "Descansa, te lo has ganado",
            "Un error no borra todo lo bueno",
            "Me importas tal como eres"
        ],
        [
            "Hago lo que puedo",
            "Equivocarse es humano",
            "Puedo aprender de esto",
            "Ahora es difícil, y merezco apoyo",
            "No tengo que ser perfecto",
            "Tengo derecho a descansar",
            "Un error no me define",
            "Estoy aprendiendo, y está bien",
            "Me trato con la misma amabilidad que a un amigo"
        ]);

    private static readonly PhraseSet Pt = new(
        [
            new(["sempre", "nunca", "ninguém", "todo mundo", "nada", "tudo "],
                ["«Sempre» e «nunca» raramente são verdade: já foi diferente antes", "É uma situação, não a minha vida inteira"]),
            new(["estragu", "estrag", "não dou conta", "desastre", "fracass", "horrível"],
                ["Mesmo que seja difícil, já dei conta de coisas difíceis", "Um erro pode ser corrigido, não é um desastre"]),
            new(["pensar", "pensam", "julg", "rir de mim", "respeit", "ignor"],
                ["Não sei com certeza o que os outros pensam", "Talvez as pessoas estejam ocupadas com a vida delas, não comigo"]),
            new(["precisa de mim", "sozinh", "não me ama", "abandon", "rejeit", "ninguém me"],
                ["Um silêncio não significa que ninguém precisa de mim", "Há pessoas que gostam de mim, mesmo que eu não sinta isso agora"]),
            new(["fracassad", "burr", "inútil", "não presto", "não sou bom", "o pior"],
                ["Sou uma pessoa que errou, não um «fracassado»", "Um deslize não me define por inteiro"])
        ],
        [
            "É um pensamento, não um fato",
            "Está difícil agora, e vai passar",
            "Outros no meu lugar também ficariam perdidos",
            "Posso dar um pequeno passo",
            "Já dei conta de coisas difíceis antes",
            "As emoções são fortes, mas não são perigosas",
            "Daqui a um mês isso vai parecer menor",
            "Não preciso resolver tudo de uma vez"
        ],
        [
            "Você tentou, e isso já conta",
            "Errar é humano",
            "Isso tem conserto, vamos ver juntos",
            "Está difícil para você agora, e tudo bem",
            "Estou aqui, você não está sozinho",
            "Você não precisa ser perfeito",
            "Descanse, você mereceu",
            "Um erro não apaga todo o bem",
            "Você é importante para mim do jeito que é"
        ],
        [
            "Estou fazendo o melhor que posso",
            "Errar é humano",
            "Posso aprender com isso",
            "Está difícil agora, e eu mereço apoio",
            "Não preciso ser perfeito",
            "Tenho direito de descansar",
            "Um erro não me define",
            "Estou aprendendo, e tudo bem",
            "Eu me trato com a mesma gentileza que trataria um amigo"
        ]);

    private static readonly PhraseSet Zh = new(
        [
            new(["总是", "从来", "永远", "没有人", "所有人", "什么都"],
                ["“总是”和“从来”很少是真的：以前也有过不一样的时候", "这只是一件事，不是我的整个人生"]),
            new(["搞砸", "完了", "应付不了", "灾难", "失败", "糟糕"],
                ["即使会很难，我以前也熬过了困难", "错误是可以改正的，这不是灾难"]),
            new(["怎么看", "觉得我", "评判", "笑话", "不尊重", "无视"],
                ["我并不确定别人在想什么", "也许别人在忙自己的事，而不是在关注我"]),
            new(["需要我", "孤单", "孤独", "不爱", "抛弃", "没人要"],
                ["一次沉默不代表没有人需要我", "有人在乎我，即使我现在感觉不到"]),
            new(["失败者", "笨", "没用", "一无是处", "不够好", "最差"],
                ["我是一个犯了错的人，而不是“失败者”", "一次失误不能定义整个我"])
        ],
        [
            "这是一个想法，不是事实",
            "现在很难，但会过去的",
            "换作别人，也会不知所措",
            "我可以迈出一小步",
            "我以前也熬过了困难",
            "情绪很强烈，但它们并不危险",
            "一个月后，这件事会显得没那么大",
            "我不必一下子解决所有问题"
        ],
        [
            "你尽力了，这已经很不容易",
            "犯错是人之常情",
            "这是可以弥补的，我们一起想想办法",
            "你现在很难受，这很正常",
            "我在这里，你并不孤单",
            "你不必完美",
            "休息一下吧，这是你应得的",
            "一个错误抹不掉所有的好",
            "你本来的样子对我就很重要"
        ],
        [
            "我已经尽力了",
            "犯错是人之常情",
            "我可以从中学习",
            "现在很难，我值得被支持",
            "我不必完美",
            "我有休息的权利",
            "一个错误不能定义我",
            "我在学习，这很正常",
            "我像对待朋友一样温和地对待自己"
        ]);

    /// <summary>The phrases in a language by its key; a language without its own gets the English ones.</summary>
    private static PhraseSet For(string? language) => language?.ToLowerInvariant() switch
    {
        "ru" => Ru,
        "fr" => Fr,
        "de" => De,
        "es" => Es,
        "pt" => Pt,
        "zh" => Zh,
        _ => En
    };

    public static List<string> Pick(EntryFieldRole role, string? context, string? language) => role switch
    {
        EntryFieldRole.Reframe => PickReframes(context, For(language)),
        EntryFieldRole.Kindness => [.. For(language).Kindness],
        EntryFieldRole.SelfKindness => [.. For(language).SelfKindness],
        _ => []
    };

    public static string SystemPrompt(EntryFieldRole role, bool english) => (role, english) switch
    {
        (EntryFieldRole.Reframe, false) =>
            "Человек записал автоматическую мысль. Предложи две короткие сбалансированные мысли от первого лица (до 12 слов каждая), " +
            "реалистичные, без советов, без диагнозов, без «всё будет хорошо». Каждую с новой строки, без нумерации.",
        (EntryFieldRole.Reframe, true) =>
            "The person wrote down an automatic thought. Suggest two short balanced thoughts in the first person (up to 12 words each), " +
            "realistic, no advice, no diagnoses, no \"everything will be fine\". One per line, no numbering.",
        (_, false) =>
            "Человек записал, за что критикует себя. Предложи две короткие добрые фразы себе от первого лица (до 12 слов каждая), " +
            "без советов и без обесценивания. Каждую с новой строки, без нумерации.",
        _ =>
            "The person wrote what they criticise themselves for. Suggest two short kind phrases to oneself in the first person (up to 12 words each), " +
            "no advice and no dismissing. One per line, no numbering."
    };

    private static List<string> PickReframes(string? context, PhraseSet set)
    {
        string text = $" {context ?? string.Empty} ".ToLowerInvariant();
        List<string> matched = set.Reframe
            .Where(pattern => pattern.Stems.Any(stem => text.Contains(stem, StringComparison.Ordinal)))
            .SelectMany(pattern => pattern.Phrases)
            .ToList();

        return [.. matched, .. set.ReframeGeneral];
    }
}
