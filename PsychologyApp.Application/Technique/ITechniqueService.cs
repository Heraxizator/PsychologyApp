using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.TechniqueService;

public interface ITechniqueService : IAppService
{
    public Task<IEnumerable<TechniqueDTO>> GetTechniquesList(int count, int cancelTimeout);
    public Task<TechniqueDTO> GetTechniqueById(long id, int cancelTimeout);
    public Task AddNewTechnique(TechniqueDTO techniqueDTO, int cancelTimeout);
    public Task DeleteTechnique(TechniqueDTO techniqueDTO, int cancelTimeout);
    public Task UpdateTechnique(TechniqueDTO techniqueDTO, int cancelTimeout);
    public Task MarkTechniqueAsCompleted(long id, int cancelTimeout);
}
