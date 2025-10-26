using PsychologyApp.Presentation.Technique.Constructor;

namespace MobileHelperMaui.Views.TechniquePages.ConstructorPages;

public partial class CreatedPage : ContentPage
{
    public CreatedPage(long id)
    {
        InitializeComponent();
        this.BindingContext = new CreatedViewModel(this.Navigation, id);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}