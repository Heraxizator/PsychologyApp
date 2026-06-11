using System.ComponentModel;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.UI.Components;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public class TechniqueSessionViewModel : BaseViewModel
{
    private const int DraftSaveDebounceMs = 400;

    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly ITechniqueService? _techniqueService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;
    private CancellationTokenSource? _draftSaveCts;
    private bool _entryDraftHandlersWired;
    private bool _sessionCompleted;

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

    public void SaveEntryDraftIfNeeded()
    {
        if (_sessionCompleted)
        {
            return;
        }

        SaveEntryDraftAsync().FireAndForget();
    }

    private void WireEntryDraftHandlers()
    {
        UnwireEntryDraftHandlers();
        foreach (EntryItem entry in Entries)
        {
            entry.PropertyChanged += OnEntryFieldChanged;
        }

        _entryDraftHandlersWired = true;
    }

    private void UnwireEntryDraftHandlers()
    {
        if (!_entryDraftHandlersWired)
        {
            return;
        }

        foreach (EntryItem entry in Entries)
        {
            entry.PropertyChanged -= OnEntryFieldChanged;
        }

        _entryDraftHandlersWired = false;
    }

    private void OnEntryFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_sessionCompleted || e.PropertyName != nameof(EntryItem.Text))
        {
            return;
        }

        CancelDraftSave();
        _draftSaveCts = new CancellationTokenSource();
        DebouncedSaveEntryDraftAsync(_draftSaveCts.Token).FireAndForget();
    }

    private async Task DebouncedSaveEntryDraftAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(DraftSaveDebounceMs, token);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (_sessionCompleted || token.IsCancellationRequested)
                {
                    return;
                }

                await SaveEntryDraftAsync();
            });
        }
        catch (TaskCanceledException)
        {
        }
    }

    private void CancelDraftSave()
    {
        _draftSaveCts?.Cancel();
        _draftSaveCts?.Dispose();
        _draftSaveCts = null;
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
        _sessionCompleted = true;
        CancelDraftSave();
        UnwireEntryDraftHandlers();

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
