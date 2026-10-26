using PsychologyApp.Application.Articles;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.ReadArticles.AnchorThoughts;
using PsychologyApp.Presentation.Pages.ReadArticles.ArticleReader;
using PsychologyApp.Presentation.Pages.ReadArticles.Articles;
using PsychologyApp.Presentation.Pages.ReadArticles.Courses;
using PsychologyApp.Presentation.Pages.ReadArticles.ForYou;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.ReadArticles;

public interface IArticlesViewModelFactory
{
    ArticlesViewModel Create(ContentPage page);
}

public interface IForYouViewModelFactory
{
    ForYouViewModel Create(ContentPage page);
}

public interface ICoursesViewModelFactory
{
    CoursesViewModel Create(ContentPage page);
}

public interface IArticleReaderViewModelFactory
{
    ArticleReaderViewModel Create(ContentPage page, string articleId);
}

public interface ICourseViewModelFactory
{
    CourseViewModel Create(ContentPage page, string courseId);
}

public interface ICourseDayViewModelFactory
{
    CourseDayViewModel Create(ContentPage page, string courseId, int day);
}

public interface IAnchorThoughtsViewModelFactory
{
    AnchorThoughtsViewModel Create(ContentPage page);
}

public interface IReadArticlesPageFactory
{
    ArticlesPage CreateArticlesPage();
    ForYouPage CreateForYouPage();
    CoursesPage CreateCoursesPage();
    ArticleReaderPage CreateArticleReaderPage(string articleId);
    CoursePage CreateCoursePage(string courseId);
    CourseDayPage CreateCourseDayPage(string courseId, int day);
    AnchorThoughtsPage CreateAnchorThoughtsPage();
}

public sealed class ArticlesViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, IArticlesViewModelFactory
{
    public ArticlesViewModel Create(ContentPage page) => new(ResolveNavigation(navigationServiceFactory, page), reading, databaseReady);
}

public sealed class ForYouViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IUserProgressService progress,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, IForYouViewModelFactory
{
    public ForYouViewModel Create(ContentPage page) => new(ResolveNavigation(navigationServiceFactory, page), reading, progress, databaseReady);
}

public sealed class CoursesViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, ICoursesViewModelFactory
{
    public CoursesViewModel Create(ContentPage page) => new(ResolveNavigation(navigationServiceFactory, page), reading, databaseReady);
}

public sealed class ArticleReaderViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, IArticleReaderViewModelFactory
{
    public ArticleReaderViewModel Create(ContentPage page, string articleId) =>
        new(ResolveNavigation(navigationServiceFactory, page), reading, databaseReady, articleId);
}

public sealed class CourseViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IUserProgressService progress,
    IDialogService dialogs,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, ICourseViewModelFactory
{
    public CourseViewModel Create(ContentPage page, string courseId) =>
        new(ResolveNavigation(navigationServiceFactory, page), reading, progress, dialogs, databaseReady, courseId);
}

public sealed class CourseDayViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IReadingService reading,
    IDialogService dialogs,
    IDatabaseReadySignal databaseReady)
    : ViewModelFactoryBase, ICourseDayViewModelFactory
{
    public CourseDayViewModel Create(ContentPage page, string courseId, int day) =>
        new(ResolveNavigation(navigationServiceFactory, page), reading, dialogs, databaseReady, courseId, day);
}

public sealed class AnchorThoughtsViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory)
    : ViewModelFactoryBase, IAnchorThoughtsViewModelFactory
{
    public AnchorThoughtsViewModel Create(ContentPage page) => new(ResolveNavigation(navigationServiceFactory, page));
}

public sealed class ReadArticlesPageFactory(
    IArticlesViewModelFactory articlesViewModelFactory,
    IForYouViewModelFactory forYouViewModelFactory,
    ICoursesViewModelFactory coursesViewModelFactory,
    IArticleReaderViewModelFactory articleReaderViewModelFactory,
    ICourseViewModelFactory courseViewModelFactory,
    ICourseDayViewModelFactory courseDayViewModelFactory,
    IAnchorThoughtsViewModelFactory anchorThoughtsViewModelFactory) : IReadArticlesPageFactory
{
    public ArticlesPage CreateArticlesPage() => new(articlesViewModelFactory);

    public ForYouPage CreateForYouPage() => new(forYouViewModelFactory);

    public CoursesPage CreateCoursesPage() => new(coursesViewModelFactory);

    public ArticleReaderPage CreateArticleReaderPage(string articleId) => new(articleReaderViewModelFactory, articleId);

    public CoursePage CreateCoursePage(string courseId) => new(courseViewModelFactory, courseId);

    public CourseDayPage CreateCourseDayPage(string courseId, int day) => new(courseDayViewModelFactory, courseId, day);

    public AnchorThoughtsPage CreateAnchorThoughtsPage() => new(anchorThoughtsViewModelFactory);
}
