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
    public Task<IList<TechniqueDTO>> GetTechniquesList(int count);
    public Task<TechniqueDTO> GetTechniqueById(int id);
    public Task AddNewTechnique(TechniqueDTO techniqueDTO);
    public Task DeleteTechnique(TechniqueDTO techniqueDTO);
    public Task UpdateTechnique(TechniqueDTO techniqueDTO);
    public Task MarkTechniqueAsCompleted(int id);
}
