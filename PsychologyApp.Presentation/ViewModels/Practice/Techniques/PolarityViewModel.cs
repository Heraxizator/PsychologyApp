using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public class PolarityViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly IDialogService _dialogService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ICommand Add { get; private set; } = default!;
    public Command<Polarity> Delete { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ICommand CompleteCommand { get; private set; } = default!;
    public ObservableCollection<Polarity> polarities { get; private set; } = [];

    public string FirstPolarityLabel => AppStrings.FirstPolarityLabel;
    public string SecondPolarityLabel => AppStrings.SecondPolarityLabel;
    public string NegativePlaceholder => AppStrings.PolarityNegativePlaceholder;
    public string PositivePlaceholder => AppStrings.PolarityPositivePlaceholder;

    public PolarityViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        IDialogService dialogService)
    {
        _userProgressService = userProgressService;
        _dialogService = dialogService;
        ApplyTechnique(TechniqueId.Polarity);
        IsFull = false;
        BindNavigation(navigationService.Navigation, navigationService);
        Add = new Command(ToAdd);
        Delete = new Command<Polarity>(DeleteItem);
        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
        LoadDraftAsync().FireAndForget();
    }

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(FirstPolarityLabel));
        OnPropertyChanged(nameof(SecondPolarityLabel));
        OnPropertyChanged(nameof(NegativePlaceholder));
        OnPropertyChanged(nameof(PositivePlaceholder));
    }

    private async Task LoadDraftAsync()
    {
        PolarityDraft? draft = await SessionDraftStore.LoadAsync<PolarityDraft>(_userProgressService, TechniqueId.Polarity.ToString());
        if (draft?.Items is null)
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            foreach (PolarityDraftItem item in draft.Items)
            {
                polarities.Add(new Polarity
                {
                    Id = item.Id,
                    Positive = item.Positive,
                    Negative = item.Negative
                });
            }

            IsFull = polarities.Count > 0;
        });
    }

    private Task SaveDraftAsync() =>
        SessionDraftStore.SaveAsync(_userProgressService, TechniqueId.Polarity.ToString(), new PolarityDraft
        {
            Items = polarities.Select(p => new PolarityDraftItem
            {
                Id = p.Id,
                Positive = p.Positive,
                Negative = p.Negative
            }).ToList()
        });

    private async Task CompleteSessionAsync()
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - _sessionStartedAt).TotalSeconds);
        await _userProgressService.RecordTechniqueCompletionAsync(
            TechniqueId.Polarity.ToString(),
            ModuleName,
            PageName,
            durationSeconds);
        await _userProgressService.DeleteSessionDraftAsync(TechniqueId.Polarity.ToString());
        await PracticeCompletionNavigator.NavigateAfterCompletionAsync(
            NavigationService!,
            _dialogService,
            _userProgressService);
    }

    private void DeleteItem(Polarity item)
    {
        if (item is null)
        {
            return;
        }

        polarities.Remove(item);
        if (polarities.Count == 0)
        {
            IsFull = false;
        }

        SaveDraftAsync().FireAndForget();
    }

    private void ToAdd(object obj)
    {
        if (string.IsNullOrEmpty(Negative) || string.IsNullOrEmpty(Positive))
        {
            return;
        }

        IsFull = true;
        Polarity item = new()
        {
            Id = AppStrings.PoleNumber(polarities.Count + 1),
            Positive = Positive,
            Negative = Negative
        };
        polarities.Add(item);
        Polarity = item;
        Negative = string.Empty;
        Positive = string.Empty;
        SaveDraftAsync().FireAndForget();
    }

    private string positive = string.Empty;
    public string Positive
    {
        get => positive;
        set => SetProperty(ref positive, value);
    }

    private string negative = string.Empty;
    public string Negative
    {
        get => negative;
        set => SetProperty(ref negative, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }

    private Polarity polarity = default!;
    public Polarity Polarity
    {
        get => polarity;
        set => SetProperty(ref polarity, value);
    }

    private sealed class PolarityDraft
    {
        public List<PolarityDraftItem> Items { get; set; } = [];
    }

    private sealed class PolarityDraftItem
    {
        public string? Id { get; set; }
        public string? Positive { get; set; }
        public string? Negative { get; set; }
    }
}
