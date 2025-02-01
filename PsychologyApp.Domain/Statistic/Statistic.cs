using PsychologyApp.Domain.Base;
using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

[Dapper.Contrib.Extensions.Table("Statistics")]
public class Statistic : Entity
{
    [Key]
    [Dapper.Contrib.Extensions.Key]

    public long StatisticId { get; private init; } = default!;
    public string ModuleName { get; private set; } = default!;
    public string PageName { get; private set; } = default!;
    public DateTime DateTime { get; private set; } = default!;
    public long SecondsDuration { get; private set; } = default!;

    public static Statistic Create(string moduleName, string pageName, DateTime dateTime, long secondsDuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleName);
        ArgumentException.ThrowIfNullOrWhiteSpace(pageName);

        Statistic statistic = new Statistic
        {
            ModuleName = moduleName,
            PageName = pageName,
            SecondsDuration = secondsDuration
        };

        return statistic;
    }
}
