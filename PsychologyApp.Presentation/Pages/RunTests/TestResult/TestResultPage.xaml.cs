using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultPage : ContentPage
{
    public TestResultPage(ITestResultViewModelFactory factory, TestResultInfo result)
    {
        InitializeComponent();
        BindingContext = factory.Create(this, result);
    }
}
