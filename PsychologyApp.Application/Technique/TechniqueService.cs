using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Services.TechniqueService;

public sealed class TechniqueService(ITechniqueRepository techniqueRepository) : ITechniqueService
{
    public async Task AddNewTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);
        await techniqueRepository.AddAsync(technique, cancellationToken);
    }

    public async Task DeleteTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);
        await techniqueRepository.DeleteAsync(technique, cancellationToken);
    }

    public async Task<TechniqueDTO> GetTechniqueByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        Technique? technique = await techniqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TechniqueNotFoundException($"Техника с идентификатором {id} не найдена");

        return TechniqueMapper.GetTechniqueDTO(technique);
    }

    public async Task<IEnumerable<TechniqueDTO>> GetTechniquesListAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<Technique> techniques = await techniqueRepository.GetLatestAsync(count, cancellationToken);
        return techniques.Select(TechniqueMapper.GetTechniqueDTO);
    }

    public async Task MarkTechniqueAsCompletedAsync(long id, CancellationToken cancellationToken = default)
    {
        Technique? technique = await techniqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TechniqueNotFoundException($"Техника с идентификатором {id} не найдена");

        technique.MarkAsCompleted();
        await techniqueRepository.EditAsync(technique, cancellationToken);
    }

    public async Task UpdateTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        Technique technique = await techniqueRepository.GetByIdAsync(techniqueDTO.TechniqueId, cancellationToken)
            ?? throw new TechniqueNotFoundException($"Техника с идентификатором {techniqueDTO.TechniqueId} не найдена");

        technique.ApplyContent(
            techniqueDTO.Number!,
            techniqueDTO.Date!,
            techniqueDTO.Header!,
            techniqueDTO.Describtion!,
            techniqueDTO.Subject!,
            techniqueDTO.Author!,
            techniqueDTO.Actions!,
            techniqueDTO.Image);

        await techniqueRepository.EditAsync(technique, cancellationToken);
    }
}
