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
            [ArticleIds.Anger, ArticleIds.SelfCriticism, ArticleIds.ShameAndGuilt, ArticleIds.Grief, ArticleIds.Numbing]),
        ("trauma", "Травма", "Trauma",
            [ArticleIds.TraumaStart, ArticleIds.TraumaReaction, ArticleIds.TraumaFlashbacks, ArticleIds.TraumaNightmares, ArticleIds.TraumaTreatment]),
        ("relationships", "Отношения", "Relationships",
            [ArticleIds.Boundaries, ArticleIds.Conflicts, ArticleIds.Narcissism, ArticleIds.EmotionalAbuse, ArticleIds.ParentStress, ArticleIds.HelpingLovedOne]),
        ("help", "Тело и помощь", "Body and help",
            [ArticleIds.BodyAndStress, ArticleIds.ChronicPain, ArticleIds.Specialist, ArticleIds.SupportInCrisis]),
        ("recognise", "Распознать и обратиться", "Recognise and get help",
            [ArticleIds.EatingDisorders, ArticleIds.Bipolar, ArticleIds.Psychosis])
    ];

    public static IReadOnlyList<ArticleSection> Sections(bool english) =>
        Layout
            .Select(section => new ArticleSection(
                section.Key,
                english ? section.English : section.Russian,
                section.Ids.Select(id => Find(id, english)).OfType<Article>().ToList()))
            .ToList();
}
