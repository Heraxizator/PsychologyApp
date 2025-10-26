using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Application.Services.ReasonService;

public sealed class ReasonService : IReasonService
{
    public async Task<IEnumerable<ReasonDTO>> GetReasonsAsync(int page, int perInPage, int cancelTimeout = 15000)
    {
        using CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        IEnumerable<Reason> reasons = await ReasonExtension.LoadReasonsAsync(cancellationTokenSource.Token);

        return reasons.Skip(page).Take(perInPage).Select(ReasonMapper.GetReasonDTO);
    }
}
