using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;

public partial class JournalOverviewPage : ContentPage
{
    private readonly JournalOverviewViewModel _viewModel;

    public JournalOverviewPage(IJournalOverviewViewModelFactory journalOverviewViewModelFactory)
    {
        InitializeComponent();
        _viewModel = journalOverviewViewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadAsync().FireAndForget();
    }
}
