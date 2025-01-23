using MobileHelper.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.Technique.Comparison;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class CheckPage : ContentPage
{
    public CheckPage()
    {
        InitializeComponent();

        this.BindingContext = new CheckViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}