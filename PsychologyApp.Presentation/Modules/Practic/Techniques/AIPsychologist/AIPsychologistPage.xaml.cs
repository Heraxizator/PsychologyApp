using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

#if ANDROID
using Android.Text;
using Microsoft.Maui.Handlers;
#endif

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

#if ANDROID
        // Настройка автоматической заглавной буквы для Android
        Loaded += OnPageLoaded;
#endif
    }

#if ANDROID
    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (MessageEntry?.Handler?.PlatformView is Android.Widget.EditText editText)
        {
            editText.InputType = InputTypes.ClassText | InputTypes.TextFlagCapSentences;
        }
    }
#endif

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DateBegin = DateTime.Now;

#if ANDROID
        // Настройка автоматической заглавной буквы после появления страницы
        if (MessageEntry?.Handler?.PlatformView is Android.Widget.EditText editText)
        {
            editText.InputType = InputTypes.ClassText | InputTypes.TextFlagCapSentences;
        }
#endif
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
