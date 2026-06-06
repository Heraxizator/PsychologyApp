using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class TestsListPage : ContentPage
{
    public TestsListPage(
        IPageViewModelActivator pageViewModelActivator,
        ITestsListViewModelFactory testsListViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => testsListViewModelFactory.Create(nav));
    }
}
