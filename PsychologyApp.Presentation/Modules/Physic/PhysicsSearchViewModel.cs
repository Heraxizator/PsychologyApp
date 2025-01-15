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

    public List<ReasonDTO> Reasons { get; private set; } = [];
    public ObservableRangeCollection<ReasonDTO> Results { get; private set; } = [];

    public PhysicsSearchViewModel(INavigation navigation)
    {
        this.Title = "Поиск";

        this.Reasons = new List<ReasonDTO>();

        this.Results = new ObservableRangeCollection<ReasonDTO>();

        this.Reload = new Command(() => ReloadAsync());

        this.Cancel = new Command(() => SetFail());

        Initialization = InitAsync();

        ConfigureState();
    }

    private void ConfigureState()
    {
        if (this.Reasons.Any() || this.Results.Any())
        {
            SetDone();
            return;
        }

        SetFail();
    }

    private async void ReloadAsync()
    {
        this.Reasons.Clear();

        this.Results.Clear();

        await InitAsync();
    }

    private async Task PrepareReasons()
    {
        await ReasonHelper.SavePsyhosomaticData();

        IEnumerable<ReasonDTO> reasonDTOs = await this._reasonService.GetReasons(1000, 15000);

        this.Reasons.AddRange(reasonDTOs);
    }

    private void PrepareResults()
    {
        IEnumerable<ReasonDTO> source = this.Reasons.Take(30);

        this.Results.AddRange(source);
    }

    private async Task InitAsync()
    {
        await PrepareReasons();

        PrepareResults();
    }

    public void ExecuteSearch(string input)
    {
        SetInit();

        this.Results.Clear();

        if (string.IsNullOrEmpty(input))
        {
            ConfigureState();
            return;
        }

        string text = input.ToLower();

        IEnumerable<ReasonDTO> source = this.Reasons
            .Where(x => x.Title?.Length >= text.Length && x.Title.ToLower().Contains(text));

        this.Results.AddRange(source);

        ConfigureState();
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
