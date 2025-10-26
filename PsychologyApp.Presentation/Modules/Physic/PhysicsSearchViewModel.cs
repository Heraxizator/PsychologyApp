using MvvmHelpers;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Infrastructure.Data.Context;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace MobileHelper.ViewModels.PhysicsViewModels;

public class PhysicsSearchViewModel : BaseViewModel
{
    private readonly IReasonService _reasonService = new ReasonService();

    public List<ReasonDTO> ReasonsList { get; private set; } = [];
    public ObservableRangeCollection<ReasonDTO> ResultsObservableCollection { get; private set; } = [];

    public PhysicsSearchViewModel(INavigation navigation)
    {
        try
        {
            this.ModuleName = "Психосоматик";
            this.PageName = "Поисковик";

            this.Reload = new Command(ReloadAsync);
            this.Cancel = new Command(SetFail);

            Task.Run(async () => await InitAsync(Constants.MiddleBaseTimeout));
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            SetFail();
        }
    }

    private async void ReloadAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                this.ReasonsList.Clear();
                this.ResultsObservableCollection.Clear();
            });

            await InitAsync(Constants.MiddleBaseTimeout);
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetFail();
            });

            Console.WriteLine(e.Message);
        }
    }

    private async Task InitAsync(int cancelTimeout)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetInit();
            });

            IEnumerable<ReasonDTO> reasonDTOs = await this._reasonService.GetReasonsAsync(0, 10000, 15000);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                this.ReasonsList.AddRange(reasonDTOs);
                this.ResultsObservableCollection.AddRange(reasonDTOs.Take(50));
            });

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetFail();
            });

            Console.WriteLine(e.Message);
        }
    }

    public async void ExecuteSearch(string input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input) is true)
            {
                return;
            }

            await Task.Run(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SetInit();
                });

                string text = input.ToLower();

                IEnumerable<ReasonDTO> source = this.ReasonsList
                    .Where(x => x.Title?.Length >= text.Length && x.Title.ToLower().Contains(text));

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    this.ResultsObservableCollection.Clear();
                    this.ResultsObservableCollection.AddRange(source);

                    SetDone();
                });

            });
        }
        
        catch (Exception e)
        {
            SetDone();

            Console.WriteLine(e.Message);
        }
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
