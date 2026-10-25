namespace PsychologyApp.Application.Articles;

/// <param name="Key">Stable, language-neutral: the filter chips use it.</param>
public sealed record ArticleSection(string Key, string Title, IReadOnlyList<Article> Articles);

public static partial class ArticleContent
{
    /// <summary>The sections of the Articles tab and the order of articles in them; every article is in exactly one (tested).</summary>
    private static readonly (string Key, string Russian, string English, string[] Ids)[] Layout =
    [
        ("anxiety", "Тревога и страх", "Anxiety and fear",
            [ArticleIds.Anxiety, ArticleIds.Panic, ArticleIds.Avoidance, ArticleIds.SocialAnxiety, ArticleIds.HealthAnxiety, ArticleIds.Ocd]),
        ("thoughts", "Мысли", "Thoughts",
            [ArticleIds.Thoughts, ArticleIds.Rumination, ArticleIds.Perfectionism, ArticleIds.Procrastination]),
        ("energy", "Силы и настроение", "Energy and mood",
            [ArticleIds.Depression, ArticleIds.LowMood, ArticleIds.Burnout, ArticleIds.Sleep, ArticleIds.Loneliness]),
        ("feelings", "Чувства", "Feelings",
            [ArticleIds.Emotions, ArticleIds.Anger, ArticleIds.SelfCriticism, ArticleIds.SelfEsteem, ArticleIds.ShameAndGuilt, ArticleIds.Grief, ArticleIds.Numbing]),
        ("trauma", "Травма", "Trauma",
            [ArticleIds.TraumaStart, ArticleIds.TraumaReaction, ArticleIds.TraumaFlashbacks, ArticleIds.TraumaNightmares, ArticleIds.TraumaTreatment]),
        ("relationships", "Отношения", "Relationships",
            [ArticleIds.Boundaries, ArticleIds.Conflicts, ArticleIds.Narcissism, ArticleIds.EmotionalAbuse, ArticleIds.ParentStress, ArticleIds.HelpingLovedOne]),
        ("help", "Тело и помощь", "Body and help",
            [ArticleIds.BodyAndStress, ArticleIds.Mindfulness, ArticleIds.ChronicPain, ArticleIds.Specialist, ArticleIds.SupportInCrisis]),
        ("recognise", "Распознать и обратиться", "Recognise and get help",
            [ArticleIds.EatingDisorders, ArticleIds.Bipolar, ArticleIds.Psychosis])
    ];

    /// <summary>The section titles of the other languages, in the order of <see cref="Layout"/>.</summary>
    private static readonly string[] SectionsFr =
        ["Anxiété et peur", "Pensées", "Énergie et humeur", "Émotions", "Traumatisme", "Relations", "Corps et aide", "Reconnaître et se faire aider"];

    private static readonly string[] SectionsDe =
        ["Angst und Furcht", "Gedanken", "Kraft und Stimmung", "Gefühle", "Trauma", "Beziehungen", "Körper und Hilfe", "Erkennen und Hilfe holen"];

    private static readonly string[] SectionsEs =
        ["Ansiedad y miedo", "Pensamientos", "Energía y ánimo", "Emociones", "Trauma", "Relaciones", "Cuerpo y ayuda", "Reconocer y pedir ayuda"];

    private static readonly string[] SectionsPt =
        ["Ansiedade e medo", "Pensamentos", "Energia e humor", "Emoções", "Trauma", "Relacionamentos", "Corpo e ajuda", "Reconhecer e buscar ajuda"];

    private static readonly string[] SectionsZh =
        ["焦虑与恐惧", "想法", "精力与情绪", "情绪感受", "创伤", "人际关系", "身体与求助", "识别与求助"];

    public static IReadOnlyList<ArticleSection> Sections(string? language) =>
        Layout
            .Select((section, index) => new ArticleSection(
                section.Key,
                InLanguage(language, section.Russian, section.English, SectionsFr[index], SectionsDe[index], SectionsEs[index], SectionsPt[index], SectionsZh[index]),
                section.Ids.Select(id => Find(id, language)).OfType<Article>().ToList()))
            .ToList();
}
