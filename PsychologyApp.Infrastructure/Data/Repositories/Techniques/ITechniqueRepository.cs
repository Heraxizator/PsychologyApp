using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Techniques;

internal interface ITechniqueRepository
{
    Task<IEnumerable<Technique>> GetByHeaderAsync(string header);
    Task<IEnumerable<Technique>> GetByAuthorAsync(string author);
    Task<IEnumerable<Technique>> GetBySubjectAsync(string subject);
}
