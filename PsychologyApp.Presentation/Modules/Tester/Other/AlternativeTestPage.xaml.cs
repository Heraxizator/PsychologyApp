using MobileHelper.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class AlternativeTestPage : ContentPage
{
    public AlternativeTestPage()
    {
        InitializeComponent();

        BindingContext = new AlternativeTestViewModel(Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync(false);
    }
}