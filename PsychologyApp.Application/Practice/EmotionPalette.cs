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

    private static readonly EmotionGroup[] French =
    [
        new(EmotionFamily.Fear, "Peur", ["Anxiété", "Inquiétude", "Panique", "Insécurité", "Désarroi"]),
        new(EmotionFamily.Sadness, "Tristesse", ["Chagrin", "Solitude", "Déception", "Mélancolie", "Impuissance"]),
        new(EmotionFamily.Anger, "Colère", ["Vexation", "Irritation", "Indignation", "Jalousie", "Rage"]),
        new(EmotionFamily.Shame, "Honte", ["Culpabilité", "Malaise", "Embarras", "Humiliation"]),
        new(EmotionFamily.Disgust, "Dégoût", ["Antipathie", "Mépris", "Répulsion"]),
        new(EmotionFamily.Joy, "Joie", ["Soulagement", "Calme", "Gratitude", "Espoir", "Fierté"])
    ];

    private static readonly EmotionGroup[] German =
    [
        new(EmotionFamily.Fear, "Angst", ["Ängstlichkeit", "Sorge", "Panik", "Unsicherheit", "Ratlosigkeit"]),
        new(EmotionFamily.Sadness, "Traurigkeit", ["Kummer", "Einsamkeit", "Enttäuschung", "Sehnsucht", "Hilflosigkeit"]),
        new(EmotionFamily.Anger, "Wut", ["Kränkung", "Gereiztheit", "Empörung", "Eifersucht", "Zorn"]),
        new(EmotionFamily.Shame, "Scham", ["Schuld", "Verlegenheit", "Peinlichkeit", "Demütigung"]),
        new(EmotionFamily.Disgust, "Ekel", ["Abneigung", "Verachtung", "Widerwille"]),
        new(EmotionFamily.Joy, "Freude", ["Erleichterung", "Ruhe", "Dankbarkeit", "Hoffnung", "Stolz"])
    ];

    private static readonly EmotionGroup[] Spanish =
    [
        new(EmotionFamily.Fear, "Miedo", ["Ansiedad", "Preocupación", "Pánico", "Inseguridad", "Desconcierto"]),
        new(EmotionFamily.Sadness, "Tristeza", ["Pena", "Soledad", "Decepción", "Añoranza", "Impotencia"]),
        new(EmotionFamily.Anger, "Enojo", ["Resentimiento", "Irritación", "Indignación", "Celos", "Furia"]),
        new(EmotionFamily.Shame, "Vergüenza", ["Culpa", "Incomodidad", "Bochorno", "Humillación"]),
        new(EmotionFamily.Disgust, "Asco", ["Antipatía", "Desprecio", "Repulsión"]),
        new(EmotionFamily.Joy, "Alegría", ["Alivio", "Calma", "Gratitud", "Esperanza", "Orgullo"])
    ];

    private static readonly EmotionGroup[] Portuguese =
    [
        new(EmotionFamily.Fear, "Medo", ["Ansiedade", "Preocupação", "Pânico", "Insegurança", "Confusão"]),
        new(EmotionFamily.Sadness, "Tristeza", ["Pesar", "Solidão", "Decepção", "Saudade", "Impotência"]),
        new(EmotionFamily.Anger, "Raiva", ["Mágoa", "Irritação", "Indignação", "Ciúme", "Fúria"]),
        new(EmotionFamily.Shame, "Vergonha", ["Culpa", "Desconforto", "Constrangimento", "Humilhação"]),
        new(EmotionFamily.Disgust, "Nojo", ["Antipatia", "Desprezo", "Repulsa"]),
        new(EmotionFamily.Joy, "Alegria", ["Alívio", "Calma", "Gratidão", "Esperança", "Orgulho"])
    ];

    private static readonly EmotionGroup[] Chinese =
    [
        new(EmotionFamily.Fear, "恐惧", ["焦虑", "担心", "惊慌", "不安", "茫然"]),
        new(EmotionFamily.Sadness, "悲伤", ["哀伤", "孤独", "失望", "思念", "无力"]),
        new(EmotionFamily.Anger, "愤怒", ["委屈", "烦躁", "气愤", "嫉妒", "暴怒"]),
        new(EmotionFamily.Shame, "羞耻", ["内疚", "尴尬", "难为情", "屈辱"]),
        new(EmotionFamily.Disgust, "厌恶", ["反感", "鄙视", "恶心"]),
        new(EmotionFamily.Joy, "喜悦", ["释然", "平静", "感激", "希望", "自豪"])
    ];

    /// <summary>The wheel in a language by its key ("ru", "en", "fr"…); a language without its own words gets the English ones.</summary>
    public static IReadOnlyList<EmotionGroup> For(string? language) =>
        language?.ToLowerInvariant() switch
        {
            "ru" => Russian,
            "fr" => French,
            "de" => German,
            "es" => Spanish,
            "pt" => Portuguese,
            "zh" => Chinese,
            _ => English
        };
}
