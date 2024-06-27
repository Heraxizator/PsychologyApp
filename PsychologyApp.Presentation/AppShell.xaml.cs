using PsychologyApp.Presentation.ServiceLocator;
using PsychologyApp.Presentation.ServiceLocator.Dialog;

namespace PsychologyApp.Presentation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void InitServices()
        {
            ServiceLocator.ServiceLocator.Instance.Register<IToastService>(new ToastService());
            ServiceLocator.ServiceLocator.Instance.Register<IDialogService>(new DialogService());
        }
    }
}
