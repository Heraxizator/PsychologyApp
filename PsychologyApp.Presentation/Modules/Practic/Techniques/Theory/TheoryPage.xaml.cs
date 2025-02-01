using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(string content)
    {
        InitializeComponent();

        BindingContext = new TheoryViewModel(Navigation, content);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }
}