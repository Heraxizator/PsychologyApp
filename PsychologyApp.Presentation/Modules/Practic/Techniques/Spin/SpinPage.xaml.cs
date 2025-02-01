using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Modules.Practic.Techniques.Spin;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class SpinPage : ContentPage
{
    private readonly SpinViewModel ViewModel = default!;
    private DateTime DateBegin = default!;
    public SpinPage()
    {
        InitializeComponent();

        ViewModel = new SpinViewModel(Navigation);
        BindingContext = ViewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        _ = await Navigation.PopAsync(false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        DateBegin = DateTime.Now;
    }

    protected override async void OnDisappearing()
    {
        StatisticService statisticService = new();

        await statisticService.AddSingleAsync(new StatisticDTO { ModuleName = ViewModel.ModuleName, PageName = ViewModel.PageName, DateTime = DateTime.Now, SecondsDuration = DateBegin.Subtract(DateTime.Now).Seconds }, 3000);
    }
}