using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Pages.ManageJournal.Journal;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;

public static class ManageJournalFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddManageJournalFeature(this IServiceCollection services)
    {
        services.AddFeatureSingleton<JournalMoodLoader>();
        services.AddFeatureSingleton<JournalEditorContext>();
        services.AddFeatureSingleton<JournalScreenCoordinator>();
        services.AddFeatureViewModelFactory<IJournalViewModelFactory, JournalViewModelFactory>();
        services.AddFeatureViewModelFactory<IJournalOverviewViewModelFactory, JournalOverviewViewModelFactory>();
        services.AddFeatureViewModelFactory<IJournalTimelineViewModelFactory, JournalTimelineViewModelFactory>();
        services.AddFeatureSingleton<IJournalPageFactory, JournalPageFactory>();
        return services;
    }
}

public interface IJournalPageFactory
{
    JournalPage CreateJournalPage();
    JournalOverviewPage CreateJournalOverviewPage();
    JournalTimelinePage CreateJournalTimelinePage();
}

public sealed class JournalPageFactory(
    IJournalViewModelFactory journalViewModelFactory,
    IJournalOverviewViewModelFactory journalOverviewViewModelFactory,
    IJournalTimelineViewModelFactory journalTimelineViewModelFactory) : IJournalPageFactory
{
    public JournalPage CreateJournalPage() =>
        new(journalViewModelFactory);

    public JournalOverviewPage CreateJournalOverviewPage() =>
        new(journalOverviewViewModelFactory);

    public JournalTimelinePage CreateJournalTimelinePage() =>
        new(journalTimelineViewModelFactory);
}
