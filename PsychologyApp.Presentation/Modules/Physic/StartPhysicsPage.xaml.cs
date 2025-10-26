using MobileHelper.ViewModels.PhysicsViewModels;

namespace MobileHelperMaui.Views.PhysicsPages;

public partial class StartPhysicsPage : ContentPage
{
    private StartPhysicsViewModel viewModel;
    public StartPhysicsPage()
    {
        InitializeComponent();

        this.viewModel = new StartPhysicsViewModel(this.Navigation);

        this.BindingContext = this.viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        this.viewModel.SetDone();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PhysicsSerchPage(), false);
    }
}