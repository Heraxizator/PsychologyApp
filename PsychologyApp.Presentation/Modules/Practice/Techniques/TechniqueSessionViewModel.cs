using System.ComponentModel;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.UI.Components;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public class TechniqueSessionViewModel : BaseViewModel
{
    private const int DraftSaveDebounceMs = 400;

    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly ITechniqueService? _techniqueService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;
    private CancellationTokenSource? _draftSaveCts;
    private bool _entryDraftHandlersWired;

    public ICommand BackCommand { get; }
    public ICommand CompleteCommand { get; }

    public TechniqueSessionViewModel(
        INavigation navigation,
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITechniqueService? techniqueService = null)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _techniqueService = techniqueService;

        ApplyTechnique(techniqueId);
        BindNavigation(navigation, navigationService);

        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
    }

    protected override void OnTechniqueContentChanged()
    {
        if (TechniqueCatalog.Get(_techniqueId).UiKind == TechniqueUiKind.Entry)
        {
            WireEntryDraftHandlers();
            LoadEntryDraftAsync().FireAndForget();
        }
    }

    public void SaveEntryDraftIfNeeded() => SaveEntryDraftAsync().FireAndForget();

    private void WireEntryDraftHandlers()
    {
        if (_entryDraftHandlersWired)
        {
            return;
        }

        _entryDraftHandlersWired = true;
        foreach (EntryItem entry in Entries)
        {
            entry.PropertyChanged += OnEntryFieldChanged;
        }
    }

    private void OnEntryFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(EntryItem.Text))
        {
            return;
        }

        _draftSaveCts?.Cancel();
        _draftSaveCts?.Dispose();
        _draftSaveCts = new CancellationTokenSource();
        CancellationToken token = _draftSaveCts.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(DraftSaveDebounceMs, token);
                await SaveEntryDraftAsync();
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private async Task LoadEntryDraftAsync()
    {
        EntryDraft? draft = await SessionDraftStore.LoadAsync<EntryDraft>(
            _userProgressService,
            _techniqueId.ToString());

        if (draft?.Fields is null || draft.Fields.Count == 0)
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                if (draft.Fields.TryGetValue(i.ToString(), out string? value))
                {
                    Entries[i].Text = value;
                }
            }

            OnPropertyChanged(nameof(Entries));
        });
    }

    private async Task SaveEntryDraftAsync()
    {
        Dictionary<string, string> fields = new(StringComparer.Ordinal);
        for (int i = 0; i < Entries.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(Entries[i].Text))
            {
                fields[i.ToString()] = Entries[i].Text;
            }
        }

        string key = _techniqueId.ToString();
        if (fields.Count == 0)
        {
            await _userProgressService.DeleteSessionDraftAsync(key);
            return;
        }

        await SessionDraftStore.SaveAsync(_userProgressService, key, new EntryDraft { Fields = fields });
    }

    private async Task CompleteSessionAsync()
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - _sessionStartedAt).TotalSeconds);
        string itemKey = _techniqueId.ToString();

        await _userProgressService.RecordTechniqueCompletionAsync(
            itemKey,
            ModuleName,
            PageName,
            durationSeconds);
        await _userProgressService.DeleteSessionDraftAsync(itemKey);

        await GoBackAsync();
    }
}
