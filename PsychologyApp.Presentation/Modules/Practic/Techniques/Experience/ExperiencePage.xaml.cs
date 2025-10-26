using MobileHelper.ViewModels.TechniqueViewModels;
using PsychologyApp.Application;
using PsychologyApp.Application.Models;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class ExperiencePage : ContentPage
{
    private readonly ExperienceViewModel ViewModel = default!;
    private DateTime DateBegin = default!;

    public ExperiencePage()
    {
        InitializeComponent();

        ViewModel = new ExperienceViewModel(Navigation);
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