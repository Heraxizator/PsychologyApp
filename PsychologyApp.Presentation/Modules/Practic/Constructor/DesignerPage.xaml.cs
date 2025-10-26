using PsychologyApp.Presentation.Modules.Practic.Constructor;

namespace MobileHelperMaui.Views.TechniquePages.ConstructorPages;

public partial class DesignerPage : ContentPage
{
    private DesignerViewModel ViewModel;
	public DesignerPage(long id)
	{
		InitializeComponent();

        ViewModel = new DesignerViewModel(this.Navigation, id);
        this.BindingContext = ViewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        ViewModel.ExecuteOperation(Navigation);
    }
}