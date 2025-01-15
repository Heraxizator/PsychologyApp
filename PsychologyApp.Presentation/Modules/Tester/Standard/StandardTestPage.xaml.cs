using PsychologyApp.Presentation.ViewModels.TestViewModels;

namespace PsychologyApp.Presentation.Views.TestPages;

public partial class StandardTestPage : ContentPage
{
	public StandardTestPage()
	{
		InitializeComponent();

		this.BindingContext = new StandardTestViewModel(this.Navigation);
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		await Navigation.PopAsync(false);
    }
}