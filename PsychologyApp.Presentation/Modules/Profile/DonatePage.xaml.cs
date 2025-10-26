namespace PsychologyApp.Presentation.Modules.Profile;

public partial class DonatePage : ContentPage
{
    public DonatePage()
    {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        _ = await Browser.Default.OpenAsync("https://yoomoney.ru/fundraise/17UP5E1QFCU.250123");
    }
}