using MobileHelper.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class FindProblemPage : ContentPage
{
	public FindProblemPage(string? describtion, List<string> algorithm, string? comment, Action action)
	{
		InitializeComponent();

		this.BindingContext = new FindProblemViewModel(this.Navigation, describtion, algorithm, comment, action);
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		await Navigation.PopAsync(false);
    }
}