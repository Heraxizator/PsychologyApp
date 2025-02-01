using MobileHelper.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class FindProblemPage : ContentPage
{
    public FindProblemPage(string? describtion, List<string> algorithm, string? comment, Action action)
    {
        InitializeComponent();

        BindingContext = new FindProblemViewModel(Navigation, describtion, algorithm, comment, action);
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync(false);
    }
}