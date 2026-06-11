using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class TestResultPage : ContentPage
{
    public TestResultPage(ITestResultViewModelFactory factory, TestResultInfo result)
    {
        InitializeComponent();
        BindingContext = factory.Create(Navigation, result);
    }
}
