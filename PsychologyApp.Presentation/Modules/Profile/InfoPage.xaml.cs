namespace MobileHelperMaui.Views.AboutPages;

public partial class InfoPage : ContentPage
{
	public InfoPage()
	{
		InitializeComponent();
	}

	private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		await Navigation.PopAsync(false);

	}
}