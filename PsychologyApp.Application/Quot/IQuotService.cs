using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.QuotService;

public interface IQuotService : IAppService
{
    public Task SaveQuotFromApi(int cancelTimeout);
    public Task<IList<QuotDTO>> GetQuotsList(int count, bool readed = false, int cancelTimeout = 3000);
    public Task AddNewQuot(QuotDTO quotDTO, int cancelTimeout);
    public Task MarkQuotAsReaded(int quotId, int cancelTimeout);
}
