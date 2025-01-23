using MobileHelper.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.Technique.Comparison;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ResizePage : ContentPage
{
    public ResizePage()
    {
        InitializeComponent();

        this.BindingContext = new ResizeViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}