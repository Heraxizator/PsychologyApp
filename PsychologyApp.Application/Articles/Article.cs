using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public enum ArticleBlockKind
{
    Paragraph,
    Heading,

    /// <summary>A bulleted list in <see cref="ArticleBlock.Items"/>.</summary>
    Bullets,

    /// <summary>A heading that must not be missed (for example, when to see a doctor); shown as a warning.</summary>
    Warning,

    /// <summary>A link to a practice of the catalogue (<see cref="ArticleBlock.Technique"/>); <see cref="ArticleBlock.Text"/> is its title.</summary>
    Practice,

    /// <summary>A link to the fear ladder.</summary>
    FearLadder,

    /// <summary>A link to the crisis help screen (hotlines, the safety plan).</summary>
    CrisisHelp,

    /// <summary>A link to the questionnaire <see cref="ArticleBlock.TestId"/>; <see cref="ArticleBlock.Text"/> is how it is called.</summary>
    Tests,

    /// <summary>A link to another article (<see cref="ArticleBlock.ArticleId"/>); <see cref="ArticleBlock.Text"/> is its title.</summary>
    Article,

    /// <summary>
    /// "Do I recognise this?": statements in <see cref="ArticleBlock.Items"/> to tick. At <see cref="ArticleBlock.Threshold"/> or more
    /// ticks the reader sees <see cref="ArticleBlock.Verdict"/>, below it <see cref="ArticleBlock.VerdictLow"/>. Not a test and never a
    /// diagnosis: it points to the next step.
    /// </summary>
    Checklist,

    /// <summary>
    /// A question about what was just read, options in <see cref="ArticleBlock.Items"/>, the right one at <see cref="ArticleBlock.Correct"/>.
    /// Recalling an answer fixes it in memory better than reading again; <see cref="ArticleBlock.Explanation"/> follows any answer.
    /// </summary>
    Quiz,

    /// <summary>A breathing pacer inside the article (slow breaths with a longer out-breath), so trying it needs no other screen.</summary>
    Breathe,

    /// <summary>A vicious circle drawn as a loop of the steps in <see cref="ArticleBlock.Items"/> (for example, the anxiety loop).</summary>
    Cycle,

    /// <summary>Opens the chat in its trauma mode: work on what the event changed, without retelling it.</summary>
    TraumaChat,

    /// <summary>A task to try before the next day of a course.</summary>
    Task
}

public sealed record ArticleBlock(
    ArticleBlockKind Kind,
    string Text = "",
    IReadOnlyList<string>? Items = null,
    TechniqueId? Technique = null,
    string? ArticleId = null,
    int Threshold = 0,
    string? Verdict = null,
    string? VerdictLow = null,
    int Correct = -1,
    string? Explanation = null,
    string? TestId = null);

/// <param name="Id">Stable: the companion and other screens open an article by it.</param>
/// <param name="Icon">A Material icon name.</param>
/// <param name="Minutes">About how long it takes to read.</param>
public sealed record Article(string Id, string Title, string Summary, string Icon, int Minutes, IReadOnlyList<ArticleBlock> Blocks);

public static class ArticleIds
{
    /// <summary>How stress is felt in the body, what helps, and when a doctor is needed instead.</summary>
    public const string BodyAndStress = "body-stress";
    public const string Anxiety = "anxiety";
    public const string Panic = "panic";
    public const string Avoidance = "avoidance";
    public const string Thoughts = "thoughts";
    public const string Rumination = "rumination";
    public const string LowMood = "low-mood";
    public const string SelfCriticism = "self-criticism";
    public const string Sleep = "sleep";
    public const string Specialist = "specialist";
    public const string Burnout = "burnout";
    public const string Procrastination = "procrastination";
    public const string Perfectionism = "perfectionism";
    public const string Anger = "anger";
    public const string Boundaries = "boundaries";
    public const string Narcissism = "narcissism";
    public const string EmotionalAbuse = "emotional-abuse";
    public const string Grief = "grief";
    public const string SocialAnxiety = "social-anxiety";
    public const string HealthAnxiety = "health-anxiety";
    public const string Loneliness = "loneliness";
    public const string ShameAndGuilt = "shame-guilt";
    public const string Conflicts = "conflicts";
    public const string SupportInCrisis = "support-in-crisis";
    public const string TraumaStart = "trauma-start";
    public const string TraumaReaction = "trauma-reaction";
    public const string TraumaFlashbacks = "trauma-flashbacks";
    public const string TraumaNightmares = "trauma-nightmares";
    public const string TraumaTreatment = "trauma-treatment";
    public const string Ocd = "ocd";
    public const string Depression = "depression";
    public const string Numbing = "numbing";
    public const string ChronicPain = "chronic-pain";
    public const string ParentStress = "parent-stress";
    public const string HelpingLovedOne = "helping-loved-one";
    public const string EatingDisorders = "eating-disorders";
    public const string Bipolar = "bipolar";
    public const string Psychosis = "psychosis";
}
