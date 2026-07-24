using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public void SubscribeToTechniqueChanges() =>
        _techniqueMessenger.Subscribe(this, message => ApplyTechniqueMessageAsync(message).FireAndForget());

    public Task RefreshOnAppearAsync()
    {
        if (!_initialized)
        {
            return EnsureInitializedAsync();
        }

        return RefreshDashboardOnAppearAsync();
    }

    public async Task TryOpenPendingTechniqueAsync()
    {
        if (_dashboardLoader.ConsumePendingTechnique() is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }

    public async Task TryOpenPendingJournalAsync()
    {
        if (!UserPreferences.ConsumePendingOpenJournal())
        {
            return;
        }

        await _navigationService.GoToJournalAsync();
    }
}
