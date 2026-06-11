using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.ReasonSearch;

public sealed record RankedReason(ReasonDTO Reason, int MatchScore);

public interface IReasonSearchService
{
    Task<IReadOnlyList<ReasonDTO>> LoadReasonsAsync(CancellationToken cancellationToken = default);

    IReadOnlyList<RankedReason> Search(IReadOnlyList<ReasonDTO> source, string query);
}
