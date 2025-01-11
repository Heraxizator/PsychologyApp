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
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.AddAsync(technique);
    }

    public async Task DeleteTechnique(TechniqueDTO techniqueDTO, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.DeleteAsync(technique);
    }

    public async Task<TechniqueDTO> GetTechniqueById(long id, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        Technique? technique = await Database.TechniqueRepository.GetByIdAsync(id);

        return TechniqueMapper.GetTechniqueDTO(technique);
    }

    public async Task<IEnumerable<TechniqueDTO>> GetTechniquesList(int count, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        IEnumerable<Technique> techniques = (await Database.TechniqueRepository.GetAllAsync()).Take(count);

        return techniques.Select(TechniqueMapper.GetTechniqueDTO);
    }

    public async Task MarkTechniqueAsCompleted(long id, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        Technique? technique = await Database.TechniqueRepository.GetByIdAsync(id);

        if (technique is null)
        {
            throw new TechniqueNotFoundException($"Техника с идентификатором {id} не найдена");
        }

        technique.MarkAsCompleted();

        await Database.TechniqueRepository.EditAsync(technique);
    }

    public async Task UpdateTechnique(TechniqueDTO techniqueDTO, int cancelTimeout = 5000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        Technique technique = TechniqueMapper.GetTechnique(techniqueDTO);

        await Database.TechniqueRepository.EditAsync(technique);
    }
}
