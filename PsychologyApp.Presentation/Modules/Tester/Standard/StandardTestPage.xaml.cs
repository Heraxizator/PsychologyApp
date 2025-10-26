using PsychologyApp.Presentation.Modules.Tester.Standard;

namespace PsychologyApp.Presentation.Views.TestPages;

public partial class StandardTestPage : ContentPage
{
    public StandardTestPage()
    {
        InitializeComponent();

        BindingContext = new StandardTestViewModel(Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync(false);
    }
}