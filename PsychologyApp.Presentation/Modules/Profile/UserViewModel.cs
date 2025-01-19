using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Technique;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Views.ProfilePages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MobileHelper.ViewModels.ProfileViewModels;

public class UserViewModel : BaseViewModel
{
    private static Task? Initialization = default;
    private readonly QuotService _quotService = new();

    public ICommand OpenOptionsCommand { get; set; } = default!;
    public ICommand ReloadQuotsCommand { get; set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<Quots> Quots { get; private set; } = [];

    public UserViewModel(INavigation navigation)
    {
        this.Title = "Профиль";

        this.Techniques = new ObservableCollection<TechniqueItem>();

        this.Quots = new ObservableCollection<Quots>();

        this.OpenOptionsCommand = new Command(() => navigation.PushAsync(new OptionsPage(), false));

        this.ReloadQuotsCommand = new Command(async () => await InitAsync());

        SetInit();

        Initialization = InitAsync();

        SetDone();
    }

    public async Task InitAsync(int cancelTimeout = 10000)
    {
        try
        {
            using CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            InitTechniques();
            InitQuots();

            await InitQuotsAsync();
        }
        
        catch (Exception e)
        {
            SetDone();
        }
    }

    private void InitTechniques()
    {
        this.Techniques.Add(new TechniqueItem
        {
            Title = "BSFF",
            Subtitle = "Методика депрограммирования подсознания"
        });
    }

    private void InitQuots()
    {
        this.Quots.Add(new Quots()
        {
            Text = "Лето, урожай, война.",
            Author = "Латинская пословица"
        });
    }

    private async Task InitQuotsAsync(int cancelTimeout = 10000)
    {
        await QuotHandler.GetQuotsFromApi(cancelTimeout);

        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetQuotsList(2, false, cancelTimeout);

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (string.IsNullOrEmpty(quotDTO.Text) || string.IsNullOrEmpty(quotDTO.Title))
            {
                continue;
            }

            this.Quots.Add(new Quots()
            {
                Text = quotDTO.Text,
                Author = quotDTO.Title
            });
        }
    }
}
