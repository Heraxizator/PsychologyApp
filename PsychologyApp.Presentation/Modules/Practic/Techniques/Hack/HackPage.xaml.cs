using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class HackPage : ContentPage
{
    public HackPage()
    {
        InitializeComponent();

        this.BindingContext = new HackViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}