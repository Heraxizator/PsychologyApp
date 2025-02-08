using MobileHelperMaui.ViewModels.TechniqueViewModels;
using PsychologyApp.Application;
using PsychologyApp.Application.Models;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class PaperPage : ContentPage
{
    private readonly PaperViewModel ViewModel = default!;
    private DateTime DateBegin = default!;

    public PaperPage()
    {
        InitializeComponent();

        ViewModel = new PaperViewModel(Navigation);
        BindingContext = ViewModel;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        PaperViewModel? vm = BindingContext as PaperViewModel;

        if (vm!.PapersObservableCollection.Any() is false)
        {
            return;
        }

        //Papers.ScrollTo(vm!.PapersObservableCollection.LastOrDefault(), ScrollToPosition.End, false);
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