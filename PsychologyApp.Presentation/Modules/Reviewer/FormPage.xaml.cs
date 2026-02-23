using MobileHelper.ViewModels.ReviewViewModels;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using Microsoft.Maui.Controls;

namespace MobileHelperMaui.Views.ReviewPages;

public partial class FormPage : ContentPage
{
    public FormPage()
    {
        InitializeComponent();

        var dialogService = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<IDialogService>() 
            ?? throw new InvalidOperationException("IDialogService not registered");

        this.BindingContext = new FormViewModel(dialogService);
    }
}