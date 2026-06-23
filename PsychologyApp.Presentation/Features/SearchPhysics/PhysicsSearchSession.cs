using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Physics;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.SearchPhysics;

public sealed class PhysicsSearchSession(PhysicsSearchCoordinator searchCoordinator)
{
    private List<PhysicsReasonItem> _searchMatches = [];
    private int _loadedSearchCount;

    public IReadOnlyList<ReasonDTO> Reasons { get; private set; } = [];

    public int LoadedSearchCount => _loadedSearchCount;

    public void ResetSearchMatches()
    {
        _searchMatches = [];
        _loadedSearchCount = 0;
    }

    public async Task<IReadOnlyList<ReasonDTO>> LoadReasonsAsync(
        IReasonSearchService reasonSearchService,
        IDatabaseReadySignal databaseReadySignal,
        CancellationToken cancellationToken)
    {
        await databaseReadySignal.WaitAsync(cancellationToken);
        Reasons = (await reasonSearchService.LoadReasonsAsync(cancellationToken)).ToList();
        return Reasons;
    }

    public CancellationTokenSource CreateInitCancellationSource(IOptions<AppSettings> settings) =>
        OperationCancellation.CreateLargeTimeoutSource(settings);

    public async Task<PhysicsSearchPageSlice?> SearchAsync(
        string searchText,
        string currentSearchText,
        INavigationService navigationService,
        Func<ReasonDTO, IReadOnlyList<PhysicsTechniqueSuggestion>, string, PhysicsReasonItem> createItem,
        CancellationToken cancellationToken)
    {
        List<PhysicsReasonItem> matches = (await searchCoordinator.SearchAsync(
            Reasons,
            searchText,
            navigationService,
            createItem,
            cancellationToken)).ToList();

        if (cancellationToken.IsCancellationRequested || !string.Equals(currentSearchText, searchText, StringComparison.Ordinal))
        {
            return null;
        }

        _searchMatches = matches;
        PhysicsSearchPageSlice page = searchCoordinator.CreateInitialPage(_searchMatches);
        _loadedSearchCount = page.LoadedCount;
        return page;
    }

    public PhysicsSearchPageSlice LoadNextPage()
    {
        PhysicsSearchPageSlice page = searchCoordinator.LoadNextPage(_searchMatches, _loadedSearchCount);
        _loadedSearchCount = page.LoadedCount;
        return page;
    }
}
