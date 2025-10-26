using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Reasons;

internal interface IReasonRepository
{
    Task<IEnumerable<Reason>> GetByTitle(string title);
}
