using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Modules.Practic.Techniques.Resize;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ResizePage : ContentPage
{
    private readonly ResizeViewModel ViewModel = default!;
    private DateTime DateBegin = default!;
    public ResizePage()
    {
        InitializeComponent();

        ViewModel = new ResizeViewModel(Navigation);
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