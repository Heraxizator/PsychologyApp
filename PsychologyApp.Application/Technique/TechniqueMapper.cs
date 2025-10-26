using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application;

public static class TechniqueMapper
{
    public static Technique GetTechnique(TechniqueDTO techniqueDTO)
    {
        return Technique.Create(techniqueDTO.TechniqueId, techniqueDTO.Number!, techniqueDTO.Date!, techniqueDTO.Header!, techniqueDTO.Describtion!, techniqueDTO.Subject!, techniqueDTO.Author!, techniqueDTO.Actions!, techniqueDTO.Image!);
    }

    public static TechniqueDTO GetTechniqueDTO(Technique technique)
    {
        return new TechniqueDTO
        {
            TechniqueId = technique.TechniqueId,
            Number = technique.Number,
            Date = technique.Date,
            Header = technique.Header,
            Describtion = technique.Describtion,
            Subject = technique.Subject,
            Author = technique.Author,
            Actions = technique.Algorithm,
            Image = technique.Image
        };
    }
}
