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
    public ICommand ShareErrorLogCommand { get; }

    public string ErrorLogTitle => AppStrings.ErrorLogTitle;
    public string ErrorLogSubtitle => AppStrings.ErrorLogSubtitle;

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
        ShareErrorLogCommand = new AsyncCommand(ShareErrorLogAsync);
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
            nameof(SummarySubtitle),
            nameof(ErrorLogTitle),
            nameof(ErrorLogSubtitle));
    }

    private async Task ExportBackupAsync()
    {
        string json = await _backupService.ExportAsync();
        string fileName = $"psychologyapp-backup-{DateTime.Now:yyyyMMdd-HHmm}.json";
        await ShareTemporaryFileAsync(fileName, json, AppStrings.DataBackupExportTitle);
    }

    /// <summary>
    /// The file holds chats, mood notes and the safety plan in plain text, so it is written to the cache only for the moment
    /// the share sheet is open and removed afterwards (also when sharing fails or is cancelled).
    /// </summary>
    private static async Task ShareTemporaryFileAsync(string fileName, string content, string title)
    {
        string path = Path.Combine(FileSystem.CacheDirectory, fileName);
        try
        {
            await File.WriteAllTextAsync(path, content, Encoding.UTF8);
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(path)
            });
        }
        finally
        {
            TryDelete(path);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
            // Left for the OS to clear from the cache.
        }
        catch (UnauthorizedAccessException)
        {
        }
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
                    result.Completions + result.SessionResults,
                    result.ChatSessions,
                    result.SkippedDuplicates));
        }
        catch (BackupFormatException)
        {
            // Nothing was written: the import is a single transaction and the file was rejected before it.
            _toastService.LongToast(AppStrings.DataBackupImportFailedToast, AppToastKind.Error);
        }
        catch (Exception)
        {
            // The transaction rolled back, so the database is exactly as it was before the attempt.
            _toastService.LongToast(AppStrings.DataBackupImportRolledBackToast, AppToastKind.Error);
        }
    }

    private async Task ShareErrorLogAsync()
    {
        string text;
        try
        {
            string path = Shared.Common.Infrastructure.DebugFileLoggerProvider.ErrorLogPath;
            text = File.Exists(path) ? await File.ReadAllTextAsync(path, Encoding.UTF8) : string.Empty;
        }
        catch (IOException)
        {
            text = string.Empty;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            _toastService.ShortToast(AppStrings.ErrorLogEmptyToast);
            return;
        }

        await ShareTemporaryFileAsync($"psychologyapp-errors-{DateTime.Now:yyyyMMdd-HHmm}.log", text, AppStrings.ErrorLogTitle);
    }

    private async Task ExportSummaryAsync()
    {
        bool english = UserPreferences.IsEnglish(UserPreferences.Load().Language);
        string summary = await _specialistSummaryService.BuildSummaryAsync(english);
        string fileName = $"specialist-summary-{DateTime.Now:yyyyMMdd-HHmm}.txt";
        await ShareTemporaryFileAsync(fileName, summary, AppStrings.DataBackupSummaryTitle);
    }
}
