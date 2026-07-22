using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileJournal;

public partial class JournalPage : ContentPage
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
}
