using MobileHelperMaui.ViewModels.TechniqueViewModels;
using MobileHelperMaui.Views.ProfilePages;
using PsychologyApp.Presentation.Technique.Main;

namespace MobileHelperMaui.Views;

public partial class TechniquesPage : ContentPage
{
    private readonly TechniquesViewModel viewModel;
    public TechniquesPage()
    {
        InitializeComponent();

        Application.Current.UserAppTheme = AppTheme.Light;

        this.BindingContext = new TechniquesViewModel(this.Navigation);

        this.viewModel = this.BindingContext as TechniquesViewModel;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await this.Navigation.PushAsync(new UserPage(), false);
    }
}