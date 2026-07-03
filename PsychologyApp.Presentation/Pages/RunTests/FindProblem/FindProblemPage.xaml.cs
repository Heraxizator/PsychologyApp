using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTests.FindProblem;

public partial class FindProblemPage : ContentPage
{
    public FindProblemPage(
        IFindProblemViewModelFactory findProblemViewModelFactory,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null)
    {
        BindingContext = findProblemViewModelFactory.Create(this, description, algorithm, comment, startTest, testId);
        InitializeComponent();
    }
}
