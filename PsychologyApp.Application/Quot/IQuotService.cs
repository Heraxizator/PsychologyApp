using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Quot;

public interface IQuotService
{
    Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuotDTO>> GetUnreadAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuotDTO>> GetUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default);

    Task EnsureThemedQuotesInFeedAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default);

    Task<bool> TryLoadThemedSingleAsync(
        IReadOnlyList<string> themes,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<QuotDTO>> GetByThemeAsync(string theme, int count, CancellationToken cancellationToken = default);
    Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddSingleAsync(QuotDTO quotDTO, CancellationToken cancellationToken = default);
    Task<bool> TryLoadSingleAsync(CancellationToken cancellationToken = default);
    Task LoadSingleAsync(CancellationToken cancellationToken = default);
    Task ReseedFeedAsync(int count, CancellationToken cancellationToken = default);
    Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default);
    Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default);
    Task<IEnumerable<QuotDTO>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default);
    Task<bool> IsAllCaughtUpAsync(CancellationToken cancellationToken = default);
    Task ResetReadStateAsync(CancellationToken cancellationToken = default);
    Task<QuotDTO?> GetDailyQuoteAsync(DateOnly date, CancellationToken cancellationToken = default);
}
