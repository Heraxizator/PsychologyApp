using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.TechniqueService;

public interface ITechniqueService
{
    Task<IEnumerable<TechniqueDTO>> GetTechniquesListAsync(int count, CancellationToken cancellationToken = default);
    Task<TechniqueDTO> GetTechniqueByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddNewTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default);
    Task DeleteTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default);
    Task UpdateTechniqueAsync(TechniqueDTO techniqueDTO, CancellationToken cancellationToken = default);
    Task MarkTechniqueAsCompletedAsync(long id, CancellationToken cancellationToken = default);
}
