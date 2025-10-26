using MobileHelper.ViewModels.PhysicsViewModels;

namespace MobileHelperMaui.Views.PhysicsPages;

public partial class PhysicsSerchPage : ContentPage
{
    PhysicsSearchViewModel ViewModel { get; set; }
    public PhysicsSerchPage()
    {
        InitializeComponent();

        this.ViewModel = new PhysicsSearchViewModel(Navigation);

        this.BindingContext = ViewModel;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.ViewModel.ExecuteSearch(this.ViewModel.SearchText);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        this.ViewModel.ExecuteSearch(e.NewTextValue);
    }
}