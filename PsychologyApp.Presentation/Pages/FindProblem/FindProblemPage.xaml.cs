using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.FindProblem;

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
