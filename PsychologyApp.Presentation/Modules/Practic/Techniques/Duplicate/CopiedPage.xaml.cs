using MobileHelperMaui.ViewModels.TechniqueViewModels;
using PsychologyApp.Presentation.Models;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class CopiedPage : ContentPage
{
    public CopiedPage()
    {
        InitializeComponent();
        BindingContext = new CopiedViewModel(Navigation);
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

    private void Papers_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        Papers.SelectedItem = null;
    }
}