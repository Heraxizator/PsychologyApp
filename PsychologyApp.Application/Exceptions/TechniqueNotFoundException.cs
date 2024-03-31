using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Exceptions
{
    public class TechniqueNotFoundException : Exception
    {
        public TechniqueNotFoundException(string? message) : base(message)
        {
        }
    }
}
