using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public class TechniqueSessionViewModel : BaseViewModel
{
    public TechniqueSessionViewModel(INavigation navigation, TechniqueId techniqueId, INavigationService navigationService)
    {
        ApplyTechnique(techniqueId);
        BindNavigation(navigation, navigationService);
    }
}
