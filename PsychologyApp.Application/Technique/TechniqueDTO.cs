using PsychologyApp.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Models;

public struct TechniqueDTO
{
    public long TechniqueId { get; set; }
    public string? Number { get; set; }
    public string? Date { get; set; }
    public string? Header { get; set; }
    public string? Describtion { get; set; }
    public string? Subject { get; set; }
    public string? Author { get; set; }
    public string? Actions { get; set; }
    public string? Image {  get; set; }
    public bool IsCompleted { get; set; }
}
