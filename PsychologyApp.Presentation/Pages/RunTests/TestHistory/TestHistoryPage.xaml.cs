using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTests.TestHistory;

public partial class TestHistoryPage : ContentPage
{
    public TestHistoryPage(
        ITestHistoryViewModelFactory factory,
        string testId,
        string testTitle)
    {
        InitializeComponent();
        BindingContext = factory.Create(this, testId, testTitle);
    }
}
