using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Services.ReasonService
{
    public interface IReasonService : IAppService
    {
        public Task<IEnumerable<ReasonDTO>> GetReasonsAsync(int page, int perInPage, int cancelTimeout);
    }
}
