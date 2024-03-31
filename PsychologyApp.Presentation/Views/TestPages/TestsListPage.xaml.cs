using PsychologyApp.Presentation.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class TestsListPage : ContentPage
{
	public TestsListPage()
	{
		InitializeComponent();

        TestsListViewModel vm = new()
        {
            Navigation = this.Navigation
        };

        this.BindingContext = vm;
	}
}