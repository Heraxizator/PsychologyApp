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
        InitializeComponent();
        BindingContext = findProblemViewModelFactory.Create(Navigation, description, algorithm, comment, startTest, testId);
    }
}
