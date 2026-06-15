namespace PsychologyApp.Presentation.ViewModels.Physics;

public partial class PhysicsSearchViewModel
{
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                UpdateSearchUiState();
                DebouncedSearch(value);
            }
        }
    }
}
