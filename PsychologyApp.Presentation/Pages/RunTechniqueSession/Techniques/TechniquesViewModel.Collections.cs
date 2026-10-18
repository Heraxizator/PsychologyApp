using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public RangeObservableCollection<TechniqueGroup> TechniqueGroups { get; private set; } = [];
    public RangeObservableCollection<TechniqueItem> CatalogTechniques { get; private set; } = [];

    private bool _isTechniquesGrouped;
    private object _techniquesItemsSource = new ObservableCollection<TechniqueItem>();

    public bool IsTechniquesGrouped
    {
        get => _isTechniquesGrouped;
        private set => SetProperty(ref _isTechniquesGrouped, value);
    }

    public object TechniquesItemsSource
    {
        get => _techniquesItemsSource;
        private set => SetProperty(ref _techniquesItemsSource, value);
    }

    private void ApplyUiState(TechniqueDashboardUiState uiState)
    {
        _staticItemsAll = [.. (uiState.IsGrouped && uiState.Groups.Count > 0 ? uiState.Groups[0] : uiState.CatalogTechniques)];
        _staticItemsAll.RemoveAll(_programmeItems.Contains);
        _programmeItems = [CreateFearLadderItem(), CreateTraumaItem()];
        _staticItemsAll.InsertRange(0, _programmeItems);

        // The person's own practices keep one group object for as long as the list lives: paging and inserting work on it.
        _customGroup = uiState.IsGrouped && uiState.Groups.Count > 1
            ? new TechniqueGroup(uiState.Groups[^1].Title, uiState.Groups[^1]) { IsCustom = true }
            : null;

        bool groupingChanged = !IsTechniquesGrouped;
        IsTechniquesGrouped = true;
        CatalogTechniques.Clear();
        TechniquesItemsSource = TechniqueGroups;
        RebuildSections();

        if (groupingChanged)
        {
            OnPropertyChanged(nameof(TechniquesItemsSource));
        }
    }

    /// <summary>
    /// The fear ladder (first in "Action") and the trauma module (first in "Heart") sit in the catalogue like practices rather than on
    /// banners above it: more blocks on the main page would crowd it. Each opens its own screen, not a practice session.
    /// </summary>
    private TechniqueItem[] _programmeItems = [];

    private TechniqueItem CreateFearLadderItem() => new()
    {
        Title = AppStrings.ExposureEntryTitle,
        Subtitle = AppStrings.ExposureCatalogSubtitle,
        IconName = "Stairs",
        MetaText = AppStrings.ExposureCatalogMeta,
        Date = AppStrings.ExposureCatalogBadge,
        Author = AppStrings.ExposureCatalogAuthor,
        Flavor = nameof(TechniqueFlavor.Action),
        Active = true,
        TapCommand = OpenExposureCommand
    };

    private TechniqueItem CreateTraumaItem() => new()
    {
        Title = AppStrings.TraumaCatalogTitle,
        Subtitle = AppStrings.TraumaCatalogSubtitle,
        IconName = "Healing",
        MetaText = AppStrings.TraumaCatalogMeta,
        Date = AppStrings.TraumaCatalogBadge,
        Author = AppStrings.TraumaCatalogAuthor,
        Flavor = nameof(TechniqueFlavor.Heart),
        Active = true,
        TapCommand = OpenTraumaCommand
    };

    private void ReplaceGroups(ObservableCollection<TechniqueGroup> sourceGroups)
    {
        TechniqueGroups.ReplaceAll(sourceGroups.Select(group => new TechniqueGroup(group.Title, group)).ToList());
    }

    private void ReplaceCatalog(ObservableCollection<TechniqueItem> sourceItems)
    {
        CatalogTechniques.ReplaceAll(sourceItems);
    }
}
