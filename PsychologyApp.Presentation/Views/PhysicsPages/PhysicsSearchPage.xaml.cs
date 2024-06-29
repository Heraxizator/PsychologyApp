using MobileHelper.ViewModels.PhysicsViewModels;

namespace MobileHelperMaui.Views.PhysicsPages;

public partial class PhysicsSerchPage : ContentPage
{
    PhysicsSearchViewModel ViewModel { get; set; }
    public PhysicsSerchPage(PhysicsSearchViewModel vm)
    {
        InitializeComponent();

        this.ViewModel = vm;

        this.BindingContext = vm;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.ViewModel.ExecuteSearch(this.ViewModel.SearchText);
    }
}