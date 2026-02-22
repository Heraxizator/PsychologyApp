using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Presentation.Base.ServiceLocator.Dialog
{
    public interface IDialogService
    {
        public void ShowAsync(string title, string message);
        public Task<bool> AskAsync(string title, string message, string accept, string cancel);
    }
}
