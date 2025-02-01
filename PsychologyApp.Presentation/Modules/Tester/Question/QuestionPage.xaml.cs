using MobileHelper.ViewModels.TestViewModels;

namespace PsychologyApp.Presentation.Modules.Tester;

public partial class QuestionPage : ContentPage
{
    public QuestionPage(List<Question> questions, Func<int, string> analyzer)
    {
        InitializeComponent();

        BindingContext = new QuestionViewModel(Navigation, questions, analyzer);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }
}