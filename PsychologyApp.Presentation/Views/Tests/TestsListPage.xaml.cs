using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class TestsListPage : ContentPage
{
    private TestsListViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;

    public TestsListPage(
        IPageViewModelActivator pageViewModelActivator,
        ITestsListViewModelFactory testsListViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => testsListViewModelFactory.Create(nav));
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
