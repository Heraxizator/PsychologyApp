using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Exceptions;

namespace PsychologyApp.Application.Quot;

public sealed class QuotService(
    IQuotRepository quotRepository,
    IQuotContentProvider quotContentProvider,
    IQuoteCatalogLookup catalogLookup,
    IFavoriteQuoteTextStore favoriteTextStore) : IQuotService
{
    public async Task AddSingleAsync(QuotDTO quotDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = QuotMapper.GetQuot(quotDTO);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    public async Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetLatestAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<IEnumerable<QuotDTO>> GetUnreadAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetUnreadLatestAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<IEnumerable<QuotDTO>> GetUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetUnreadByThemesAsync(themes, count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task EnsureThemedQuotesInFeedAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default)
    {
        if (themes.Count == 0 || count <= 0)
        {
            return;
        }

        int unreadCount = await CountUnreadByThemesAsync(themes, count, cancellationToken);
        int needed = count - unreadCount;
        if (needed <= 0)
        {
            return;
        }

        for (int i = 0; i < needed; i++)
        {
            if (!await TryAddThemedQuoteFromCatalogAsync(themes, cancellationToken))
            {
                break;
            }
        }

        unreadCount = await CountUnreadByThemesAsync(themes, count, cancellationToken);
        needed = count - unreadCount;
        if (needed <= 0)
        {
            return;
        }

        await RestoreThemedQuotesAsUnreadAsync(themes, needed, cancellationToken);
    }

    public async Task<bool> TryLoadThemedSingleAsync(
        IReadOnlyList<string> themes,
        CancellationToken cancellationToken = default)
    {
        if (themes.Count == 0)
        {
            return false;
        }

        if (await TryAddThemedQuoteFromCatalogAsync(themes, cancellationToken))
        {
            return true;
        }

        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> readThemed =
            await quotRepository.GetReadByThemesAsync(themes, 1, cancellationToken);
        global::PsychologyApp.Domain.Entities.Quot? candidate = readThemed.FirstOrDefault();
        if (candidate is null)
        {
            return false;
        }

        candidate.MarkAsUnread();
        await quotRepository.EditAsync(candidate, cancellationToken);
        return true;
    }

    public async Task<IEnumerable<QuotDTO>> GetByThemeAsync(
        string theme,
        int count,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetByThemeAsync(theme, count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {id} не найдена");

        return QuotMapper.GetQuotDTO(quot);
    }

    public async Task<bool> TryLoadSingleAsync(CancellationToken cancellationToken = default)
    {
        int beforeCount = await quotRepository.CountAllAsync(cancellationToken);
        await AddRandomQuoteAsync(cancellationToken);
        int afterCount = await quotRepository.CountAllAsync(cancellationToken);
        return afterCount > beforeCount;
    }

    public Task LoadSingleAsync(CancellationToken cancellationToken = default) =>
        AddRandomQuoteAsync(cancellationToken);

    public async Task ReseedFeedAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            return;
        }

        IReadOnlySet<string> favoriteTexts = await CollectFavoriteTextsForReseedAsync(cancellationToken);
        await quotRepository.DeleteAllAsync(cancellationToken);

        for (int i = 0; i < count; i++)
        {
            await AddRandomQuoteAsync(cancellationToken);
        }

        foreach (string text in favoriteTexts)
        {
            await EnsureFavoriteByTextAsync(text, cancellationToken);
        }
    }

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.SetFavourite(isFavourite);
        await quotRepository.EditAsync(quot, cancellationToken);

        if (string.IsNullOrWhiteSpace(quot.Text))
        {
            return;
        }

        if (isFavourite)
        {
            await favoriteTextStore.AddTextAsync(quot.Text, cancellationToken);
        }
        else
        {
            await favoriteTextStore.RemoveTextAsync(quot.Text, cancellationToken);
        }
    }

    public async Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.MarkAsReaded();
        await quotRepository.EditAsync(quot, cancellationToken);
    }

    public async Task<IEnumerable<QuotDTO>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetFavouritesAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<bool> IsAllCaughtUpAsync(CancellationToken cancellationToken = default)
    {
        if (await quotRepository.CountUnreadAsync(cancellationToken) > 0)
        {
            return false;
        }

        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken);
        HashSet<string> knownTexts = new(existingTexts ?? Array.Empty<string>(), StringComparer.Ordinal);
        return seeds.All(seed => knownTexts.Contains(seed.Text));
    }

    public Task ResetReadStateAsync(CancellationToken cancellationToken = default) =>
        quotRepository.ResetReadStateAsync(cancellationToken);

    public async Task<QuotDTO?> GetDailyQuoteAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        int catalogCount = await catalogLookup.GetCountAsync(cancellationToken);
        if (catalogCount <= 0)
        {
            return null;
        }

        int index = QuotePersonalizationPolicy.ResolveDailyQuoteIndex(date, catalogCount);
        QuotSeed? seed = await catalogLookup.GetSeedAtAsync(index, cancellationToken);
        if (seed is null)
        {
            return null;
        }

        global::PsychologyApp.Domain.Entities.Quot? existing =
            await quotRepository.GetByTextAsync(seed.Text, cancellationToken);
        if (existing is not null)
        {
            return QuotMapper.GetQuotDTO(existing);
        }

        global::PsychologyApp.Domain.Entities.Quot quot = global::PsychologyApp.Domain.Entities.Quot.Create(
            seed.Author,
            seed.Text,
            seed.Theme,
            isReaded: false,
            isFavourite: false);
        await quotRepository.AddAsync(quot, cancellationToken);
        return QuotMapper.GetQuotDTO(quot);
    }

    private async Task<IReadOnlySet<string>> CollectFavoriteTextsForReseedAsync(CancellationToken cancellationToken)
    {
        HashSet<string> texts = new(StringComparer.Ordinal);

        foreach (string text in await quotRepository.GetFavoriteTextsAsync(cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                texts.Add(text);
            }
        }

        foreach (string text in await favoriteTextStore.GetTextsAsync(cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                texts.Add(text);
            }
        }

        await favoriteTextStore.SaveTextsAsync(texts, cancellationToken);
        return texts;
    }

    private async Task EnsureFavoriteByTextAsync(string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        global::PsychologyApp.Domain.Entities.Quot? existing =
            await quotRepository.GetByTextAsync(text, cancellationToken);
        if (existing is null)
        {
            int? catalogIndex = await catalogLookup.TryGetIndexByTextAsync(text, cancellationToken);
            if (catalogIndex is null)
            {
                return;
            }

            QuotSeed? seed = await catalogLookup.GetSeedAtAsync(catalogIndex.Value, cancellationToken);
            if (seed is null)
            {
                return;
            }

            global::PsychologyApp.Domain.Entities.Quot quot = global::PsychologyApp.Domain.Entities.Quot.Create(
                seed.Author,
                seed.Text,
                seed.Theme,
                isReaded: false,
                isFavourite: true);
            await quotRepository.AddAsync(quot, cancellationToken);
            return;
        }

        if (existing.IsFavourite)
        {
            return;
        }

        existing.SetFavourite(true);
        await quotRepository.EditAsync(existing, cancellationToken);
    }

    private async Task AddRandomQuoteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        if (seeds.Count == 0)
        {
            throw new InvalidOperationException("Embedded quote catalog is empty.");
        }

        IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken);
        HashSet<string> knownTexts = new(existingTexts ?? Array.Empty<string>(), StringComparer.Ordinal);

        List<QuotSeed> available = seeds
            .Where(seed => !knownTexts.Contains(seed.Text))
            .ToList();

        if (available.Count == 0)
        {
            await quotRepository.DeleteAllAsync(cancellationToken);
            available = seeds.ToList();
        }

        QuotSeed seed = available[Random.Shared.Next(available.Count)];
        global::PsychologyApp.Domain.Entities.Quot quot = global::PsychologyApp.Domain.Entities.Quot.Create(
            seed.Author,
            seed.Text,
            seed.Theme,
            isReaded: false,
            isFavourite: false);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    private async Task<int> CountUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> unread =
            await quotRepository.GetUnreadByThemesAsync(themes, count, cancellationToken);
        return unread.Count();
    }

    private async Task<bool> TryAddThemedQuoteFromCatalogAsync(
        IReadOnlyList<string> themes,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        if (seeds.Count == 0)
        {
            return false;
        }

        HashSet<string> themeSet = new(themes, StringComparer.OrdinalIgnoreCase);
        IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken);
        HashSet<string> knownTexts = new(existingTexts ?? Array.Empty<string>(), StringComparer.Ordinal);

        List<QuotSeed> available = seeds
            .Where(seed => themeSet.Contains(seed.Theme) && !knownTexts.Contains(seed.Text))
            .ToList();

        if (available.Count == 0)
        {
            return false;
        }

        QuotSeed seed = available[Random.Shared.Next(available.Count)];
        global::PsychologyApp.Domain.Entities.Quot quot = global::PsychologyApp.Domain.Entities.Quot.Create(
            seed.Author,
            seed.Text,
            seed.Theme,
            isReaded: false,
            isFavourite: false);
        await quotRepository.AddAsync(quot, cancellationToken);
        return true;
    }

    private async Task RestoreThemedQuotesAsUnreadAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> readThemed =
            await quotRepository.GetReadByThemesAsync(themes, count, cancellationToken);

        foreach (global::PsychologyApp.Domain.Entities.Quot quot in readThemed)
        {
            if (!quot.IsReaded)
            {
                continue;
            }

            quot.MarkAsUnread();
            await quotRepository.EditAsync(quot, cancellationToken);
        }
    }
}
