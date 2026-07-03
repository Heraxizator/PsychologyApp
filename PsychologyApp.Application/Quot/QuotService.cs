using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Exceptions;

namespace PsychologyApp.Application.Quot;

public sealed class QuotService(
    IQuotRepository quotRepository,
    IQuotContentProvider quotContentProvider) : IQuotService
{
    public async Task AddSingleAsync(QuotDTO quotDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = QuotMapper.GetQuot(quotDTO);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    public async Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots = await quotRepository.GetLatestAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {id} не найдена");

        return QuotMapper.GetQuotDTO(quot);
    }

    public async Task LoadSingleAsync(CancellationToken cancellationToken = default)
    {
        await AddRandomQuoteAsync(cancellationToken);
    }

    public async Task ReseedFeedAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            return;
        }

        await quotRepository.DeleteAllAsync(cancellationToken);

        for (int i = 0; i < count; i++)
        {
            await AddRandomQuoteAsync(cancellationToken);
        }
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

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.SetFavourite(isFavourite);
        await quotRepository.EditAsync(quot, cancellationToken);
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
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots = await quotRepository.GetFavouritesAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }
}
