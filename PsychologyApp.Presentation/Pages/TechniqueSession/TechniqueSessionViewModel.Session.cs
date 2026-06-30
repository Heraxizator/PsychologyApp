using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.TechniqueSession;

public partial class TechniqueSessionViewModel
{
    protected override void OnTechniqueContentChanged()
    {
        if (TechniqueDefinitionMapper.ToPresentation(TechniqueCatalogService!.Get(_techniqueId)).UiKind == TechniqueUiKind.Entry)
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
