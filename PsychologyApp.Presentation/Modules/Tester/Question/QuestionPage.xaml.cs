using MobileHelper.ViewModels.TestViewModels;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Base.ServiceLocator;
using Microsoft.Maui.Controls;

namespace PsychologyApp.Presentation.Modules.Tester;

public partial class QuestionPage : ContentPage
{
    private QuestionViewModel ViewModel;
    public QuestionPage(List<Question> questions, Func<int, string> analyzer, bool singleAnswer)
    {
        InitializeComponent();

        var toastService = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IToastService>() 
            ?? throw new InvalidOperationException("IToastService not registered");
        var dialogService = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IDialogService>() 
            ?? throw new InvalidOperationException("IDialogService not registered");

        ViewModel = new QuestionViewModel(Navigation, questions, analyzer, singleAnswer, toastService, dialogService);

        BindingContext = ViewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync(false);
    }
}