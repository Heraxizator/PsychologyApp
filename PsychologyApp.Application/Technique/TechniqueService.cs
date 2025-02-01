using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Application.Services.TechniqueService;

public sealed class TechniqueService : ITechniqueService
{
    public async Task AddNewTechnique(TechniqueDTO techniqueDTO, int cancelTimeout = 5000)
    {
        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.AddAsync(technique, cancelTimeout);
    }

    public async Task DeleteTechnique(TechniqueDTO techniqueDTO, int cancelTimeout = 5000)
    {
        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.DeleteAsync(technique, cancelTimeout);
    }

    public async Task<TechniqueDTO> GetTechniqueById(long id, int cancelTimeout = 5000)
    {
        Technique? technique = await Database.TechniqueRepository.GetByIdAsync(id, cancelTimeout);

        return TechniqueMapper.GetTechniqueDTO(technique);
    }

    public async Task<IEnumerable<TechniqueDTO>> GetTechniquesList(int count, int cancelTimeout = 5000)
    {
        IEnumerable<Technique> techniques = (await Database.TechniqueRepository.GetAllAsync(cancelTimeout)).Take(count);

        return techniques.Select(TechniqueMapper.GetTechniqueDTO);
    }

    public async Task MarkTechniqueAsCompleted(long id, int cancelTimeout = 5000)
    {
        Technique? technique = await Database.TechniqueRepository.GetByIdAsync(id, cancelTimeout);

        if (technique is null)
        {
            throw new TechniqueNotFoundException($"Техника с идентификатором {id} не найдена");
        }

        technique.MarkAsCompleted();

        await Database.TechniqueRepository.EditAsync(technique, cancelTimeout);
    }

    public async Task UpdateTechnique(TechniqueDTO techniqueDTO, int cancelTimeout = 5000)
    {
        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.EditAsync(technique, cancelTimeout);
    }
}
