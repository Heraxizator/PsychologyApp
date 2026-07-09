using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public ObservableCollection<TechniqueGroup> TechniqueGroups { get; private set; } = [];
    public ObservableCollection<TechniqueItem> CatalogTechniques { get; private set; } = [];

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
        bool groupingChanged = IsTechniquesGrouped != uiState.IsGrouped;
        IsTechniquesGrouped = uiState.IsGrouped;

        if (uiState.IsGrouped)
        {
            ReplaceGroups(uiState.Groups);
            CatalogTechniques.Clear();
            TechniquesItemsSource = TechniqueGroups;
        }
        else
        {
            TechniqueGroups.Clear();
            ReplaceCatalog(uiState.CatalogTechniques);
            TechniquesItemsSource = CatalogTechniques;
        }

        if (groupingChanged)
        {
            OnPropertyChanged(nameof(TechniquesItemsSource));
        }
    }

    private void ReplaceGroups(ObservableCollection<TechniqueGroup> sourceGroups)
    {
        TechniqueGroups.Clear();
        foreach (TechniqueGroup group in sourceGroups)
        {
            TechniqueGroups.Add(new TechniqueGroup(group.Title, group));
        }
    }

    private void ReplaceCatalog(ObservableCollection<TechniqueItem> sourceItems)
    {
        CatalogTechniques.Clear();
        foreach (TechniqueItem item in sourceItems)
        {
            CatalogTechniques.Add(item);
        }
    }
}
