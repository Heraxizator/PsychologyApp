using PsychologyApp.Presentation.Modules.Tester.Collection;

namespace MobileHelperMaui.Views.TestPages;

public partial class TestsListPage : ContentPage
{
	public TestsListPage()
	{
		InitializeComponent();

		TestsListViewModel vm = new(Navigation);

        this.BindingContext = vm;
	}
}