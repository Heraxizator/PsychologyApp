using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Services.QuotService;

public sealed class QuotService(IQuotRepository quotRepository, IQuotContentProvider quotContentProvider) : IQuotService
{
    public async Task AddSingleAsync(QuotDTO quotDTO, CancellationToken cancellationToken = default)
    {
        Quot quot = QuotMapper.GetQuot(quotDTO);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    public async Task<IEnumerable<QuotDTO>> GetAllAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<Quot> quots = await quotRepository.GetUnreadLatestAsync(count, cancellationToken);
        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<QuotDTO> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        Quot quot = await quotRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатром {id} не найдена");

        return QuotMapper.GetQuotDTO(quot);
    }

    public async Task LoadSingleAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        if (seeds.Count == 0)
        {
            throw new InvalidOperationException("Embedded quote catalog is empty.");
        }

        QuotSeed seed = seeds[Random.Shared.Next(seeds.Count)];
        Quot quot = Quot.Create(seed.Author, seed.Text, seed.Theme, isReaded: false, isFavourite: false);
        await quotRepository.AddAsync(quot, cancellationToken);
    }

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, CancellationToken cancellationToken = default)
    {
        Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатром {quotId} не найдена");

        quot.SetFavourite(isFavourite);
        await quotRepository.EditAsync(quot, cancellationToken);
    }

    public async Task MarkAsReadedAsync(long quotId, CancellationToken cancellationToken = default)
    {
        Quot quot = await quotRepository.GetByIdAsync(quotId, cancellationToken)
            ?? throw new QuotNotFoundException($"Цитата с идентификатором {quotId} не найдена");

        quot.MarkAsReaded();
        await quotRepository.EditAsync(quot, cancellationToken);
    }
}
