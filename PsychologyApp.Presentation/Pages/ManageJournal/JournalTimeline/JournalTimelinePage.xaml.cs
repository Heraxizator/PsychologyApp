using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;

public partial class JournalTimelinePage : ContentPage
{
    private readonly JournalTimelineViewModel _viewModel;

    public JournalTimelinePage(IJournalTimelineViewModelFactory journalTimelineViewModelFactory)
    {
        InitializeComponent();
        _viewModel = journalTimelineViewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadAsync().FireAndForget();
    }
}
