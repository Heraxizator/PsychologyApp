using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class TestHistoryPage : ContentPage
{
    public TestHistoryPage(
        ITestHistoryViewModelFactory factory,
        string testId,
        string testTitle)
    {
        InitializeComponent();
        BindingContext = factory.Create(Navigation, testId, testTitle);
    }
}
