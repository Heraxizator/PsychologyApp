using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageJournal.Journal;

public partial class JournalPage : ContentPage, IJournalHubPage
{
    private readonly JournalViewModel _viewModel;

    public JournalPage(IJournalViewModelFactory journalViewModelFactory)
    {
        InitializeComponent();
        _viewModel = journalViewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadAsync().FireAndForget();
    }

    protected override void OnDisappearing()
    {
        _viewModel.FlushPendingNoteSaveAsync().FireAndForget();
        base.OnDisappearing();
    }
}
