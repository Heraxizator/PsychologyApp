using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.Polarity;

public partial class PolarityViewModel
{
    public string FirstPolarityLabel => AppStrings.FirstPolarityLabel;
    public string SecondPolarityLabel => AppStrings.SecondPolarityLabel;

    public string NegativePlaceholder
    {
        get
        {
            TechniqueDefinition definition = TechniqueDefinitionMapper.ToPresentation(TechniqueCatalogService!.Get(TechniqueId.Polarity));
            return definition.PolarityNegativePlaceholder ?? AppStrings.PolarityNegativePlaceholder;
        }
    }

    public string PositivePlaceholder
    {
        get
        {
            TechniqueDefinition definition = TechniqueDefinitionMapper.ToPresentation(TechniqueCatalogService!.Get(TechniqueId.Polarity));
            return definition.PolarityPositivePlaceholder ?? AppStrings.PolarityPositivePlaceholder;
        }
    }

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(NegativePlaceholder));
        OnPropertyChanged(nameof(PositivePlaceholder));
        OnPropertyChanged(nameof(FirstPolarityLabel));
        OnPropertyChanged(nameof(SecondPolarityLabel));
    }
}
