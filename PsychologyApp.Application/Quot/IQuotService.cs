using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.QuotService;

public interface IQuotService : IAppService
{
    public Task<IEnumerable<QuotDTO>> GetAllAsync(int count, int cancelTimeout);
    public Task<QuotDTO> GetByIdAsync(long id, int cancelTimeout);
    public Task AddSingleAsync(QuotDTO quotDTO, int cancelTimeout);
    public Task LoadSingleAsync(int cancelTimeout);
    public Task MarkAsReadedAsync(long quotId, int cancelTimeout);
    public Task MarkAsFavouriteAsync(long quotId, bool isFavourite, int cancelTimeout);
}
