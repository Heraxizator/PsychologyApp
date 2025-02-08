using PsychologyApp.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Models;

public struct QuotDTO 
{ 
    public long QuotId {  get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public string? Theme { get; set; }
    public bool IsReaded { get; set; }
    public bool IsFavourite {  get; set; }
}
