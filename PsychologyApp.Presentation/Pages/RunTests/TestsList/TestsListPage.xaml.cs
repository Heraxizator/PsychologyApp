using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;

namespace PsychologyApp.Presentation.Pages.RunTests.TestsList;

public partial class TestsListPage : ContentPage
{
    private TestsListViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;

    public TestsListPage(
        IPageViewModelActivator pageViewModelActivator,
        ITestsListViewModelFactory testsListViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => testsListViewModelFactory.Create(page));
        _animationHelper = new PageAnimationHelper(_viewModel, LoadingProgress, TestsCollectionView);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
        _viewModel?.InitAsync().FireAndForget();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
