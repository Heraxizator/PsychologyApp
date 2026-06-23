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

    public static StatisticDTO GetStatisticDTO(global::PsychologyApp.Domain.Entities.Statistic statistic)
    {
        return new StatisticDTO
        {
            StatisticId = statistic.StatisticId,
            ModuleName = statistic.ModuleName,
            PageName = statistic.PageName,
            DateTime = statistic.DateTime,
            SecondsDuration = statistic.SecondsDuration
        };
    }
}