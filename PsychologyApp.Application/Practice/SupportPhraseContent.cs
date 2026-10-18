using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

/// <summary>Reviewed ready phrases for balanced thoughts and kind words, matched to the kind of harsh thought the person wrote.</summary>
internal static class SupportPhraseContent
{
    public const int MaxPhraseLength = 120;

    /// <summary>A trap in thinking, recognised by word stems, and the balanced answers that fit it.</summary>
    private sealed record Pattern(string[] Stems, string[] Phrases);

    private static readonly Pattern[] ReframeRu =
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
    ];

    private static readonly Pattern[] ReframeEn =
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
    ];

    private static readonly string[] ReframeGeneralRu =
    [
        "Это мысль, а не факт",
        "Мне сейчас тяжело, и это пройдёт",
        "Другие на моём месте тоже бы растерялись",
        "Я могу сделать один маленький шаг",
        "Я справлялся(ась) с трудным раньше",
        "Чувства сильные, но они не опасны",
        "Через месяц это будет казаться меньше",
        "Мне не обязательно решать всё сразу"
    ];

    private static readonly string[] ReframeGeneralEn =
    [
        "This is a thought, not a fact",
        "It's hard right now, and it will pass",
        "Others in my place would feel lost too",
        "I can take one small step",
        "I have coped with hard things before",
        "The feelings are strong, but they are not dangerous",
        "In a month this will seem smaller",
        "I don't have to solve everything at once"
    ];

    private static readonly string[] KindnessRu =
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
    ];

    private static readonly string[] KindnessEn =
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
    ];

    private static readonly string[] SelfKindnessRu =
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
    ];

    private static readonly string[] SelfKindnessEn =
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
    ];

    public static List<string> Pick(EntryFieldRole role, string? context, bool english) => role switch
    {
        EntryFieldRole.Reframe => PickReframes(context, english),
        EntryFieldRole.Kindness => [.. english ? KindnessEn : KindnessRu],
        EntryFieldRole.SelfKindness => [.. english ? SelfKindnessEn : SelfKindnessRu],
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

    private static List<string> PickReframes(string? context, bool english)
    {
        string text = $" {context ?? string.Empty} ".ToLowerInvariant();
        List<string> matched = (english ? ReframeEn : ReframeRu)
            .Where(pattern => pattern.Stems.Any(stem => text.Contains(stem, StringComparison.Ordinal)))
            .SelectMany(pattern => pattern.Phrases)
            .ToList();

        return [.. matched, .. english ? ReframeGeneralEn : ReframeGeneralRu];
    }
}
