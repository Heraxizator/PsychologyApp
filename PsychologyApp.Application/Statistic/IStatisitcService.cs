using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.Statistic;

public interface IStatisitcService : IAppService
{
    Task<IEnumerable<StatisticDTO>> GetAllAsync(int cancelTimeout);
    Task AddSingleAsync(StatisticDTO statisticDTO, int cancelTimeout);
    Task<long> CountPageCompletedAsync(int cancelTimeout);
    Task<long> CountPageFactsAsync(string pageName, int cancelTimeout);

}
