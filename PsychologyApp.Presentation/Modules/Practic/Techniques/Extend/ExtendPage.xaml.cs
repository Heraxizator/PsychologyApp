using MobileHelper.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.Technique.Comparison;
using PsychologyApp.Presentation.Technique.Extend;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ExtendPage : ContentPage
{
    public ExtendPage()
    {
        InitializeComponent();

        this.BindingContext = new ExtendViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}