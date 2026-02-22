using MobileHelperMaui.ViewModels.TechniqueViewModels;
using MobileHelperMaui.Views.ProfilePages;
using PsychologyApp.Presentation.Technique.Main;
using PsychologyApp.Presentation.Base.ServiceLocator;
using Microsoft.Maui.Controls;

namespace MobileHelperMaui.Views;

public partial class TechniquesPage : ContentPage
{
    private readonly TechniquesViewModel viewModel;
    public TechniquesPage()
    {
        InitializeComponent();

        Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Light;

        var toastService = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IToastService>() 
            ?? throw new InvalidOperationException("IToastService not registered");

        this.BindingContext = new TechniquesViewModel(this.Navigation, toastService);

        this.viewModel = this.BindingContext as TechniquesViewModel;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await this.Navigation.PushAsync(new UserPage(), false);
    }
}