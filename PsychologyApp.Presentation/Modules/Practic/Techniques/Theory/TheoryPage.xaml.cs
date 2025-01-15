using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(string content)
    {
        InitializeComponent();

        this.BindingContext = new TheoryViewModel(content);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}