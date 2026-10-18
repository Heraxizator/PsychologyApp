using PsychologyApp.Presentation.Features.ReadArticles;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.ReadArticles.ForYou;

public partial class ForYouPage : ContentPage
{
    private readonly ForYouViewModel _viewModel;

    public ForYouPage(IForYouViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    // A course day closed, an article read or a test taken since the tab was last seen changes the picks.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
