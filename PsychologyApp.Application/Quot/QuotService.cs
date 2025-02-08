using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;
using System.Threading;

namespace PsychologyApp.Application;

public sealed class QuotService : IQuotService
{
    public async Task AddAsync(QuotDTO quotDTO, int cancelTimeout)
    {
        Quot quot = QuotMapper.GetQuot(quotDTO);

        await Database.QuotRepository.AddAsync(quot, cancelTimeout);
    }

    public async Task<IEnumerable<QuotDTO>> GetAllAsync(int count, int timeout)
    {
        IEnumerable<Quot> quots = (await Database.QuotRepository.GetAllAsync(timeout)).Where(x => x.IsReaded is false).TakeLast(count);

        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task<QuotDTO> GetByIdAsync(long id, int timeout)
    {
        Quot quot = await Database.QuotRepository.GetByIdAsync(id, timeout);

        return QuotMapper.GetQuotDTO(quot);
    }

    public async Task MarkAsFavouriteAsync(long quotId, bool isFavourite, int cancelTimeout)
    {
        Quot quot = await Database.QuotRepository.GetByIdAsync(quotId, cancelTimeout) ?? throw new QuotNotFoundException($"Цитата с идентификатром {quotId} не найдена");

        quot.SetFavourite(isFavourite);

        await Database.QuotRepository.EditAsync(quot, cancelTimeout);
    }

    public async Task MarkQuotAsReadedAsync(long quotId, int cancelTimeout = 5000)
    {
        Quot? quot = await Database.QuotRepository.GetByIdAsync(quotId, cancelTimeout) ?? throw new QuotNotFoundException($"Цитата с идентификатром {quotId} не найдена");

        quot.MarkAsReaded();

        await Database.QuotRepository.EditAsync(quot, cancelTimeout);
    }
}
