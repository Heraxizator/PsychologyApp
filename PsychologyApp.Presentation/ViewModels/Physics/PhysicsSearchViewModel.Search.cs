using Microsoft.Extensions.Logging;
using MvvmHelpers;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Physics;
using PsychologyApp.Presentation.Services.Physics;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public partial class PhysicsSearchViewModel
{
    private void DebouncedSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            UiThread.RunAsync(ClearSearchResults).FireAndForget();
            return;
        }

        IsSearching = true;
        _searchDebounceCts = _searchCoordinator.ScheduleDebouncedSearch(
            _searchDebounceCts,
            searchText,
            token => RunSearchAsync(searchText, token).FireAndForget());
    }

    private void UpdateSearchUiState()
    {
        PhysicsSearchUiSnapshot snapshot = PhysicsSearchUiState.Resolve(
            IsDone,
            SearchText,
            IsSearching,
            ResultsObservableCollection.Count);

        PhysicsSearchUiBinder.Apply(_searchUi, snapshot, name =>
        {
            OnPropertyChanged(name switch
            {
                nameof(PhysicsSearchUiBindings.IsSearchEmptyPromptVisible) => nameof(IsSearchEmptyPromptVisible),
                nameof(PhysicsSearchUiBindings.IsSearchFilteringVisible) => nameof(IsSearchFilteringVisible),
                nameof(PhysicsSearchUiBindings.IsSearchResultsListVisible) => nameof(IsSearchResultsListVisible),
                nameof(PhysicsSearchUiBindings.IsSearchNoResultsVisible) => nameof(IsSearchNoResultsVisible),
                _ => name
            });
        });
    }

    private void ExecuteSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ClearSearchResults();
            return;
        }

        CancelPendingSearch();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;
        IsSearching = true;
        RunSearchAsync(searchText, token).FireAndForget();
    }

    private void ClearSearchResults()
    {
        _searchSession.ResetSearchMatches();
        ResultsObservableCollection.Clear();
        IsSearching = false;
        UpdateSearchUiState();
    }

    private async Task RunSearchAsync(string searchText, CancellationToken cancellationToken)
    {
        try
        {
            PhysicsSearchPageSlice? page = await _searchSession.SearchAsync(
                searchText,
                SearchText,
                _navigationService,
                PhysicsReasonItemFactory.CreateExpandable,
                cancellationToken);

            if (page is null)
            {
                await ResetSearchingIfCurrentAsync(searchText);
                return;
            }

            await UiThread.RunAsync(() =>
            {
                ResultsObservableCollection.ReplaceRange(page.Items.ToList());
                IsSearching = false;
                UpdateSearchUiState();
            });
        }
        catch (OperationCanceledException)
        {
            await ResetSearchingIfCurrentAsync(searchText);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Physics search failed for query {SearchText}.", searchText);
            await ResetSearchingIfCurrentAsync(searchText);
        }
    }

    private Task ResetSearchingIfCurrentAsync(string searchText) =>
        UiThread.RunAsync(() =>
        {
            if (!string.Equals(SearchText, searchText, StringComparison.Ordinal))
            {
                return;
            }

            IsSearching = false;
            UpdateSearchUiState();
        });

    private void LoadMoreSearchResults()
    {
        PhysicsSearchPageSlice page = _searchSession.LoadNextPage();
        if (page.Items.Count == 0)
        {
            return;
        }

        ResultsObservableCollection.AddRange(page.Items.ToList());
    }
}
