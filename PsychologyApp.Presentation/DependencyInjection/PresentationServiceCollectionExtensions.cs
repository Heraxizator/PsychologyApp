using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services.Physics;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Services.Shell;
using PsychologyApp.Presentation.Services.Clean;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.DependencyInjection;

public static class PresentationServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IPageHost, MauiPageHost>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<GlobalExceptionHandler>();
        services.AddSingleton<IPageViewModelActivator, PageViewModelActivator>();
        services.AddSingleton<IProfilePageFactory, ProfilePageFactory>();
        services.AddSingleton<ITestPageFactory, TestPageFactory>();
        services.AddSingleton<ITechniquePageFactory, TechniquePageFactory>();
        services.AddSingleton<IPageFactory, MauiPageFactory>();
        services.AddSingleton<IShellStartupCoordinator, ShellStartupCoordinator>();
        services.AddSingleton<ITechniqueMessenger, TechniqueMessengerService>();
        services.AddSingleton<PhysicsSearchCoordinator>();
        AddTransientFactory<PhysicsSearchSession>(services);
        services.AddSingleton<TechniqueListBuilder>();
        services.AddSingleton<DesignerTechniqueOperations>();
        services.AddSingleton<TechniqueSessionCompletionService>();
        AddTransientFactory<EntryDraftCoordinator>(services);
        AddTransientFactory<PaperListDraftCoordinator>(services);
        AddTransientFactory<PolarityListDraftCoordinator>(services);
        services.AddSingleton<CustomTechniqueSessionOperations>();
        services.AddSingleton<TestRetakeOperations>();
        services.AddSingleton<TestHistoryLoader>();
        services.AddSingleton<TestsListLoader>();
        services.AddSingleton<QuestionnaireSubmissionService>();
        services.AddSingleton<LuscherTestSubmissionService>();
        services.AddSingleton<FindProblemContentLoader>();
        services.AddSingleton<ProfileStatsLoader>();
        services.AddSingleton<ProfileQuotesPresenter>();
        AddTransientFactory<ProfileQuotesLoader>(services);
        services.AddSingleton<SettingsPreferencesPresenter>();
        services.AddSingleton<PracticeDashboardLoader>();
        services.AddSingleton<TechniquesListInitializer>();
        services.AddSingleton<ProfilePracticeHistoryLoader>();
        services.AddSingleton<ProfileFeaturedTechniquesBuilder>();
        services.AddSingleton<UserProfileRefreshCoordinator>();
        AddTransientFactory<QuoteFeedCoordinator>(services);
        services.AddSingleton<QuoteFeedLoader>();
        services.AddSingleton<QuoteItemCommandsFactory>();
        services.AddSingleton<MusicPlaylistPresenter>();
        services.AddSingleton<MusicPlaybackPresenter>();
        services.AddSingleton<IUserPreferencesStore, MauiUserPreferencesStore>();
        services.AddSingleton<IDatabaseReadySignal, DatabaseReadySignal>();
        services.AddSingleton<IQuotesChangeNotifier, QuotesChangeNotifier>();
        services.AddSingleton<Func<NavigationContext, INavigationService>>(sp => context =>
            context.NavigationService ?? new MauiNavigationService(
                context.Navigation,
                sp.GetRequiredService<IPageFactory>(),
                sp.GetRequiredService<IShellStartupCoordinator>()));

        services.AddSingleton<ITechniquesViewModelFactory, TechniquesViewModelFactory>();
        services.AddSingleton<IUserViewModelFactory, UserViewModelFactory>();
        services.AddSingleton<IQuoteViewModelFactory, QuoteViewModelFactory>();
        services.AddSingleton<IPhysicsSearchViewModelFactory, PhysicsSearchViewModelFactory>();
        services.AddSingleton<ICreatedViewModelFactory, CreatedViewModelFactory>();
        services.AddSingleton<IDesignerViewModelFactory, DesignerViewModelFactory>();
        services.AddSingleton<ISettingsViewModelFactory, SettingsViewModelFactory>();
        services.AddSingleton<ITechniqueViewModelFactory, TechniqueViewModelFactory>();
        services.AddSingleton<ITestsListViewModelFactory, TestsListViewModelFactory>();
        services.AddSingleton<ITestHistoryViewModelFactory, TestHistoryViewModelFactory>();
        services.AddSingleton<ILuscherTestViewModelFactory, LuscherTestViewModelFactory>();
        services.AddSingleton<ITestResultViewModelFactory, TestResultViewModelFactory>();
        services.AddSingleton<IQuestionViewModelFactory, QuestionViewModelFactory>();
        services.AddSingleton<IFindProblemViewModelFactory, FindProblemViewModelFactory>();
        services.AddSingleton<ITheoryViewModelFactory, TheoryViewModelFactory>();
        services.AddSingleton<IStartPhysicsViewModelFactory, StartPhysicsViewModelFactory>();
        services.AddSingleton<IOptionsViewModelFactory, OptionsViewModelFactory>();
        services.AddSingleton<IInfoViewModelFactory, InfoViewModelFactory>();
        services.AddSingleton<IDonateViewModelFactory, DonateViewModelFactory>();
        services.AddSingleton<IFormViewModelFactory, FormViewModelFactory>();
        services.AddSingleton<IMusicPlayerViewModelFactory, MusicPlayerViewModelFactory>();
        services.AddSingleton<IOnboardingViewModelFactory, OnboardingViewModelFactory>();

        return services;
    }

    private static void AddTransientFactory<T>(IServiceCollection services) where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(sp => () => sp.GetRequiredService<T>());
    }
}

