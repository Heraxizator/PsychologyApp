using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Practice;

namespace PsychologyApp.Presentation.ViewModels.Practice;

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
