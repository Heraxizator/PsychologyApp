using PsychologyApp.Presentation.Entities.Technique;
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
}
