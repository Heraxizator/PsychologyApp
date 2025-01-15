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

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        ImageButton? button = sender as ImageButton;

        Polarity? item = button!.BindingContext as Polarity;

        PolarityViewModel? vm = BindingContext as PolarityViewModel;

        vm!.Delete.Execute(item);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        PolarityViewModel? vm = BindingContext as PolarityViewModel;

        Polarities.ScrollTo(vm!.Polarity, ScrollToPosition.MakeVisible, true);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}