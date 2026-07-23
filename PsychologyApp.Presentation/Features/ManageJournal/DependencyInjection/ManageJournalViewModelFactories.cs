using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Pages.ManageJournal.Journal;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;

public interface IJournalViewModelFactory
{
    JournalViewModel Create(ContentPage page);
}

public sealed class JournalViewModelFactory(
    JournalMoodLoader journalMoodLoader,
    IDialogService dialogService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IJournalViewModelFactory
{
    public JournalViewModel Create(ContentPage page) =>
        new(journalMoodLoader, dialogService, ResolveNavigation(navigationServiceFactory, page));
}
