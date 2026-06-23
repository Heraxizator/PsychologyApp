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
    public static global::PsychologyApp.Domain.Entities.Reason GetReason(ReasonDTO reasonDTO)
    {
        return global::PsychologyApp.Domain.Entities.Reason.Create(reasonDTO.Title!, reasonDTO.Subtitle!, reasonDTO.Solution!);
    }

    public static ReasonDTO GetReasonDTO(global::PsychologyApp.Domain.Entities.Reason reason)
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
