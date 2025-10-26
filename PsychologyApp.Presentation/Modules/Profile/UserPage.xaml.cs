using MobileHelper.ViewModels.ProfileViewModels;

namespace MobileHelperMaui.Views.ProfilePages;

public partial class UserPage : ContentPage
{
    public UserPage()
    {
        InitializeComponent();

        BindingContext = new UserViewModel(Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }
}