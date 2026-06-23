using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.TechniqueCreated;

public partial class CreatedViewModel
{
    public string PageTitle => AppStrings.TechniqueTitle;
    public string CustomTechniqueTitle => AppStrings.PracticeCustomTechnique;
    public string AlgorithmTitle => AppStrings.TechniqueAlgorithm;
    public string FinishButtonText => AppStrings.TechniqueFinish;
    public string EditToolbarText => AppStrings.Edit;
    public string RemoveToolbarText => AppStrings.Remove;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(CustomTechniqueTitle),
            nameof(AlgorithmTitle),
            nameof(FinishButtonText),
            nameof(EditToolbarText),
            nameof(RemoveToolbarText));
    }
}
