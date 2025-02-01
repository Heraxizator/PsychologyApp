using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Application;

public sealed class QuotService : IQuotService
{
    public async Task AddNewQuot(QuotDTO quotDTO, int cancelTimeout = 5000)
    {
        Quot quot = QuotMapper.GetQuot(quotDTO);

        await Database.QuotRepository.AddAsync(quot, cancelTimeout);
    }

    public async Task<IEnumerable<QuotDTO>> GetQuotsList(int count, bool readed = false, int cancelTimeout = 5000)
    {
        IEnumerable<Quot> quots = (await Database.QuotRepository.GetAllAsync(cancelTimeout)).Where(x => x.IsReaded is false).TakeLast(count);

        return quots.Select(QuotMapper.GetQuotDTO);
    }

    public async Task MarkQuotAsReaded(int quotId, int cancelTimeout = 5000)
    {
        Quot? quot = await Database.QuotRepository.GetByIdAsync(quotId, cancelTimeout) ?? throw new QuotNotFoundException($"Цитата с идентификатром {quotId} не найдена");

        quot.MarkAsReaded();

        await Database.QuotRepository.EditAsync(quot, cancelTimeout);
    }
}
