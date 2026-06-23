using PsychologyApp.Presentation.Features.SearchPhysics;

namespace PsychologyApp.Presentation.Pages.PhysicsSearch;

public partial class PhysicsSearchViewModel
{
    private bool _isSearching;
    public bool IsSearching
    {
        get => _isSearching;
        private set
        {
            if (_isSearching != value)
            {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
                UpdateSearchUiState();
            }
        }
    }

    public bool IsSearchEmptyPromptVisible => _searchUi.IsSearchEmptyPromptVisible;
    public bool IsSearchFilteringVisible => _searchUi.IsSearchFilteringVisible;
    public bool IsSearchResultsListVisible => _searchUi.IsSearchResultsListVisible;
    public bool IsSearchNoResultsVisible => _searchUi.IsSearchNoResultsVisible;
}
