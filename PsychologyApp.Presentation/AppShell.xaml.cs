using PsychologyApp.Application.Helpers;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Infrastructure.Data.Context;
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

            Database.CreateTables();

            Database.ConfigureSQLite();

            Task.Run(async () =>
            {
                await ReasonHelper.SavePsyhosomaticData(15000);
                await QuotHandler.GetQuotsFromApi(15000);
            });
        }

        private void InitServices()
        {
            Base.ServiceLocator.ServiceLocator.Instance.Register<IToastService>(new ToastService());
            Base.ServiceLocator.ServiceLocator.Instance.Register<IDialogService>(new DialogService());
        }
    }
}
