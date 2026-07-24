using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Pages.ManageJournal.Journal;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;

public interface IJournalViewModelFactory
{
    JournalViewModel Create(ContentPage page);
}

public sealed class JournalViewModelFactory(
    JournalMoodLoader journalMoodLoader,
    JournalEditorContext editorContext,
    IDialogService dialogService,
    IUserPreferencesStore preferencesStore,
    IMoodReminderCoordinator moodReminderCoordinator,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IJournalViewModelFactory
{
    public JournalViewModel Create(ContentPage page) =>
        new(
            journalMoodLoader,
            editorContext,
            dialogService,
            preferencesStore,
            moodReminderCoordinator,
            ResolveNavigation(navigationServiceFactory, page));
}

public interface IJournalOverviewViewModelFactory
{
    JournalOverviewViewModel Create(ContentPage page);
}

public sealed class JournalOverviewViewModelFactory(
    JournalMoodLoader journalMoodLoader,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IJournalOverviewViewModelFactory
{
    public JournalOverviewViewModel Create(ContentPage page) =>
        new(journalMoodLoader, ResolveNavigation(navigationServiceFactory, page));
}

public interface IJournalTimelineViewModelFactory
{
    JournalTimelineViewModel Create(ContentPage page);
}

public sealed class JournalTimelineViewModelFactory(
    JournalMoodLoader journalMoodLoader,
    JournalEditorContext editorContext,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IJournalTimelineViewModelFactory
{
    public JournalTimelineViewModel Create(ContentPage page) =>
        new(journalMoodLoader, editorContext, ResolveNavigation(navigationServiceFactory, page));
}
