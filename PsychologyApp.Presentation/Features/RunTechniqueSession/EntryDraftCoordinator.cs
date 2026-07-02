using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Serialization;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class EntryDraftCoordinator(ILogger<EntryDraftCoordinator> logger)
{
    private const int DraftSaveDebounceMs = 400;

    private readonly ILogger<EntryDraftCoordinator> _logger = logger;

    private CancellationTokenSource? _draftSaveCts;
    private bool _handlersWired;
    private bool _sessionCompleted;
    private TechniqueId _techniqueId;
    private List<EntryItem> _entries = [];
    private IUserProgressService _userProgressService = default!;

    public void Attach(TechniqueId techniqueId, List<EntryItem> entries, IUserProgressService userProgressService)
    {
        _techniqueId = techniqueId;
        _entries = entries;
        _userProgressService = userProgressService;
    }

    public void WireHandlers()
    {
        UnwireHandlers();
        foreach (EntryItem entry in _entries)
        {
            entry.PropertyChanged += OnEntryFieldChanged;
        }

        _handlersWired = true;
    }

    public void UnwireHandlers()
    {
        if (!_handlersWired)
        {
            return;
        }

        foreach (EntryItem entry in _entries)
        {
            entry.PropertyChanged -= OnEntryFieldChanged;
        }

        _handlersWired = false;
    }

    public void MarkSessionCompleted()
    {
        _sessionCompleted = true;
        CancelDraftSave();
        UnwireHandlers();
    }

    public void SaveIfNeeded()
    {
        if (_sessionCompleted)
        {
            return;
        }

        SaveAsync().FireAndForget();
    }

    public async Task LoadAsync(Action onEntriesChanged)
    {
        EntryDraft? draft = await SessionDraftStore.LoadAsync(
            _userProgressService,
            _techniqueId.ToString(),
            SessionDraftJsonSerializerContext.Default.EntryDraft,
            _logger);

        if (draft?.Fields is null || draft.Fields.Count == 0)
        {
            return;
        }

        await UiThread.RunAsync(() =>
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (draft.Fields.TryGetValue(i.ToString(), out string? value))
                {
                    _entries[i].Text = value;
                }
            }

            onEntriesChanged();
        });
    }

    public async Task SaveAsync()
    {
        Dictionary<string, string> fields = new(StringComparer.Ordinal);
        for (int i = 0; i < _entries.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(_entries[i].Text))
            {
                fields[i.ToString()] = _entries[i].Text;
            }
        }

        string key = _techniqueId.ToString();
        if (fields.Count == 0)
        {
            await _userProgressService.DeleteSessionDraftAsync(key);
            return;
        }

        await SessionDraftStore.SaveAsync(
            _userProgressService,
            key,
            new EntryDraft { Fields = fields },
            SessionDraftJsonSerializerContext.Default.EntryDraft);
    }

    private void OnEntryFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_sessionCompleted || e.PropertyName != nameof(EntryItem.Text))
        {
            return;
        }

        CancelDraftSave();
        _draftSaveCts = new CancellationTokenSource();
        DebouncedSaveAsync(_draftSaveCts.Token).FireAndForget();
    }

    private async Task DebouncedSaveAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(DraftSaveDebounceMs, token);
            await UiThread.RunAsync(async () =>
            {
                if (_sessionCompleted || token.IsCancellationRequested)
                {
                    return;
                }

                await SaveAsync();
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
}
