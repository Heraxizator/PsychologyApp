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
    public static Statistic GetStatistic(StatisticDTO statisticDTO)
    {
        return Statistic.Create(statisticDTO.ModuleName!, statisticDTO.PageName!, statisticDTO.DateTime, statisticDTO.SecondsDuration);
    }

    public static StatisticDTO GetStatisticDTO(Statistic statistic)
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