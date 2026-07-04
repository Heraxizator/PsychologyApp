using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IQuotRepository : IReadRepository<global::PsychologyApp.Domain.Entities.Quot>, IWriteRepository<global::PsychologyApp.Domain.Entities.Quot>
{
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetUnreadLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetReadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetByThemeAsync(
        string theme,
        int count,
        CancellationToken cancellationToken = default);
    Task<global::PsychologyApp.Domain.Entities.Quot?> GetByTextAsync(
        string text,
        CancellationToken cancellationToken = default);
    Task<int> CountAllAsync(CancellationToken cancellationToken = default);
    Task<int> CountUnreadAsync(CancellationToken cancellationToken = default);
    Task ResetReadStateAsync(CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetExistingTextsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetFavoriteTextsAsync(CancellationToken cancellationToken = default);

    Task AddManyAsync(IReadOnlyList<global::PsychologyApp.Domain.Entities.Quot> quots, CancellationToken cancellationToken = default);
}
