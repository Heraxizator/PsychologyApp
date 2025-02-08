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
    public static QuotDTO GetQuotDTO(Quot quot)
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

    public static Quot GetQuot(QuotDTO quotDTO)
    {
        return Quot.Create(quotDTO.Title!, quotDTO.Text!, quotDTO.Theme!, quotDTO.IsReaded, quotDTO.IsFavourite);
    }
}
