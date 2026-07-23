using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Pages.ManageJournal.Journal;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;

public static class ManageJournalFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddManageJournalFeature(this IServiceCollection services)
    {
        services.AddFeatureSingleton<JournalMoodLoader>();
        services.AddFeatureViewModelFactory<IJournalViewModelFactory, JournalViewModelFactory>();
        services.AddFeatureSingleton<IJournalPageFactory, JournalPageFactory>();
        return services;
    }
}

public interface IJournalPageFactory
{
    JournalPage CreateJournalPage();
}

public sealed class JournalPageFactory(IJournalViewModelFactory journalViewModelFactory) : IJournalPageFactory
{
    public JournalPage CreateJournalPage() =>
        new(journalViewModelFactory);
}
