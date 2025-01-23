using MobileHelperMaui.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.Models;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class PaperPage : ContentPage
{
    public PaperPage()
    {
        InitializeComponent();
        BindingContext = new PaperViewModel(Navigation);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as PaperViewModel;

        if (vm!.PapersObservableCollection.Any() is false)
        {
            return;
        }

        //Papers.ScrollTo(vm!.PapersObservableCollection.LastOrDefault(), ScrollToPosition.End, false);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }

    private void TapGestureRecognizer_Tapped_2(object sender, TappedEventArgs e)
    {
        Algorithm.IsVisible = false;
    }

}