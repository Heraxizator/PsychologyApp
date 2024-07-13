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
        public Task SaveReasonsIfEmpty(int cancelTimeout);
        public Task<IList<ReasonDTO>> GetReasons(int count, int cancelTimeout);
    }
}
