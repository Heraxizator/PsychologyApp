namespace PsychologyApp.Presentation.Common;

/// <summary>Copy of the Articles tab; the articles themselves are in PsychologyApp.Application/Articles.</summary>
public static partial class AppStrings
{
    public static string ArticlesLead => R(nameof(ArticlesLead));
    public static string ArticlesFilterAll => R(nameof(ArticlesFilterAll));
    public static string ArticleMinutes(object arg0) => F(nameof(ArticleMinutes), arg0);
    public static string ArticlePracticeCaption => R(nameof(ArticlePracticeCaption));
    public static string ArticleProgrammeCaption => R(nameof(ArticleProgrammeCaption));
    public static string ArticleTestsCaption => R(nameof(ArticleTestsCaption));
    public static string ArticleReadCaption => R(nameof(ArticleReadCaption));
    public static string ArticleChatCaption => R(nameof(ArticleChatCaption));
}
