using PsychologyApp.Presentation.ViewModels.ProfileViewModels;

namespace PsychologyApp.Presentation.Views.ProfilePages;

public partial class OptionsPage : ContentPage
{
    public OptionsPage()
    {
        InitializeComponent();

        OptionsViewModel viewModel = new(Navigation);

        BindingContext = viewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }
}