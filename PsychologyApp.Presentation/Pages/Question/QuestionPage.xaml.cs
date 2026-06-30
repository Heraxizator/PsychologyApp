using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionPage : ContentPage
{
    public QuestionPage(
        IQuestionViewModelFactory questionViewModelFactory,
        List<TestQuestion> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        TestSessionInfo? session = null)
    {
        BindingContext = questionViewModelFactory.Create(this, questions, analyzer, singleAnswer, session);
        if (BindingContext is QuestionViewModel viewModel)
        {
            viewModel.ValidationHintRequested += OnValidationHintRequested;
        }

        InitializeComponent();
    }

    private async void OnValidationHintRequested(object? sender, EventArgs e)
    {
        if (QuestionCard is not null)
        {
            await UiAnimations.SafeShakeAsync(QuestionCard);
        }
    }
}
