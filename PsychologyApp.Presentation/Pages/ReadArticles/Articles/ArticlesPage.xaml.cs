using PsychologyApp.Presentation.Features.ReadArticles;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Articles;

public partial class ArticlesPage : ContentPage
{
    private readonly ArticlesViewModel _viewModel;

    public ArticlesPage(IArticlesViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    // "For you", the read marks and the course progress follow what happened since the tab was last seen.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
