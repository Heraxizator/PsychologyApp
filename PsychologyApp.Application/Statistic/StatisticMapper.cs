using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application;

public static class StatisticMapper
{
    public static global::PsychologyApp.Domain.Entities.Statistic GetStatistic(StatisticDTO statisticDTO)
    {
        return global::PsychologyApp.Domain.Entities.Statistic.Create(statisticDTO.ModuleName!, statisticDTO.PageName!, statisticDTO.DateTime, statisticDTO.SecondsDuration);
    }
}