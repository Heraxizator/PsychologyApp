using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Application.Services.ReasonService;

public sealed class ReasonService : IReasonService
{
    public async Task<IEnumerable<ReasonDTO>> GetReasons(int count, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        IEnumerable<Reason> reasons = (await Database.ReasonRepository.GetAllAsync()).Take(count).ToList();

        return reasons.Select(ReasonMapper.GetReasonDTO);
    }
}
