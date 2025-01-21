using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Technique;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class PolarityPage : ContentPage
{
    public PolarityPage()
    {
        InitializeComponent();
        BindingContext = new PolarityViewModel(Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        PolarityViewModel? vm = BindingContext as PolarityViewModel;

        Polarities.ScrollTo(vm.polarities.Count -1, -1, ScrollToPosition.End, false);

        Algorithm.IsVisible = false;
    }

    private void Polarities_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PolarityViewModel? vm = BindingContext as PolarityViewModel;

        vm!.Delete.Execute(e.CurrentSelection.FirstOrDefault());

        Polarities.SelectedItem = null;
    }
}