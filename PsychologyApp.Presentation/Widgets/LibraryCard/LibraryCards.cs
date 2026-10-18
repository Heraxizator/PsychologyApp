using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Widgets.LibraryCard;

/// <summary>Cards for articles and courses, the same on every Library page: read and saved marks for articles, progress for courses.</summary>
public static class LibraryCards
{
    public static LibraryCardItem Article(Article article, ReadingState state, INavigationService navigation, string reason = "") => new()
    {
        Title = article.Title,
        Summary = article.Summary,
        Icon = article.Icon,
        MetaText = AppStrings.ArticleMinutes(article.Minutes),
        Reason = reason,
        IsRead = state.Read.Contains(article.Id),
        IsSaved = state.Saved.Contains(article.Id),
        OpenCommand = new AsyncCommand(() => navigation.GoToArticleAsync(article.Id))
    };

    public static LibraryCardItem Course(Course course, ReadingState state, INavigationService navigation, string reason = "")
    {
        CourseProgress? progress = state.Course(course.Id);
        return new LibraryCardItem
        {
            Title = course.Title,
            Summary = course.Summary,
            Icon = course.Icon,
            MetaText = progress is null
                ? AppStrings.ArticlesCourseMeta(course.Days.Count, course.Days.Max(day => day.Minutes))
                : progress.FinishedUtc is not null ? AppStrings.CourseFinished : AppStrings.CourseProgress(progress.Days.Count, course.Days.Count),
            Reason = reason,
            IsRead = progress?.FinishedUtc is not null,
            OpenCommand = new AsyncCommand(() => navigation.GoToCourseAsync(course.Id))
        };
    }
}
