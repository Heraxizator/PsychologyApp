using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.DependencyInjection;

public static class PresentationServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IPageViewModelActivator, PageViewModelActivator>();
        services.AddSingleton<IProfilePageFactory, ProfilePageFactory>();
        services.AddSingleton<ITestPageFactory, TestPageFactory>();
        services.AddSingleton<ITechniquePageFactory, TechniquePageFactory>();
        services.AddSingleton<IPageFactory, MauiPageFactory>();
        services.AddSingleton<ITechniqueMessenger, TechniqueMessengerService>();
        services.AddSingleton<Func<INavigation, INavigationService>>(sp => navigation =>
            new MauiNavigationService(navigation, sp.GetRequiredService<IPageFactory>()));

        services.AddSingleton<ITechniquesViewModelFactory, TechniquesViewModelFactory>();
        services.AddSingleton<IUserViewModelFactory, UserViewModelFactory>();
        services.AddSingleton<IQuoteViewModelFactory, QuoteViewModelFactory>();
        services.AddSingleton<IPhysicsSearchViewModelFactory, PhysicsSearchViewModelFactory>();
        services.AddSingleton<ICreatedViewModelFactory, CreatedViewModelFactory>();
        services.AddSingleton<IDesignerViewModelFactory, DesignerViewModelFactory>();
        services.AddSingleton<ISettingsViewModelFactory, SettingsViewModelFactory>();
        services.AddSingleton<ITechniqueViewModelFactory, TechniqueViewModelFactory>();
        services.AddSingleton<ITestsListViewModelFactory, TestsListViewModelFactory>();
        services.AddSingleton<IStandardTestViewModelFactory, StandardTestViewModelFactory>();
        services.AddSingleton<IAlternativeTestViewModelFactory, AlternativeTestViewModelFactory>();
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
}
