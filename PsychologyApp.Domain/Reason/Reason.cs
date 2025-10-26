using PsychologyApp.Domain.Base;
using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Entities;

[Dapper.Contrib.Extensions.Table("Reasons")]
public class Reason : Entity
{
    [Key]
    [Dapper.Contrib.Extensions.Key]

    public long ReasonId { get; private init; } = default!;
    public string Title { get; private set; } = default!;
    public string Subtitle { get; private set; } = default!;
    public string Solution { get; private set; } = default!;

    public static Reason Create(string title, string subtitle, string solution)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(subtitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(solution);

        Reason reason = new Reason
        {
            Title = title,
            Subtitle = subtitle,
            Solution = solution
        };

        return reason;
    }
}
