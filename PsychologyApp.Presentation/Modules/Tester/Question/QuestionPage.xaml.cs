using MobileHelper.ViewModels.TestViewModels;

namespace PsychologyApp.Presentation.Modules.Tester;

public partial class QuestionPage : ContentPage
{
    private QuestionViewModel ViewModel;
    public QuestionPage(List<Question> questions, Func<int, string> analyzer, bool singleAnswer)
    {
        InitializeComponent();

        ViewModel = new QuestionViewModel(Navigation, questions, analyzer, singleAnswer);

        BindingContext = ViewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync(false);
    }
}