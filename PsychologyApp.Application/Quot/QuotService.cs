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

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken);
        for (int i = 0; i < needed; i++)
        {
            if (!await TryAddThemedQuoteFromCatalogAsync(themes, context, cancellationToken))
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

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken);
        if (await TryAddThemedQuoteFromCatalogAsync(themes, context, cancellationToken))
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
        await AddRandomQuotesAsync(1, cancellationToken);
        int afterCount = await quotRepository.CountAllAsync(cancellationToken);
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

        IReadOnlySet<string> favoriteTexts = await CollectFavoriteTextsForReseedAsync(cancellationToken);
        await quotRepository.DeleteAllAsync(cancellationToken);
        await AddRandomQuotesAsync(count, cancellationToken, loadExistingFromDatabase: false);

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

        global::PsychologyApp.Domain.Entities.Quot quot = CreateQuotFromSeed(seed);
        await quotRepository.AddAsync(quot, cancellationToken);
        return QuotMapper.GetQuotDTO(quot);
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
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        if (seeds.Count == 0)
        {
            throw new InvalidOperationException("Embedded quote catalog is empty.");
        }

        HashSet<string> knownTexts = new(StringComparer.Ordinal);
        if (loadExistingFromDatabase)
        {
            IReadOnlyList<string> existingTexts = await quotRepository.GetExistingTextsAsync(cancellationToken);
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

        QuoteSeedContext context = await CreateSeedContextAsync(cancellationToken, loadExistingFromDatabase);
        List<global::PsychologyApp.Domain.Entities.Quot> quots = [];

        for (int i = 0; i < count; i++)
        {
            QuotSeed? seed = await PickRandomSeedAsync(context, cancellationToken);
            if (seed is null)
            {
                break;
            }

            quots.Add(CreateQuotFromSeed(seed));
        }

        await quotRepository.AddManyAsync(quots, cancellationToken);
    }

    private async Task<QuotSeed?> PickRandomSeedAsync(
        QuoteSeedContext context,
        CancellationToken cancellationToken)
    {
        List<QuotSeed> available = context.Seeds
            .Where(seed => !context.KnownTexts.Contains(seed.Text))
            .ToList();

        if (available.Count == 0)
        {
            await quotRepository.DeleteAllAsync(cancellationToken);
            context.KnownTexts.Clear();
            available = context.Seeds.ToList();
        }

        if (available.Count == 0)
        {
            return null;
        }

        QuotSeed seed = available[Random.Shared.Next(available.Count)];
        context.KnownTexts.Add(seed.Text);
        return seed;
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

            global::PsychologyApp.Domain.Entities.Quot quot = CreateQuotFromSeed(seed, isFavourite: true);
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
        QuoteSeedContext context,
        CancellationToken cancellationToken)
    {
        HashSet<string> themeSet = new(themes, StringComparer.OrdinalIgnoreCase);
        List<QuotSeed> available = context.Seeds
            .Where(seed => themeSet.Contains(seed.Theme) && !context.KnownTexts.Contains(seed.Text))
            .ToList();

        if (available.Count == 0)
        {
            return false;
        }

        QuotSeed seed = available[Random.Shared.Next(available.Count)];
        context.KnownTexts.Add(seed.Text);
        await quotRepository.AddAsync(CreateQuotFromSeed(seed), cancellationToken);
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
