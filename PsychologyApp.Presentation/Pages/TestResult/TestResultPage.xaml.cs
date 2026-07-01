using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.TestResult;

public partial class TestResultPage : ContentPage
{
    public TestResultPage(ITestResultViewModelFactory factory, TestResultInfo result)
    {
        InitializeComponent();
        BindingContext = factory.Create(this, result);
    }
}
