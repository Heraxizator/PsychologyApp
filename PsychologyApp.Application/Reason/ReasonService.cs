using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Services.ReasonService;

public sealed class ReasonService(IReasonContentProvider reasonContentProvider) : IReasonService
{
    public async Task<IEnumerable<ReasonDTO>> GetReasonsAsync(int page, int perInPage, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Reason> reasons = await reasonContentProvider.GetPageAsync(page, perInPage, cancellationToken);
        return reasons.Select(ReasonMapper.GetReasonDTO);
    }
}
