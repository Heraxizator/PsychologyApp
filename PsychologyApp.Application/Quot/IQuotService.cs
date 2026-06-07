using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.QuotService;

public interface IQuotService
{
    Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default);
    Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddSingleAsync(QuotDTO quotDTO, CancellationToken cancellationToken = default);
    Task LoadSingleAsync(CancellationToken cancellationToken = default);
    Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default);
    Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuotDTO>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default);
}
