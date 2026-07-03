using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public void SubscribeToTechniqueChanges() =>
        _techniqueMessenger.Subscribe(this, message => OnTechniqueMessageAsync(message).FireAndForget());

    public Task RefreshOnAppearAsync() =>
        InitializeAsync(showLoadingOverlay: false);

    public async Task TryOpenPendingTechniqueAsync()
    {
        if (_dashboardLoader.ConsumePendingTechnique() is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }
}
