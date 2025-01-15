using MobileHelper.ViewModels.ProfileViewModels;

namespace MobileHelperMaui.Views.ProfilePages;

public partial class UserPage : ContentPage
{
	public UserPage()
	{
		InitializeComponent();

        this.BindingContext = new UserViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}