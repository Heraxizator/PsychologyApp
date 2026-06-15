using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Practice;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class TechniqueSessionViewModel
{
    protected override void OnTechniqueContentChanged()
    {
        if (TechniqueCatalog.Get(_techniqueId).UiKind == TechniqueUiKind.Entry)
        {
            _entryDraftCoordinator.WireHandlers();
            _entryDraftCoordinator.LoadAsync(() => OnPropertyChanged(nameof(Entries))).FireAndForget();
        }
    }

    public void SaveEntryDraftIfNeeded() => _entryDraftCoordinator.SaveIfNeeded();

    private async Task CompleteSessionAsync()
    {
        _entryDraftCoordinator.MarkSessionCompleted();

        await _sessionHelper.CompleteAsync(
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt);
    }
}
