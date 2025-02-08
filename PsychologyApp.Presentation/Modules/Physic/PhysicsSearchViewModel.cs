using MvvmHelpers;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Infrastructure.Data.Context;
using System.Windows.Input;
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

        this.Reload = new Command(ReloadAsync);

        this.Cancel = new Command(SetFail);

        Initialization = InitAsync();
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

        this.ResultsObservableCollection.AddRange(reasonDTOs.Take(50));
    }

    public async void ExecuteSearch(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
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

                ExecuteSearch(this._search_text ?? string.Empty);

                OnPropertyChanged(nameof(this.SearchText));
            }
        }
    }
}
