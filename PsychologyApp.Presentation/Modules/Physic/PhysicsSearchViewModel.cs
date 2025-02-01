using MvvmHelpers;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Infrastructure.Data.Context;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace MobileHelper.ViewModels.PhysicsViewModels;

public class PhysicsSearchViewModel : BaseViewModel
{
    private static Task? Initialization = default;
    private readonly ReasonService _reasonService = new();

    public List<ReasonDTO> ReasonsList { get; private set; } = [];
    public ObservableRangeCollection<ReasonDTO> ResultsObservableCollection { get; private set; } = [];

    public PhysicsSearchViewModel(INavigation navigation)
    {
        this.ModuleName = "Психосоматик";
        this.PageName = "Поисковик";

        this.Reload = new Command(() => ReloadAsync());

        this.Cancel = new Command(() => SetFail());

        Initialization = InitAsync();

        ConfigureState();
    }

    private void ConfigureState()
    {
        if (IfEmpty() is true)
        {
            SetDone();
            return;
        }

        SetFail();
    }

    private bool IfEmpty()
    {
        return (this.ReasonsList.Any() || this.ResultsObservableCollection.Any());
    }

    private async void ReloadAsync()
    {
        this.ReasonsList.Clear();
        this.ResultsObservableCollection.Clear();

        await InitAsync();
    }

    private async Task InitAsync()
    {
        await ReasonExtension.SavePsyhosomaticData();

        IEnumerable<ReasonDTO> reasonDTOs = await this._reasonService.GetReasons(1000, 15000);

        this.ReasonsList.AddRange(reasonDTOs);

        IEnumerable<ReasonDTO> source = this.ReasonsList.Take(50);

        this.ResultsObservableCollection.AddRange(source);
    }

    public async void ExecuteSearch(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            ConfigureState();
            return;
        }

        SetInit();

        this.ResultsObservableCollection.Clear();

        await Task.Run(() =>
        {
            string text = input.ToLower();

            IEnumerable<ReasonDTO> source = this.ReasonsList
                .Where(x => x.Title?.Length >= text.Length && x.Title.ToLower().Contains(text));

            Application.Current.Dispatcher.Dispatch(() =>
            {
                this.ResultsObservableCollection.AddRange(source);

                ConfigureState();
            });

        });
    }

    public PhysicsSearchViewModel() { }

    private string? _search_text;
    public string? SearchText
    {
        get => this._search_text;
        set
        {
            if (this._search_text != value)
            {
                this._search_text = value;
                OnPropertyChanged(nameof(this.SearchText));
            }
        }
    }
}
