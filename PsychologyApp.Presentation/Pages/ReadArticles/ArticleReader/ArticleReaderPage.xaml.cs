using PsychologyApp.Presentation.Features.ReadArticles;

namespace PsychologyApp.Presentation.Pages.ReadArticles.ArticleReader;

public partial class ArticleReaderPage : ContentPage
{
    // "Read" when the last lines are on screen, not on opening: an article glanced at is not an article read.
    private const double EndSlack = 48;

    private readonly ArticleReaderViewModel _viewModel;

    public ArticleReaderPage(IArticleReaderViewModelFactory viewModelFactory, string articleId)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this, articleId);
        BindingContext = _viewModel;
        foreach (IView child in Tools.Children)
        {
            FlexLayout.SetShrink((BindableObject)child, 0);
        }

        Scroller.Scrolled += (_, _) => CheckEnd();
        Scroller.SizeChanged += (_, _) => CheckEnd();
    }

    private void CheckEnd()
    {
        if (Scroller.ContentSize.Height > 0 && Scroller.ScrollY + Scroller.Height >= Scroller.ContentSize.Height - EndSlack)
        {
            _viewModel.ReachedEnd();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopListening();
    }
}
