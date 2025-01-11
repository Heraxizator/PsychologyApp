using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Presentation.Base.ServiceLocator.Toast
{
    internal interface IToastService
    {
        public void LongToast(string message);
        public void ShortToast(string message);
    }
}
