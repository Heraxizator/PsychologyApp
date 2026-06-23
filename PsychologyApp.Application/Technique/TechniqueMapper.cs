using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application;

public static class TechniqueMapper
{
    public static global::PsychologyApp.Domain.Entities.Technique GetTechnique(TechniqueDTO techniqueDTO) =>
        global::PsychologyApp.Domain.Entities.Technique.Create(
            techniqueDTO.TechniqueId,
            techniqueDTO.Number!,
            techniqueDTO.Date!,
            techniqueDTO.Header!,
            techniqueDTO.Description!,
            techniqueDTO.Subject!,
            techniqueDTO.Author!,
            techniqueDTO.Algorithm!,
            techniqueDTO.Image!);

    public static TechniqueDTO GetTechniqueDTO(global::PsychologyApp.Domain.Entities.Technique technique) =>
        new()
        {
            TechniqueId = technique.TechniqueId,
            Number = technique.Number,
            Date = technique.Date,
            Header = technique.Header,
            Description = technique.Description,
            Subject = technique.Subject,
            Author = technique.Author,
            Algorithm = technique.Algorithm,
            Image = technique.Image,
            IsCompleted = technique.IsCompleted
        };
}
