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
    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<Quots> Quots { get; private set; } = [];

    public UserViewModel(INavigation navigation)
    {
        this.Title = "Профиль";

        this.Techniques = new ObservableCollection<TechniqueItem>();

        this.Quots = new ObservableCollection<Quots>();

        this.OpenOptionsCommand = new Command(() => navigation.PushAsync(new OptionsPage(), false));

        Initialization = InitAsync();
    }

    public async Task InitAsync()
    {
        SetInit();

        this.Techniques.Add(new TechniqueItem
        {
            Title = "BSFF",
            Subtitle = "Методика депрограммирования подсознания"
        });

        await QuotHandler.GetQuotsFromApi(10000);

        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetQuotsList(2);

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            this.Quots.Add(new Quots()
            {
                Text = quotDTO.Text,
                Author = quotDTO.Title
            });
        }

        SetDone();
    }
}
