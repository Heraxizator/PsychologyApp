using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Quot;

public sealed class QuotService(IQuotRepository quotRepository, IQuotContentProvider quotContentProvider) : IQuotService
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
            ?? throw new QuotNotFoundException($"Р¦РёС‚Р°С‚Р° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {id} РЅРµ РЅР°Р№РґРµРЅР°");

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

        QuotSeed seed = seeds[Random.Shared.Next(seeds.Count)];
        global::PsychologyApp.Domain.Entities.Quot quot = global::PsychologyApp.Domain.Entities.Quot.Create(seed.Author, seed.Text, seed.Theme, isReaded: false, isFavourite: false);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Р¦РёС‚Р°С‚Р° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {quotId} РЅРµ РЅР°Р№РґРµРЅР°");

        quot.SetFavourite(isFavourite);
        await quotRepository.EditAsync(quot, cancellationToken);
    }

    public async Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Р¦РёС‚Р°С‚Р° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {quotId} РЅРµ РЅР°Р№РґРµРЅР°");

        quot.MarkAsReaded();
        await quotRepository.EditAsync(quot, cancellationToken);
    }

    public async Task<IEnumerable<QuotDTO>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Quot> quots = await quotRepository.GetFavouritesAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }
}
