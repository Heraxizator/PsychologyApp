using PsychologyApp.Application.Helpers;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Base.ServiceLocator.Toast;
using PsychologyApp.Presentation.Base.ServiceLocatorж;

namespace PsychologyApp.Presentation;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        InitServices();

        ConfigureMigrations();

        Database.ConfigureSQLite();

        _ = Task.Run(async () =>
        {
            _ = await ReasonExtension.SavePsyhosomaticData(15000);
            _ = await QuotHandler.GetQuotsFromApi(15000);
        });
    }

    private void InitServices()
    {
        Base.ServiceLocator.ServiceLocator.Instance.Register<IToastService>(new ToastService());
        Base.ServiceLocator.ServiceLocator.Instance.Register<IDialogService>(new DialogService());
    }

    private void ConfigureMigrations()
    {
        string currentVersionString = $"{AppInfo.Current.VersionString}";

        if (Preferences.Default.ContainsKey(currentVersionString) is false)
        {
            Database.ReCreateTables();

            Preferences.Default.Set(currentVersionString, true);
        }
    }
}
