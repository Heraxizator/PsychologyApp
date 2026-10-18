namespace PsychologyApp.Application.Practice;

public enum EmotionFamily
{
    Fear,
    Sadness,
    Anger,
    Shame,
    Disgust,
    Joy
}

/// <param name="Name">The family itself, e.g. "Fear".</param>
/// <param name="Shades">Finer words inside the family, e.g. "anxiety", "panic": naming the exact feeling is half of the work.</param>
public sealed record EmotionGroup(EmotionFamily Family, string Name, IReadOnlyList<string> Shades);

/// <summary>A small emotion wheel: six families and their shades, so a feeling is picked with two taps instead of typed.</summary>
public static class EmotionPalette
{
    private static readonly EmotionGroup[] Russian =
    [
        new(EmotionFamily.Fear, "Страх", ["Тревога", "Беспокойство", "Паника", "Неуверенность", "Растерянность"]),
        new(EmotionFamily.Sadness, "Грусть", ["Печаль", "Одиночество", "Разочарование", "Тоска", "Бессилие"]),
        new(EmotionFamily.Anger, "Злость", ["Обида", "Раздражение", "Возмущение", "Ревность", "Ярость"]),
        new(EmotionFamily.Shame, "Стыд", ["Вина", "Неловкость", "Смущение", "Униженность"]),
        new(EmotionFamily.Disgust, "Отвращение", ["Неприязнь", "Презрение", "Брезгливость"]),
        new(EmotionFamily.Joy, "Радость", ["Облегчение", "Спокойствие", "Благодарность", "Надежда", "Гордость"])
    ];

    private static readonly EmotionGroup[] English =
    [
        new(EmotionFamily.Fear, "Fear", ["Anxiety", "Worry", "Panic", "Insecurity", "Confusion"]),
        new(EmotionFamily.Sadness, "Sadness", ["Grief", "Loneliness", "Disappointment", "Longing", "Helplessness"]),
        new(EmotionFamily.Anger, "Anger", ["Hurt", "Irritation", "Indignation", "Jealousy", "Rage"]),
        new(EmotionFamily.Shame, "Shame", ["Guilt", "Awkwardness", "Embarrassment", "Humiliation"]),
        new(EmotionFamily.Disgust, "Disgust", ["Dislike", "Contempt", "Aversion"]),
        new(EmotionFamily.Joy, "Joy", ["Relief", "Calm", "Gratitude", "Hope", "Pride"])
    ];

    public static IReadOnlyList<EmotionGroup> For(bool english) => english ? English : Russian;
}
