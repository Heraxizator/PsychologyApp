using MobileHelper.ViewModels.TechniqueViewModels;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ExperiencePage : ContentPage
{
    public ExperiencePage()
    {
        InitializeComponent();

        this.BindingContext = new ExperienceViewModel(this.Navigation);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}