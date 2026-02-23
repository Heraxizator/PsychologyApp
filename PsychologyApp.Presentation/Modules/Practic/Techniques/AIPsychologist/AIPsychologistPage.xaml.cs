using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

namespace MobileHelperMaui.Views.TechniquePages;

public partial class AIPsychologistPage : ContentPage
{
    private readonly AIPsychologistViewModel ViewModel = default!;
    private DateTime DateBegin = default!;

    public AIPsychologistPage()
    {
        InitializeComponent();

        ViewModel = new AIPsychologistViewModel(Navigation);
        BindingContext = ViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DateBegin = DateTime.Now;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        
        StatisticService statisticService = new();
        await statisticService.AddSingleAsync(new StatisticDTO 
        { 
            ModuleName = ViewModel.ModuleName, 
            PageName = ViewModel.PageName, 
            DateTime = DateTime.Now, 
            SecondsDuration = (int)DateTime.Now.Subtract(DateBegin).TotalSeconds 
        }, 3000);
    }
}
