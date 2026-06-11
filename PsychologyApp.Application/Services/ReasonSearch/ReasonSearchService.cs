using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.ReasonSearch;

public sealed class ReasonSearchService(IReasonContentProvider reasonContentProvider) : IReasonSearchService
{
    public async Task<IReadOnlyList<ReasonDTO>> LoadReasonsAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Entities.Reason> reasons =
            await reasonContentProvider.LoadReasonsAsync(cancellationToken);

        return reasons.Select(ReasonMapper.GetReasonDTO).ToList();
    }

    public IReadOnlyList<RankedReason> Search(IReadOnlyList<ReasonDTO> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        return source
            .Where(reason => MatchesSearch(reason, query))
            .Select(reason => new RankedReason(reason, RankMatch(reason, query)))
            .OrderByDescending(pair => pair.MatchScore)
            .ThenBy(pair => pair.Reason.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool MatchesSearch(ReasonDTO reason, string searchText) =>
        reason.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true
        || reason.Subtitle?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true
        || reason.Solution?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true;

    private static int RankMatch(ReasonDTO reason, string searchText)
    {
        if (reason.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true)
        {
            return 3;
        }

        if (reason.Subtitle?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true)
        {
            return 2;
        }

        return reason.Solution?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true ? 1 : 0;
    }
}
