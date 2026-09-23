using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Text;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileDataBackup;

public sealed class DataBackupViewModel : BaseViewModel
{
    private readonly IBackupService _backupService;
    private readonly ISpecialistSummaryService _specialistSummaryService;
    private readonly IToastService _toastService;

    public ICommand BackCommand { get; }
    public ICommand ExportBackupCommand { get; }
    public ICommand ImportBackupCommand { get; }
    public ICommand ExportSummaryCommand { get; }

    public string PageTitle => AppStrings.DataBackupTitle;
    public string LeadText => AppStrings.DataBackupLead;
    public string ExportTitle => AppStrings.DataBackupExportTitle;
    public string ExportSubtitle => AppStrings.DataBackupExportSubtitle;
    public string ImportTitle => AppStrings.DataBackupImportTitle;
    public string ImportSubtitle => AppStrings.DataBackupImportSubtitle;
    public string SummaryTitle => AppStrings.DataBackupSummaryTitle;
    public string SummarySubtitle => AppStrings.DataBackupSummarySubtitle;

    public DataBackupViewModel(
        INavigationService navigationService,
        IBackupService backupService,
        ISpecialistSummaryService specialistSummaryService,
        IToastService toastService)
    {
        BindNavigation(navigationService);
        _backupService = backupService;
        _specialistSummaryService = specialistSummaryService;
        _toastService = toastService;

        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        ExportBackupCommand = new AsyncCommand(ExportBackupAsync);
        ImportBackupCommand = new AsyncCommand(ImportBackupAsync);
        ExportSummaryCommand = new AsyncCommand(ExportSummaryAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(LeadText),
            nameof(ExportTitle),
            nameof(ExportSubtitle),
            nameof(ImportTitle),
            nameof(ImportSubtitle),
            nameof(SummaryTitle),
            nameof(SummarySubtitle));
    }

    private async Task ExportBackupAsync()
    {
        string json = await _backupService.ExportAsync();
        string fileName = $"psychologyapp-backup-{DateTime.Now:yyyyMMdd-HHmm}.json";
        string path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(path, json, Encoding.UTF8);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = AppStrings.DataBackupExportTitle,
            File = new ShareFile(path)
        });
    }

    private async Task ImportBackupAsync()
    {
        try
        {
            FileResult? file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = AppStrings.DataBackupImportTitle
            });
            if (file is null)
            {
                return;
            }

            string json = await File.ReadAllTextAsync(file.FullPath, Encoding.UTF8);
            BackupImportResult result = await _backupService.ImportAsync(json);

            _toastService.LongToast(
                AppStrings.DataBackupImportedToast(
                    result.MoodEntries,
                    result.TestResults,
                    result.Completions,
                    result.ChatSessions));
        }
        catch
        {
            _toastService.LongToast(AppStrings.DataBackupImportFailedToast, AppToastKind.Error);
        }
    }

    private async Task ExportSummaryAsync()
    {
        bool english = UserPreferences.IsEnglish(UserPreferences.Load().Language);
        string summary = await _specialistSummaryService.BuildSummaryAsync(english);
        string fileName = $"specialist-summary-{DateTime.Now:yyyyMMdd-HHmm}.txt";
        string path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(path, summary, Encoding.UTF8);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = AppStrings.DataBackupSummaryTitle,
            File = new ShareFile(path)
        });
    }
}
