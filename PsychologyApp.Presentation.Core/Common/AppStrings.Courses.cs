namespace PsychologyApp.Presentation.Common;

/// <summary>Copy of the courses, the reader tools and the "For you" picks of the Articles tab.</summary>
public static partial class AppStrings
{
    public static string ArticleChecklistHint => R(nameof(ArticleChecklistHint));
    public static string ArticleQuizCaption => R(nameof(ArticleQuizCaption));
    public static string ArticleCycleLoop => R(nameof(ArticleCycleLoop));
    public static string ArticleTaskCaption => R(nameof(ArticleTaskCaption));
    public static string ArticleSave => R(nameof(ArticleSave));
    public static string ArticleSaved => R(nameof(ArticleSaved));
    public static string ArticleListen => R(nameof(ArticleListen));
    public static string ArticleStopListening => R(nameof(ArticleStopListening));
    public static string ArticleReadMark => R(nameof(ArticleReadMark));
    public static string ArticleRight => R(nameof(ArticleRight));
    public static string ArticleNotQuite => R(nameof(ArticleNotQuite));
    public static string ArticlesSearchPlaceholder => R(nameof(ArticlesSearchPlaceholder));
    public static string ArticlesNothingFound => R(nameof(ArticlesNothingFound));
    public static string ArticlesFilterSaved => R(nameof(ArticlesFilterSaved));
    public static string ArticlesNothingSaved => R(nameof(ArticlesNothingSaved));
    public static string ArticlesForYou => R(nameof(ArticlesForYou));
    public static string ArticlesCourses => R(nameof(ArticlesCourses));
    public static string ArticlesCoursesLead => R(nameof(ArticlesCoursesLead));
    public static string ArticlesCourseMeta(object arg0, object arg1) => F(nameof(ArticlesCourseMeta), arg0, arg1);
    public static string PickContinueCourse => R(nameof(PickContinueCourse));
    public static string PickContinueReading => R(nameof(PickContinueReading));
    public static string PickTest(object arg0) => F(nameof(PickTest), arg0);
    public static string PickMood => R(nameof(PickMood));
    /// <summary>The reason on a card of "For you" before there is anything personal to pick from.</summary>
    public static string PickStarter => R(nameof(PickStarter));
    public static string CourseCaption => R(nameof(CourseCaption));
    public static string CourseStart => R(nameof(CourseStart));
    public static string CourseContinue(object arg0) => F(nameof(CourseContinue), arg0);
    public static string CourseFinished => R(nameof(CourseFinished));
    public static string CourseDayLabel(object arg0) => F(nameof(CourseDayLabel), arg0);
    public static string CourseDayTomorrow => R(nameof(CourseDayTomorrow));
    public static string CourseDayLocked => R(nameof(CourseDayLocked));
    public static string CourseDayDone => R(nameof(CourseDayDone));
    public static string CourseProgress(object arg0, object arg1) => F(nameof(CourseProgress), arg0, arg1);
    public static string CourseMeasureTitle => R(nameof(CourseMeasureTitle));
    public static string CourseMeasureHint(object arg0) => F(nameof(CourseMeasureHint), arg0);
    public static string CourseMeasureOnce(object arg0, object arg1) => F(nameof(CourseMeasureOnce), arg0, arg1);
    public static string CourseMeasureChange(object arg0, object arg1, object arg2) => F(nameof(CourseMeasureChange), arg0, arg1, arg2);
    public static string CourseTakeTest(object arg0) => F(nameof(CourseTakeTest), arg0);
    public static string CourseReset => R(nameof(CourseReset));
    public static string CourseResetConfirm => R(nameof(CourseResetConfirm));
    public static string CourseReflectionPlaceholder => R(nameof(CourseReflectionPlaceholder));
    public static string CourseCompleteDay => R(nameof(CourseCompleteDay));
    public static string CourseDayClosed(object arg0) => F(nameof(CourseDayClosed), arg0);
    public static string CourseAllDone => R(nameof(CourseAllDone));
    public static string CourseDayNotOpen => R(nameof(CourseDayNotOpen));
}
