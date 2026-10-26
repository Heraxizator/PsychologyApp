using PsychologyApp.Application.Articles;

namespace PsychologyApp.Application.AnchorThoughts;

/// <summary>What a thought is about; the page offers one theme at a time or all of them.</summary>
public enum AnchorTheme
{
    /// <summary>Other people's words, judgements and how they treat me.</summary>
    OthersWords,

    /// <summary>Mistakes, self-criticism and the demand to be perfect.</summary>
    Mistakes,

    /// <summary>Thoughts, feelings, anxiety.</summary>
    ThoughtsAndFeelings,

    /// <summary>Boundaries, needs and close relationships.</summary>
    Relationships,

    /// <summary>Low energy, loss, a long hard stretch.</summary>
    HardTimes
}

/// <param name="Id">The same in every language: the languages are matched by it (tested).</param>
/// <param name="Text">The thought itself, short enough to remember in a hard minute.</param>
/// <param name="Why">Why it holds, so the thought can be checked against experience instead of taken as a rule.</param>
/// <param name="ArticleId">The article that explains it more fully (<see cref="ArticleIds"/>).</param>
public sealed record AnchorThought(string Id, AnchorTheme Theme, string Text, string Why, string ArticleId);

/// <summary>
/// Short thoughts to lean on: about one's worth, mistakes, other people's words, feelings and hard times. Each speaks about an action
/// or about oneself, never labels the other person, and says why it holds. Clinical copy: reviewed by a psychologist before release.
/// </summary>
public static partial class AnchorThoughtContent
{
    /// <summary>The theme and the article of each thought, in the order they are shown; one place for every language.</summary>
    private static readonly (string Id, AnchorTheme Theme, string ArticleId)[] Entries =
    [
        ("humiliation", AnchorTheme.OthersWords, ArticleIds.Boundaries),
        ("others-words", AnchorTheme.OthersWords, ArticleIds.SelfEsteem),
        ("criticism", AnchorTheme.OthersWords, ArticleIds.SelfEsteem),
        ("how-treated", AnchorTheme.OthersWords, ArticleIds.EmotionalAbuse),
        ("comparison", AnchorTheme.OthersWords, ArticleIds.SelfEsteem),
        ("not-everyone", AnchorTheme.OthersWords, ArticleIds.SocialAnxiety),
        ("mind-reading", AnchorTheme.OthersWords, ArticleIds.SocialAnxiety),
        ("refusal", AnchorTheme.OthersWords, ArticleIds.Loneliness),
        ("silence", AnchorTheme.OthersWords, ArticleIds.Loneliness),
        ("not-my-shame", AnchorTheme.OthersWords, ArticleIds.ShameAndGuilt),
        ("loud-not-true", AnchorTheme.OthersWords, ArticleIds.Boundaries),

        ("mistake", AnchorTheme.Mistakes, ArticleIds.ShameAndGuilt),
        ("kind-voice", AnchorTheme.Mistakes, ArticleIds.SelfCriticism),
        ("good-enough", AnchorTheme.Mistakes, ArticleIds.Perfectionism),
        ("learning", AnchorTheme.Mistakes, ArticleIds.Perfectionism),
        ("past-decisions", AnchorTheme.Mistakes, ArticleIds.ShameAndGuilt),
        ("worth-not-output", AnchorTheme.Mistakes, ArticleIds.Burnout),
        ("repair-is-enough", AnchorTheme.Mistakes, ArticleIds.ShameAndGuilt),
        ("start-imperfect", AnchorTheme.Mistakes, ArticleIds.Procrastination),
        ("failure-is-information", AnchorTheme.Mistakes, ArticleIds.Perfectionism),

        ("thought-not-fact", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Thoughts),
        ("feeling-wave", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Emotions),
        ("any-feeling", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Emotions),
        ("anxiety-signal", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Anxiety),
        ("panic-passes", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Panic),
        ("not-right-now", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Rumination),
        ("worry-not-safety", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Rumination),
        ("small-steps-to-fear", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Avoidance),
        ("calm-body-first", AnchorTheme.ThoughtsAndFeelings, ArticleIds.BodyAndStress),
        ("uncertainty-bearable", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Uncertainty),
        ("let-thoughts-pass", AnchorTheme.ThoughtsAndFeelings, ArticleIds.Mindfulness),

        ("saying-no", AnchorTheme.Relationships, ArticleIds.Boundaries),
        ("my-needs", AnchorTheme.Relationships, ArticleIds.Boundaries),
        ("others-feelings", AnchorTheme.Relationships, ArticleIds.Conflicts),
        ("say-what-i-need", AnchorTheme.Relationships, ArticleIds.Conflicts),
        ("repair", AnchorTheme.Relationships, ArticleIds.Conflicts),
        ("loneliness-feeling", AnchorTheme.Relationships, ArticleIds.Loneliness),
        ("envy-shows-wants", AnchorTheme.Relationships, ArticleIds.JealousyAndEnvy),
        ("breakup-not-worth", AnchorTheme.Relationships, ArticleIds.Breakup),
        ("different-not-wrong", AnchorTheme.Relationships, ArticleIds.Conflicts),

        ("small-step", AnchorTheme.HardTimes, ArticleIds.LowMood),
        ("one-day", AnchorTheme.HardTimes, ArticleIds.LowMood),
        ("rest", AnchorTheme.HardTimes, ArticleIds.Burnout),
        ("not-a-straight-line", AnchorTheme.HardTimes, ArticleIds.Depression),
        ("grief-love", AnchorTheme.HardTimes, ArticleIds.Grief),
        ("reasonable-decision", AnchorTheme.HardTimes, ArticleIds.Uncertainty),
        ("basic-care", AnchorTheme.HardTimes, ArticleIds.LowMood),
        ("small-hope", AnchorTheme.HardTimes, ArticleIds.Depression),
        ("not-forever", AnchorTheme.HardTimes, ArticleIds.Depression),
        ("asking-for-help", AnchorTheme.HardTimes, ArticleIds.Specialist)
    ];

    /// <summary>The thoughts in a language by its key ("ru", "en", "fr"…); a language without its own gets the English ones.</summary>
    public static IReadOnlyList<AnchorThought> All(string? language)
    {
        IReadOnlyDictionary<string, (string Text, string Why)> texts = language?.ToLowerInvariant() switch
        {
            "ru" => Ru,
            "fr" => Fr,
            "de" => De,
            "es" => Es,
            "pt" => Pt,
            "zh" => Zh,
            _ => En
        };

        return [.. Entries.Select(entry => new AnchorThought(entry.Id, entry.Theme, texts[entry.Id].Text, texts[entry.Id].Why, entry.ArticleId))];
    }
}
