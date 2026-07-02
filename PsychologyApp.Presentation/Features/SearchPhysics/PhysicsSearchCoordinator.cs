using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Physics;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.SearchPhysics;

public sealed class PhysicsSearchPageSlice
{
    public required IReadOnlyList<PhysicsReasonItem> Items { get; init; }
    public required int LoadedCount { get; init; }
}

public sealed class PhysicsSearchCoordinator(
    IReasonSearchService reasonSearchService,
    TechniqueCatalogGateway techniqueCatalog,
    ITechniqueRecommendationService techniqueRecommendationService)
{
    public const int SearchDebounceMs = 300;
    public const int SearchResultsPageSize = 20;

    public CancellationTokenSource ScheduleDebouncedSearch(
        CancellationTokenSource? existing,
        string searchText,
        Action<CancellationToken> onDebounced)
    {
        existing?.Cancel();
        existing?.Dispose();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new CancellationTokenSource();
        }

        CancellationTokenSource cts = new();
        _ = RunDebouncedSearchAsync(cts.Token, onDebounced);
        return cts;
    }

    public void CancelPendingSearch(ref CancellationTokenSource? cts)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public PhysicsSearchPageSlice CreateInitialPage(IReadOnlyList<PhysicsReasonItem> matches) =>
        CreatePageSlice(matches, startIndex: 0);

    public PhysicsSearchPageSlice LoadNextPage(IReadOnlyList<PhysicsReasonItem> matches, int loadedCount)
    {
        if (loadedCount >= matches.Count)
        {
            return new PhysicsSearchPageSlice { Items = [], LoadedCount = loadedCount };
        }

        return CreatePageSlice(matches, loadedCount);
    }

    public Task<IReadOnlyList<PhysicsReasonItem>> SearchAsync(
        IReadOnlyList<ReasonDTO> reasons,
        string searchText,
        INavigationService navigationService,
        Func<ReasonDTO, IReadOnlyList<PhysicsTechniqueSuggestion>, string, PhysicsReasonItem> createItem,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<TechniqueId> techniqueIds = techniqueRecommendationService.RecommendForSomaticQuery(searchText);
        List<PhysicsTechniqueSuggestion> suggestions = techniqueIds
            .Select(id =>
            {
                TechniqueDefinition definition = techniqueCatalog.Get(id);
                return new PhysicsTechniqueSuggestion
                {
                    Title = definition.PageName,
                    OpenCommand = new AsyncCommand(() => navigationService.GoToTechniqueAsync(id))
                };
            })
            .ToList();

        List<PhysicsReasonItem> results = reasonSearchService.Search(reasons, searchText)
            .Select(pair => createItem(pair.Reason, suggestions, searchText))
            .ToList();

        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<PhysicsReasonItem>>(results);
    }

    private static async Task RunDebouncedSearchAsync(CancellationToken token, Action<CancellationToken> onDebounced)
    {
        try
        {
            await Task.Delay(SearchDebounceMs, token);
            onDebounced(token);
        }
        catch (TaskCanceledException)
        {
        }
    }

    private static PhysicsSearchPageSlice CreatePageSlice(IReadOnlyList<PhysicsReasonItem> matches, int startIndex)
    {
        List<PhysicsReasonItem> page = matches
            .Skip(startIndex)
            .Take(SearchResultsPageSize)
            .ToList();

        return new PhysicsSearchPageSlice
        {
            Items = page,
            LoadedCount = startIndex + page.Count
        };
    }
}
