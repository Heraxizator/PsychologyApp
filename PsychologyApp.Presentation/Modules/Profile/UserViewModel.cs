using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Base.Constants;
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
    private readonly QuotService _quotService = new();
    private readonly StatisticService _statisticService = new();

    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand ReloadQuotsCommand { get; private set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<Quots> Quots { get; private set; } = [];

    public UserViewModel(INavigation navigation)
    {
        try
        {
            this.ModuleName = "Практик";
            this.PageName = "Профиль";

            this.OpenOptionsCommand = new Command(() => navigation.PushAsync(new OptionsPage(), false));
            this.ReloadQuotsCommand = new Command(async () => await InitAsync(Constants.MiddleBaseTimeout));

            Task.Run(async () => await InitAsync(Constants.MiddleBaseTimeout));
        }
        
        catch (Exception e)
        {
            SetFail();
            Console.WriteLine(e.Message);
        }
    }

    public async Task InitAsync(int cancelTimeout)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetInit();
                InitQuots();
                InitTechniques();
            });

            await SetCompletedTechniquesCountAsync(cancelTimeout);

            await GetQuotsAsync(cancelTimeout);

            await InitQuotsAsync(cancelTimeout);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });
        }

        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });

            Console.WriteLine(e.Message);
        }
    }

    private void InitTechniques()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Techniques.Add(new TechniqueItem
            {
                Title = "BSFF",
                Subtitle = "Методика депрограммирования подсознания"
            });
        });
        
    }

    private void InitQuots()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Quots.Add(new Quots()
            {
                Text = "Лето, урожай, война.",
                Author = "Латинская пословица"
            });
        });
    }

    private async Task InitQuotsAsync(int cancelTimeout)
    {
        try
        {
            IEnumerable<QuotDTO> quotDTOs = await _quotService.GetAllAsync(2, cancelTimeout);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
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
            });
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private Task GetQuotsAsync(int cancelTimeout)
    {
        return _quotService.LoadSingleAsync(cancelTimeout);
    }

    private async Task SetCompletedTechniquesCountAsync(int cancelTimeout)
    {
        try
        {
            TechniquesCompletedCount = (await _statisticService.CountPageCompletedAsync(cancelTimeout)).ToString();
        }

        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
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
