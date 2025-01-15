using MobileHelper.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class AlternativeTestPage : ContentPage
{
	public AlternativeTestPage()
	{
		InitializeComponent();

		this.BindingContext = new AlternativeTestViewModel(this.Navigation);
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		await Navigation.PopAsync(false);
    }
}