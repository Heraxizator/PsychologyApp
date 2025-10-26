using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Exceptions;

public class UnsupportedColourMeaningException : Exception
{
    public UnsupportedColourMeaningException(string message) : base(message)
    {
    }
}
