using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class TestsListPage : ContentPage
{
    private TestsListViewModel? _viewModel;

    public TestsListPage(
        IPageViewModelActivator pageViewModelActivator,
        ITestsListViewModelFactory testsListViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => testsListViewModelFactory.Create(nav));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.InitAsync().FireAndForget();
    }
}
