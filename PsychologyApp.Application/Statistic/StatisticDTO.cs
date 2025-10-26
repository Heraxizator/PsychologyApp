using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Models;

public struct StatisticDTO
{
    public long StatisticId { get; set; }
    public string? ModuleName { get; set; }
    public string? PageName { get; set; }
    public DateTime DateTime { get; set; }
    public long SecondsDuration { get; set; }
}
