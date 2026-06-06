using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.ReasonService;

public interface IReasonService
{
    Task<IEnumerable<ReasonDTO>> GetReasonsAsync(int page, int perInPage, CancellationToken cancellationToken = default);
}
