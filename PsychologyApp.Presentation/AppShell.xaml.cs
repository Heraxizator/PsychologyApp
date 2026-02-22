using PsychologyApp.Application;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Presentation;

public partial class AppShell : Shell
{
    private readonly IQuotService _quotService = new QuotService();
    private readonly IReasonService _reasonService = new ReasonService();
    public AppShell()
    {
        try
        {
            InitializeComponent();

            ConfigureMigrations();

            Database.ConfigureSQLite();

            _ = Task.Run(async () =>
            {
                await _quotService.LoadSingleAsync(5000);
                await _quotService.LoadSingleAsync(5000);
            });
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
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
