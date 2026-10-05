using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Reason;

public sealed class ReasonSearchService(IReasonContentProvider reasonContentProvider) : IReasonSearchService
{
    public async Task<IReadOnlyList<ReasonDTO>> LoadReasonsAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Reason> reasons =
            await reasonContentProvider.LoadReasonsAsync(cancellationToken).ConfigureAwait(false);

        return reasons.Select(ReasonMapper.GetReasonDTO).ToList();
    }

    public IReadOnlyList<RankedReason> Search(IReadOnlyList<ReasonDTO> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        IReadOnlyList<string> stems = SearchText.Stems(query);
        if (stems.Count == 0)
        {
            return [];
        }

        return source
            .Select(reason => (Reason: reason, Score: TryGetMatchScore(reason, stems)))
            .Where(match => match.Score is not null)
            .Select(match => new RankedReason(match.Reason, match.Score!.Value))
            .OrderByDescending(pair => pair.MatchScore)
            .ThenBy(pair => pair.Reason.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int? TryGetMatchScore(ReasonDTO reason, IReadOnlyList<string> stems)
    {
        if (SearchText.Matches(reason.Title, stems))
        {
            return 3;
        }

        if (SearchText.Matches(reason.Subtitle, stems))
        {
            return 2;
        }

        return SearchText.Matches(reason.Solution, stems) ? 1 : null;
    }
}
