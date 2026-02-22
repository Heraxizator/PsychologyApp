using PsychologyApp.Presentation.Technique.Constructor;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using Microsoft.Maui.Controls;

namespace MobileHelperMaui.Views.TechniquePages.ConstructorPages;

public partial class CreatedPage : ContentPage
{
    public CreatedPage(long id)
    {
        InitializeComponent();
        var dialogService = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IDialogService>() 
            ?? throw new InvalidOperationException("IDialogService not registered");
        this.BindingContext = new CreatedViewModel(this.Navigation, id, dialogService);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}