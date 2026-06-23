using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.TestsList;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionPage : ContentPage
{
    public QuestionPage(
        IQuestionViewModelFactory questionViewModelFactory,
        List<Models.Tests.Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        InitializeComponent();
        BindingContext = questionViewModelFactory.Create(Navigation, questions, analyzer, singleAnswer, session);
    }
}
