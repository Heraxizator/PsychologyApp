using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public interface IQuotRepository
{
    Task<IEnumerable<Quot>> GetByTitleAsync(string title);
    Task<IEnumerable<Quot>> GetByThemeAsync(string theme);
}
