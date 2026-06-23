using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.TestHistory;

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
