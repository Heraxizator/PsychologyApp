using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class FuturePage : ContentPage
{
    public FuturePage()
    {
        InitializeComponent();

        this.BindingContext = new FutureViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}