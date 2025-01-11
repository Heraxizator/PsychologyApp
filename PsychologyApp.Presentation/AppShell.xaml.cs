using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Base.ServiceLocator.Toast;
using PsychologyApp.Presentation.Base.ServiceLocatorж;

namespace PsychologyApp.Presentation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            InitServices();
        }

        private void InitServices()
        {
            Base.ServiceLocator.ServiceLocator.Instance.Register<IToastService>(new ToastService());
            Base.ServiceLocator.ServiceLocator.Instance.Register<IDialogService>(new DialogService());
        }
    }
}
