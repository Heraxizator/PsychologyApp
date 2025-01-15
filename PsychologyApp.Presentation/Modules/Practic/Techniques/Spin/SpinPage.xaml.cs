using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class SpinPage : ContentPage
{
    public SpinPage()
    {
        InitializeComponent();

        this.BindingContext = new SpinViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}