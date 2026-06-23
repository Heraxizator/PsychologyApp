namespace PsychologyApp.Presentation.Features.SearchPhysics;

public sealed class PhysicsSearchUiSnapshot
{
    public bool IsSearchEmptyPromptVisible { get; init; }
    public bool IsSearchFilteringVisible { get; init; }
    public bool IsSearchResultsListVisible { get; init; }
    public bool IsSearchNoResultsVisible { get; init; }
}

public static class PhysicsSearchUiState
{
    public static PhysicsSearchUiSnapshot Resolve(
        bool isDone,
        string searchText,
        bool isSearching,
        int resultCount)
    {
        bool hasText = !string.IsNullOrWhiteSpace(searchText);
        bool hasResults = resultCount > 0;

        return new PhysicsSearchUiSnapshot
        {
            IsSearchEmptyPromptVisible = isDone && !hasText && !isSearching,
            IsSearchFilteringVisible = isDone && hasText && isSearching,
            IsSearchResultsListVisible = isDone && hasText && !isSearching && hasResults,
            IsSearchNoResultsVisible = isDone && hasText && !isSearching && !hasResults
        };
    }
}
