using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application;

public static class QuotMapper
{
    public static QuotDTO GetQuotDTO(global::PsychologyApp.Domain.Entities.Quot quot)
    {
        return new QuotDTO
        {
            QuotId = quot.QuotId,
            Title = quot.Title,
            Text = quot.Text,
            Theme = quot.Theme,
            IsReaded = quot.IsReaded,
            IsFavourite = quot.IsFavourite
        };
    }

    public static global::PsychologyApp.Domain.Entities.Quot GetQuot(QuotDTO quotDTO)
    {
        return global::PsychologyApp.Domain.Entities.Quot.Create(quotDTO.Title!, quotDTO.Text!, quotDTO.Theme!, quotDTO.IsReaded, quotDTO.IsFavourite);
    }
}
