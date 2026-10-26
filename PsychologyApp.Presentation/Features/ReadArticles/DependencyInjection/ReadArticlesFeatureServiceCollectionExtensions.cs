using PsychologyApp.Presentation.App.DependencyInjection;

namespace PsychologyApp.Presentation.Features.ReadArticles.DependencyInjection;

public static class ReadArticlesFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddReadArticlesFeature(this IServiceCollection services)
    {
        services.AddFeatureSingleton<IArticlesViewModelFactory, ArticlesViewModelFactory>();
        services.AddFeatureSingleton<IForYouViewModelFactory, ForYouViewModelFactory>();
        services.AddFeatureSingleton<ICoursesViewModelFactory, CoursesViewModelFactory>();
        services.AddFeatureSingleton<IArticleReaderViewModelFactory, ArticleReaderViewModelFactory>();
        services.AddFeatureSingleton<ICourseViewModelFactory, CourseViewModelFactory>();
        services.AddFeatureSingleton<ICourseDayViewModelFactory, CourseDayViewModelFactory>();
        services.AddFeatureSingleton<IAnchorThoughtsViewModelFactory, AnchorThoughtsViewModelFactory>();
        services.AddFeatureSingleton<IReadArticlesPageFactory, ReadArticlesPageFactory>();
        return services;
    }
}
