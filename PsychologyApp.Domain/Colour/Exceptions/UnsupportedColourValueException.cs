using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Exceptions;

public class UnsupportedColourValueException : Exception
{
    public UnsupportedColourValueException(string message) : base(message)
    {
    }
}
