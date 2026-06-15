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
            .Select(reason => (Reason: reason, Score: TryGetMatchScore(reason, query)))
            .Where(match => match.Score is not null)
            .Select(match => new RankedReason(match.Reason, match.Score!.Value))
            .OrderByDescending(pair => pair.MatchScore)
            .ThenBy(pair => pair.Reason.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int? TryGetMatchScore(ReasonDTO reason, string searchText)
    {
        if (reason.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true)
        {
            return 3;
        }

        if (reason.Subtitle?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true)
        {
            return 2;
        }

        return reason.Solution?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true ? 1 : null;
    }
}
