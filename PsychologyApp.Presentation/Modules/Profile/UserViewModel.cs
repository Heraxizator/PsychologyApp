using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.Technique;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Views.ProfilePages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileHelper.ViewModels.ProfileViewModels;

public class UserViewModel : BaseViewModel
{
    private static Task? Initialization = default;

    private readonly QuotService _quotService = new();
    private readonly StatisticService _statisticService = new();

    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand ReloadQuotsCommand { get; private set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<Quots> Quots { get; private set; } = [];

    public UserViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Профиль";

        Techniques = [];

        Quots = [];

        OpenOptionsCommand = new Command(() => navigation.PushAsync(new OptionsPage(), false));

        ReloadQuotsCommand = new Command(async () => await InitAsync());

        SetInit();

        Initialization = InitAsync();

        SetDone();
    }

    public async Task InitAsync(int cancelTimeout = 10000)
    {
        try
        {
            InitTechniques();

            await InitQuotsAsync(cancelTimeout);

            await GetQuotsAsync(cancelTimeout);

            if (Quots.Any() is false)
            {
                await InitQuotsAsync(cancelTimeout);
            }

            if (Quots.Any() is false)
            {
                InitQuots();
            }

            await SetCompletedTechniquesCountAsync(cancelTimeout);
        }

        catch (Exception)
        {
            SetDone();
        }
    }

    private void InitTechniques()
    {
        Techniques.Add(new TechniqueItem
        {
            Title = "BSFF",
            Subtitle = "Методика депрограммирования подсознания"
        });
    }

    private void InitQuots()
    {
        Quots.Add(new Quots()
        {
            Text = "Лето, урожай, война.",
            Author = "Латинская пословица"
        });
    }

    private async Task InitQuotsAsync(int cancelTimeout = 10000)
    {
        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetQuotsList(2, false, cancelTimeout);

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (string.IsNullOrEmpty(quotDTO.Text) || string.IsNullOrEmpty(quotDTO.Title))
            {
                continue;
            }

            Quots.Add(new Quots()
            {
                Text = quotDTO.Text,
                Author = quotDTO.Title
            });
        }
    }

    private Task GetQuotsAsync(int cancelTimeout = 10000)
    {
        return QuotHandler.GetQuotsFromApi(cancelTimeout);
    }

    private async Task SetCompletedTechniquesCountAsync(int cancelTimeout = 5000)
    {
        TechniquesCompletedCount = (await _statisticService.CountPageCompletedAsync(cancelTimeout)).ToString();
    }

    private string _techniques_completed_count = "0";
    public string TechniquesCompletedCount
    {
        get => _techniques_completed_count;
        set
        {
            if (_techniques_completed_count != value)
            {
                _techniques_completed_count = value;
                OnPropertyChanged(nameof(TechniquesCompletedCount));
            }
        }
    }
}
