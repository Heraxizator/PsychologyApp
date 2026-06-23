using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Technique;

public sealed class TechniqueService(ITechniqueRepository techniqueRepository) : ITechniqueService
{
    public async Task AddNewTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);
        await techniqueRepository.AddAsync(technique, cancellationToken);
    }

    public async Task DeleteTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);
        await techniqueRepository.DeleteAsync(technique, cancellationToken);
    }

    public async Task<TechniqueDTO> GetTechniqueByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Technique? technique = await techniqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TechniqueNotFoundException($"РўРµС…РЅРёРєР° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {id} РЅРµ РЅР°Р№РґРµРЅР°");

        return TechniqueMapper.GetTechniqueDTO(technique);
    }

    public async Task<IEnumerable<TechniqueDTO>> GetTechniquesListAsync(int count, CancellationToken cancellationToken = default)
    {
        IEnumerable<global::PsychologyApp.Domain.Entities.Technique> techniques = await techniqueRepository.GetLatestAsync(count, cancellationToken);
        return techniques.Select(TechniqueMapper.GetTechniqueDTO);
    }

    public async Task MarkTechniqueAsCompletedAsync(long id, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Technique? technique = await techniqueRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new TechniqueNotFoundException($"РўРµС…РЅРёРєР° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {id} РЅРµ РЅР°Р№РґРµРЅР°");

        technique.MarkAsCompleted();
        await techniqueRepository.EditAsync(technique, cancellationToken);
    }

    public async Task UpdateTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Technique technique = await techniqueRepository.GetByIdAsync(techniqueDTO.TechniqueId, cancellationToken)
            ?? throw new TechniqueNotFoundException($"РўРµС…РЅРёРєР° СЃ РёРґРµРЅС‚РёС„РёРєР°С‚РѕСЂРѕРј {techniqueDTO.TechniqueId} РЅРµ РЅР°Р№РґРµРЅР°");

        technique.ApplyContent(
            techniqueDTO.Number!,
            techniqueDTO.Date!,
            techniqueDTO.Header!,
            techniqueDTO.Description!,
            techniqueDTO.Subject!,
            techniqueDTO.Author!,
            techniqueDTO.Algorithm!,
            techniqueDTO.Image);

        await techniqueRepository.EditAsync(technique, cancellationToken);
    }
}
