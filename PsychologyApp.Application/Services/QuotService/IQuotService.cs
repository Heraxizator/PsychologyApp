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
    public Task SaveQuotsFromApi(int count);
    public Task<IList<QuotDTO>> GetQuotsList(int count, bool readed = false);
    public Task AddNewQuot(QuotDTO quotDTO);
    public Task MarkQuotAsReaded(int quotId);
}
