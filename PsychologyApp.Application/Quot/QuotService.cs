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
        await quotRepository.AddAsync(quot, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetLatestAsync(count, cancellationToken).ConfigureAwait(false);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<IEnumerable<QuotDTO>> GetUnreadAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetUnreadLatestAsync(count, cancellationToken).ConfigureAwait(false);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<IEnumerable<QuotDTO>> GetUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetUnreadByThemesAsync(themes, count, cancellationToken).ConfigureAwait(false);
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

        int unreadCount = await CountUnreadByThemesAsync(themes, count, cancellationToken).ConfigureAwait(false);
        int needed = count - unreadCount;
        if (needed <= 0)
        {
            return;
        }

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken).ConfigureAwait(false);
        List<global::PsychologyApp.Domain.Entities.Quot> themedQuots = [];
        List<QuotSeed> themedPool = CreateThemedPool(themes, context);
        for (int i = 0; i < needed; i++)
        {
            if (TakeUnknown(context, themedPool) is not { } seed)
            {
                break;
            }

            themedQuots.Add(CreateQuotFromSeed(seed));
        }

        if (themedQuots.Count > 0)
        {
            await quotRepository.AddManyAsync(themedQuots, cancellationToken).ConfigureAwait(false);
        }

        unreadCount = await CountUnreadByThemesAsync(themes, count, cancellationToken).ConfigureAwait(false);
        needed = count - unreadCount;
        if (needed <= 0)
        {
            return;
        }

        await RestoreThemedQuotesAsUnreadAsync(themes, needed, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> TryLoadThemedSingleAsync(
        IReadOnlyList<string> themes,
        CancellationToken cancellationToken = default)
    {
        if (themes.Count == 0)
        {
            return false;
        }

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken).ConfigureAwait(false);
        if (TakeUnknown(context, CreateThemedPool(themes, context)) is { } seed)
        {
            await quotRepository.AddManyAsync([CreateQuotFromSeed(seed)], cancellationToken).ConfigureAwait(false);
            return true;
        }

        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> readThemed =
            await quotRepository.GetReadByThemesAsync(themes, 1, cancellationToken).ConfigureAwait(false);
        global::PsychologyApp.Domain.Entities.Quot? candidate = readThemed.FirstOrDefault();
        if (candidate is null)
        {
            return false;
        }

        candidate.MarkAsUnread();
        await quotRepository.EditAsync(candidate, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<IEnumerable<QuotDTO>> GetByThemeAsync(
        string theme,
        int count,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetByThemeAsync(theme, count, cancellationToken).ConfigureAwait(false);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {id} не найдена");

        return QuotMapper.GetQuotDTO(quot);
    }

    public async Task<bool> TryLoadSingleAsync(CancellationToken cancellationToken = default)
    {
        int beforeCount = await quotRepository.CountAllAsync(cancellationToken).ConfigureAwait(false);
        await AddRandomQuotesAsync(1, cancellationToken).ConfigureAwait(false);
        int afterCount = await quotRepository.CountAllAsync(cancellationToken).ConfigureAwait(false);
        return afterCount > beforeCount;
    }

    public Task LoadSingleAsync(CancellationToken cancellationToken = default) =>
        AddRandomQuotesAsync(1, cancellationToken);

    public async Task ReseedFeedAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            return;
        }

        IReadOnlySet<string> favoriteTexts = await CollectFavoriteTextsForReseedAsync(cancellationToken).ConfigureAwait(false);
        await quotRepository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        await AddRandomQuotesAsync(count, cancellationToken, loadExistingFromDatabase: false).ConfigureAwait(false);

        foreach (string text in favoriteTexts)
        {
            await EnsureFavoriteByTextAsync(text, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken).ConfigureAwait(false)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.SetFavourite(isFavourite);
        await quotRepository.EditAsync(quot, cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(quot.Text))
        {
            return;
        }

        if (isFavourite)
        {
            await favoriteTextStore.AddTextAsync(quot.Text, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await favoriteTextStore.RemoveTextAsync(quot.Text, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken).ConfigureAwait(false)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.MarkAsReaded();
        await quotRepository.EditAsync(quot, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<QuotDTO>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots =
            await quotRepository.GetFavouritesAsync(count, cancellationToken).ConfigureAwait(false);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<bool> IsAllCaughtUpAsync(CancellationToken cancellationToken = default)
    {
        if (await quotRepository.CountUnreadAsync(cancellationToken).ConfigureAwait(false) > 0)
        {
            return false;
        }

        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken).ConfigureAwait(false);
        HashSet<string> knownTexts = new(existingTexts ?? Array.Empty<string>(), StringComparer.Ordinal);
        return seeds.All(seed => knownTexts.Contains(seed.Text));
    }

    public Task ResetReadStateAsync(CancellationToken cancellationToken = default) =>
        quotRepository.ResetReadStateAsync(cancellationToken);

    public async Task<QuotDTO?> GetDailyQuoteAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        int catalogCount = await catalogLookup.GetCountAsync(cancellationToken).ConfigureAwait(false);
        if (catalogCount <= 0)
        {
            return null;
        }

        int index = QuotePersonalizationPolicy.ResolveDailyQuoteIndex(date, catalogCount);
        QuotSeed? seed = await catalogLookup.GetSeedAtAsync(index, cancellationToken).ConfigureAwait(false);
        if (seed is null)
        {
            return null;
        }

        global::PsychologyApp.Domain.Entities.Quot? existing =
            await quotRepository.GetByTextAsync(seed.Text, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
        {
            return QuotMapper.GetQuotDTO(existing);
        }

        global::PsychologyApp.Domain.Entities.Quot quot = CreateQuotFromSeed(seed);
        await quotRepository.AddAsync(quot, cancellationToken).ConfigureAwait(false);

        global::PsychologyApp.Domain.Entities.Quot? inserted =
            await quotRepository.GetByTextAsync(seed.Text, cancellationToken).ConfigureAwait(false);

        return inserted is not null ? QuotMapper.GetQuotDTO(inserted) : null;
    }

    private sealed class QuoteSeedContext(IReadOnlyList<QuotSeed> seeds, HashSet<string> knownTexts)
    {
        public IReadOnlyList<QuotSeed> Seeds { get; } = seeds;

        public HashSet<string> KnownTexts { get; } = knownTexts;
    }

    private async Task<QuoteSeedContext> CreateSeedContextAsync(
        CancellationToken cancellationToken,
        bool loadExistingFromDatabase = true)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken).ConfigureAwait(false);
        if (seeds.Count == 0)
        {
            throw new InvalidOperationException("Embedded quote catalog is empty.");
        }

        HashSet<string> knownTexts = new(StringComparer.Ordinal);
        if (loadExistingFromDatabase)
        {
            IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken).ConfigureAwait(false);
            foreach (string text in existingTexts ?? Array.Empty<string>())
            {
                knownTexts.Add(text);
            }
        }

        return new QuoteSeedContext(seeds, knownTexts);
    }

    private async Task AddRandomQuotesAsync(
        int count,
        CancellationToken cancellationToken,
        bool loadExistingFromDatabase = true)
    {
        if (count <= 0)
        {
            return;
        }

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken, loadExistingFromDatabase).ConfigureAwait(false);
        List<global::PsychologyApp.Domain.Entities.Quot> quots = [];
        List<QuotSeed> available = context.Seeds
            .Where(seed => !context.KnownTexts.Contains(seed.Text))
            .ToList();

        for (int i = 0; i < count; i++)
        {
            QuotSeed? seed = await PickRandomSeedAsync(context, available, cancellationToken).ConfigureAwait(false);
            if (seed is null)
            {
                break;
            }

            quots.Add(CreateQuotFromSeed(seed));
        }

        await quotRepository.AddManyAsync(quots, cancellationToken).ConfigureAwait(false);
    }

    // Draws from the shared pool by swap-remove, so a page of N quotes no longer re-filters the whole catalog N times.
    private async Task<QuotSeed?> PickRandomSeedAsync(
        QuoteSeedContext context,
        List<QuotSeed> available,
        CancellationToken cancellationToken)
    {
        if (TakeUnknown(context, available) is { } seed)
        {
            return seed;
        }

        await quotRepository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        context.KnownTexts.Clear();
        available.AddRange(context.Seeds);
        return TakeUnknown(context, available);
    }

    private static QuotSeed? TakeUnknown(QuoteSeedContext context, List<QuotSeed> available)
    {
        while (available.Count > 0)
        {
            int index = Random.Shared.Next(available.Count);
            QuotSeed candidate = available[index];
            available[index] = available[^1];
            available.RemoveAt(available.Count - 1);
            if (context.KnownTexts.Add(candidate.Text))
            {
                return candidate;
            }
        }

        return null;
    }

    private static global::PsychologyApp.Domain.Entities.Quot CreateQuotFromSeed(
        QuotSeed seed,
        bool isFavourite = false) =>
        global::PsychologyApp.Domain.Entities.Quot.Create(
            seed.Author,
            seed.Text,
            seed.Theme,
            isReaded: false,
            isFavourite: isFavourite);

    private async Task<IReadOnlySet<string>> CollectFavoriteTextsForReseedAsync(CancellationToken cancellationToken)
    {
        HashSet<string> texts = new(StringComparer.Ordinal);

        foreach (string text in await quotRepository.GetFavoriteTextsAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                texts.Add(text);
            }
        }

        foreach (string text in await favoriteTextStore.GetTextsAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                texts.Add(text);
            }
        }

        await favoriteTextStore.SaveTextsAsync(texts, cancellationToken).ConfigureAwait(false);
        return texts;
    }

    private async Task EnsureFavoriteByTextAsync(string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        global::PsychologyApp.Domain.Entities.Quot? existing =
            await quotRepository.GetByTextAsync(text, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            int? catalogIndex = await catalogLookup.TryGetIndexByTextAsync(text, cancellationToken).ConfigureAwait(false);
            if (catalogIndex is null)
            {
                return;
            }

            QuotSeed? seed = await catalogLookup.GetSeedAtAsync(catalogIndex.Value, cancellationToken).ConfigureAwait(false);
            if (seed is null)
            {
                return;
            }

            global::PsychologyApp.Domain.Entities.Quot quot = CreateQuotFromSeed(seed, isFavourite: true);
            await quotRepository.AddAsync(quot, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (existing.IsFavourite)
        {
            return;
        }

        existing.SetFavourite(true);
        await quotRepository.EditAsync(existing, cancellationToken).ConfigureAwait(false);
    }

    private async Task<int> CountUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> unread =
            await quotRepository.GetUnreadByThemesAsync(themes, count, cancellationToken).ConfigureAwait(false);
        return unread.Count();
    }

    private static List<QuotSeed> CreateThemedPool(IReadOnlyList<string> themes, QuoteSeedContext context)
    {
        HashSet<string> themeSet = new(themes, StringComparer.OrdinalIgnoreCase);
        return context.Seeds
            .Where(candidate => themeSet.Contains(candidate.Theme) && !context.KnownTexts.Contains(candidate.Text))
            .ToList();
    }

    private async Task RestoreThemedQuotesAsUnreadAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> readThemed =
            await quotRepository.GetReadByThemesAsync(themes, count, cancellationToken).ConfigureAwait(false);

        foreach (global::PsychologyApp.Domain.Entities.Quot quot in readThemed)
        {
            if (!quot.IsReaded)
            {
                continue;
            }

            quot.MarkAsUnread();
            await quotRepository.EditAsync(quot, cancellationToken).ConfigureAwait(false);
        }
    }
}
