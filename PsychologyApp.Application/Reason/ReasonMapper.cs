using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application;

public static class ReasonMapper
{
    public static Reason GetReason(ReasonDTO reasonDTO)
    {
        return Reason.Create(reasonDTO.Title!, reasonDTO.Subtitle!, reasonDTO.Solution!);
    }

    public static ReasonDTO GetReasonDTO(Reason reason)
    {
        return new ReasonDTO
        {
            ReasonId = reason.ReasonId,
            Title = reason.Title,
            Subtitle = reason.Subtitle,
            Solution = reason.Solution,
        };
    }
}
