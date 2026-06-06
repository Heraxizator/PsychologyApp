using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class QuestionPage : ContentPage
{
    public QuestionPage(
        IQuestionViewModelFactory questionViewModelFactory,
        List<Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer)
    {
        InitializeComponent();
        BindingContext = questionViewModelFactory.Create(Navigation, questions, analyzer, singleAnswer);
    }
}
