using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

public partial class TechniqueSessionViewModel
{
    protected override void OnTechniqueContentChanged()
    {
        if (AppliedUiKind == TechniqueUiKind.Entry)
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
