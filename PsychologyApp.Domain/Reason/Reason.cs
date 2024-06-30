using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

public class Reason : BaseAuditableEntity
{
    public Reason() { }

    public Reason(string? title, string? subtitle, string? solution)
    {
        this.Title = title;
        this.Subtitle = subtitle;
        this.Solution = solution;
    }

    [Key]
    public long ReasonId { get; private init; }

    public string? Title { get; private set; }
    public string? Subtitle { get; private set; }
    public string? Solution { get; private set; }
}
