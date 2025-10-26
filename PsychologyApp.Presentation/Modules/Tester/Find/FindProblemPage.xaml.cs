using MobileHelper.ViewModels.TestViewModels;

namespace MobileHelperMaui.Views.TestPages;

public partial class FindProblemPage : ContentPage
{
    private FindProblemViewModel ViewModel = default!;
    public FindProblemPage(string? describtion, List<string> algorithm, string? comment, Action action)
    {
        InitializeComponent();

        ViewModel = new FindProblemViewModel(Navigation, describtion, algorithm, comment, action);
        BindingContext = ViewModel;
    }

    private async void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        await Navigation.PopToRootAsync(false);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        this.ViewModel.ToContinue();
    }
}