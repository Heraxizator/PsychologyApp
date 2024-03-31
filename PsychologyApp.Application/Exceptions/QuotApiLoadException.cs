using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Exceptions
{
    public class QuotApiLoadException : Exception
    {
        public QuotApiLoadException(string? message) : base(message)
        {
        }
    }
}
