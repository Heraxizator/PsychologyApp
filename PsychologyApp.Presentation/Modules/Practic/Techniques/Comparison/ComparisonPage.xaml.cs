using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Modules.Practic.Techniques.Comparison;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ComparisonPage : ContentPage
{
    private readonly ComparisonViewModel ViewModel = default!;
    private DateTime DateBegin = default!;

    public ComparisonPage()
    {
        InitializeComponent();

        ViewModel = new ComparisonViewModel(Navigation);
        BindingContext = new ComparisonViewModel(Navigation);
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