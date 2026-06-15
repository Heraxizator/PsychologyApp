using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Practice.Constructor;

public partial class DesignerViewModel
{
    public string PageTitle => AppStrings.TechniqueTitle;
    public string DesignTitle => AppStrings.PracticeDesignTitle;
    public string BackText => AppStrings.Back;
    public string DescriptionSection => AppStrings.TestsDescriptionHeader;
    public string NameFieldLabel => AppStrings.NameLabel;
    public string DescriptionFieldLabel => AppStrings.TestsDescriptionHeader;
    public string ThemeFieldLabel => AppStrings.ThemeLabel;
    public string AuthorFieldLabel => AppStrings.AuthorLabel;
    public string AlgorithmSection => AppStrings.TechniqueAlgorithm;
    public string ActionsFieldLabel => AppStrings.ActionsListLabel;
    public string SaveButtonText => AppStrings.Save;
    public string NamePlaceholder => AppStrings.DesignerNamePlaceholder;
    public string DescriptionPlaceholder => AppStrings.DesignerDescriptionPlaceholder;
    public string ThemePlaceholder => AppStrings.DesignerThemePlaceholder;
    public string AuthorPlaceholder => AppStrings.DesignerAuthorPlaceholder;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(DesignTitle),
            nameof(BackText),
            nameof(DescriptionSection),
            nameof(NameFieldLabel),
            nameof(DescriptionFieldLabel),
            nameof(ThemeFieldLabel),
            nameof(AuthorFieldLabel),
            nameof(AlgorithmSection),
            nameof(ActionsFieldLabel),
            nameof(SaveButtonText),
            nameof(NamePlaceholder),
            nameof(DescriptionPlaceholder),
            nameof(ThemePlaceholder),
            nameof(AuthorPlaceholder));
    }
}
