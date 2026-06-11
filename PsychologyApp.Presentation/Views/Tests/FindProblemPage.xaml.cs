using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class FindProblemPage : ContentPage
{
    public FindProblemPage(
        IFindProblemViewModelFactory findProblemViewModelFactory,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action,
        string? testId = null)
    {
        InitializeComponent();
        BindingContext = findProblemViewModelFactory.Create(Navigation, description, algorithm, comment, action, testId);
    }
}
