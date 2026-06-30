using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.App.Providers;

public static class RunTestsFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddRunTestsFeature(this IServiceCollection services)
    {
        services.AddSingleton<ITestPageFactory, TestPageFactory>();
        services.AddSingleton<TestRunCoordinator>();
        services.AddSingleton<TestRetakeOperations>();
        services.AddSingleton<QuestionnaireDetailBuilder>();
        services.AddSingleton<QuestionnaireDetailReader>();
        services.AddSingleton<TestTrendResolver>();
        services.AddSingleton<TestHistoryLoader>();
        services.AddSingleton<TestsListLoader>();
        services.AddSingleton<QuestionnaireSubmissionService>();
        services.AddSingleton<FindProblemContentLoader>();
        services.AddSingleton<ITestsListViewModelFactory, TestsListViewModelFactory>();
        services.AddSingleton<ITestHistoryViewModelFactory, TestHistoryViewModelFactory>();
        services.AddSingleton<ILuscherTestViewModelFactory, LuscherTestViewModelFactory>();
        services.AddSingleton<ITestResultViewModelFactory, TestResultViewModelFactory>();
        services.AddSingleton<IQuestionViewModelFactory, QuestionViewModelFactory>();
        services.AddSingleton<IFindProblemViewModelFactory, FindProblemViewModelFactory>();
        return services;
    }
}
