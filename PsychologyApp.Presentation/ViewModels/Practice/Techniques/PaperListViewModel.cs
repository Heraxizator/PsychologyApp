using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public class PaperListViewModel : BaseViewModel
{
    private readonly bool _clearTextAfterAdd;
    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly IDialogService _dialogService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ObservableCollection<Paper> PapersObservableCollection { get; private set; } = [];
    public Command AddCommand { get; private set; } = default!;
    public Command<Paper> DeleteCommand { get; private set; } = default!;
    public Command BackCommand { get; private set; } = default!;
    public Command CompleteCommand { get; private set; } = default!;

    public string ThoughtFieldLabel => Entries.Count > 0 ? Entries[0].Title ?? string.Empty : string.Empty;
    public string ThoughtPlaceholder => Entries.Count > 0 ? Entries[0].Placeholder ?? string.Empty : string.Empty;
    public string RepeatButtonText => AppStrings.Repeat;
    public string ConcernFieldLabel => AppStrings.ConcernLabel;

    public PaperListViewModel(
        INavigationService navigationService,
        TechniqueId techniqueId,
        bool clearTextAfterAdd,
        IUserProgressService userProgressService,
        IDialogService dialogService)
    {
        _techniqueId = techniqueId;
        _clearTextAfterAdd = clearTextAfterAdd;
        _userProgressService = userProgressService;
        _dialogService = dialogService;
        BindNavigation(navigationService.Navigation, navigationService);
        ApplyTechnique(techniqueId);
        AddCommand = new Command(ToAdd);
        DeleteCommand = new Command<Paper>(DeleteItem);
        BackCommand = new Command(async () => await GoBackAsync());
        CompleteCommand = new Command(async () => await CompleteSessionAsync());
        Finish = CompleteCommand;
        LoadDraftAsync().FireAndForget();
    }

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(ThoughtFieldLabel));
        OnPropertyChanged(nameof(ThoughtPlaceholder));
        OnPropertyChanged(nameof(RepeatButtonText));
        OnPropertyChanged(nameof(ConcernFieldLabel));
    }

    private async Task LoadDraftAsync()
    {
        PaperDraft? draft = await SessionDraftStore.LoadAsync<PaperDraft>(_userProgressService, _techniqueId.ToString());
        if (draft?.Items is null)
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            foreach (PaperDraftItem item in draft.Items)
            {
                PapersObservableCollection.Add(new Paper
                {
                    Id = item.Id,
                    Text = item.Text
                });
            }

            SetCollection(PapersObservableCollection.Any());
        });
    }

    private Task SaveDraftAsync() =>
        SessionDraftStore.SaveAsync(_userProgressService, _techniqueId.ToString(), new PaperDraft
        {
            Items = PapersObservableCollection.Select(p => new PaperDraftItem
            {
                Id = p.Id,
                Text = p.Text
            }).ToList()
        });

    private async Task CompleteSessionAsync()
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - _sessionStartedAt).TotalSeconds);
        await _userProgressService.RecordTechniqueCompletionAsync(
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            durationSeconds);
        await _userProgressService.DeleteSessionDraftAsync(_techniqueId.ToString());
        await PracticeCompletionNavigator.NavigateAfterCompletionAsync(
            NavigationService!,
            _dialogService,
            _userProgressService);
    }

    private void SetCollection(bool visible) => IsFull = visible;

    private void ToAdd(object obj)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        SetCollection(true);
        PapersObservableCollection.Add(new Paper
        {
            Id = AppStrings.RecordNumber(PapersObservableCollection.Count + 1),
            Text = Text
        });

        if (_clearTextAfterAdd)
        {
            Text = string.Empty;
        }

        SaveDraftAsync().FireAndForget();
    }

    private void DeleteItem(Paper item)
    {
        if (item is null)
        {
            return;
        }

        PapersObservableCollection.Remove(item);
        SetCollection(PapersObservableCollection.Any());
        SaveDraftAsync().FireAndForget();
    }

    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }

    private sealed class PaperDraft
    {
        public List<PaperDraftItem> Items { get; set; } = [];
    }

    private sealed class PaperDraftItem
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
    }
}
