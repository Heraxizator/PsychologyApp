using PsychologyApp.Presentation.ViewModels.ProfileViewModels;

namespace PsychologyApp.Presentation.Views.ProfilePages;

public partial class OptionsPage : ContentPage
{
	public OptionsPage()
	{
		InitializeComponent();

		OptionsViewModel viewModel = new(this.Navigation);

		this.BindingContext = viewModel;
	}
}