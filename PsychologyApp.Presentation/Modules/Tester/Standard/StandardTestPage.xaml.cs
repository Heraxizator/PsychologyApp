using PsychologyApp.Presentation.ViewModels.TestViewModels;

namespace PsychologyApp.Presentation.Views.TestPages;

public partial class StandardTestPage : ContentPage
{
	public StandardTestPage()
	{
		InitializeComponent();

		this.BindingContext = new StandardTestViewModel(this.Navigation);
	}
}