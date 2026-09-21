using System.Windows.Input;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

/// <summary>Settings section that downloads, shows and removes the on-device language model. Downloading always needs explicit consent.</summary>
public sealed class LocalAiSettingsViewModel : BaseViewModel
{
    private const long BytesPerMegabyte = 1024 * 1024;

    private readonly ILocalModelInstaller _installer;
    private readonly ILocalLanguageModel _model;
    private readonly IDialogService _dialogs;

    private CancellationTokenSource? _download;
    private bool _isDownloading;
    private bool _isInstalled;
    private double _progress;
    private string _statusText = string.Empty;

    public LocalAiSettingsViewModel(ILocalModelInstaller installer, ILocalLanguageModel model, IDialogService dialogs)
    {
        _installer = installer;
        _model = model;
        _dialogs = dialogs;
        _isInstalled = installer.IsInstalled;

        DownloadCommand = new AsyncCommand(DownloadAsync);
        CancelCommand = new Command(() => _download?.Cancel());
        DeleteCommand = new AsyncCommand(DeleteAsync);
    }

    /// <summary>Needs Android 7.0+ and a language the model was evaluated for; otherwise the section is hidden and the companion uses scripted replies.</summary>
    public bool IsSupported =>
        OperatingSystem.IsAndroidVersionAtLeast(24) && _installer.Manifest.SupportsLanguage(AppStrings.IsEnglish(AppStrings.Language));

    public ICommand DownloadCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }

    public string Title => AppStrings.LocalAiTitle;
    public string Description => AppStrings.LocalAiDescription;
    public string ExperimentalNote => AppStrings.LocalAiExperimentalNote;
    public string DownloadText => AppStrings.LocalAiDownloadAction(ModelMegabytes);
    public string CancelText => AppStrings.LocalAiCancelDownload;
    public string DeleteText => AppStrings.LocalAiDeleteAction;

    public bool CanDownload => !_isInstalled && !_isDownloading;
    public bool ShowStatus => _statusText.Length > 0;

    public bool IsDownloading
    {
        get => _isDownloading;
        private set
        {
            if (SetProperty(ref _isDownloading, value))
            {
                Notify(nameof(CanDownload));
            }
        }
    }

    public bool IsInstalled
    {
        get => _isInstalled;
        private set
        {
            if (SetProperty(ref _isInstalled, value))
            {
                Notify(nameof(CanDownload));
            }
        }
    }

    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set
        {
            if (SetProperty(ref _statusText, value))
            {
                Notify(nameof(ShowStatus));
            }
        }
    }

    private long ModelMegabytes => Math.Max(1, _installer.Manifest.TotalBytes / BytesPerMegabyte);

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(IsSupported), nameof(Title), nameof(Description), nameof(ExperimentalNote), nameof(DownloadText), nameof(CancelText), nameof(DeleteText));

    private async Task DownloadAsync()
    {
        bool accepted = await _dialogs.AskAsync(
            AppStrings.LocalAiConfirmTitle,
            AppStrings.LocalAiConfirmBody(_installer.Manifest.DisplayName, ModelMegabytes),
            AppStrings.LocalAiConfirmAccept,
            AppStrings.LocalAiCancelDialog);
        if (!accepted)
        {
            return;
        }

        _download = new CancellationTokenSource();
        IsDownloading = true;
        Progress = 0;
        StatusText = AppStrings.LocalAiDownloading(0);

        Progress<ModelInstallProgress> reporter = new(p =>
        {
            Progress = p.Fraction;
            StatusText = AppStrings.LocalAiDownloading((int)(p.Fraction * 100));
        });

        try
        {
            await _installer.InstallAsync(reporter, _download.Token);
            StatusText = string.Empty;
        }
        catch (OperationCanceledException)
        {
            StatusText = AppStrings.LocalAiCanceled;
        }
        catch (Exception)
        {
            StatusText = AppStrings.LocalAiFailed;
        }
        finally
        {
            _download.Dispose();
            _download = null;
            IsDownloading = false;
            IsInstalled = _installer.IsInstalled;
        }
    }

    private async Task DeleteAsync()
    {
        bool accepted = await _dialogs.AskAsync(
            AppStrings.LocalAiDeleteAction,
            AppStrings.LocalAiDeleteConfirm,
            AppStrings.LocalAiDeleteAccept,
            AppStrings.LocalAiCancelDialog);
        if (!accepted)
        {
            return;
        }

        await _model.ReleaseAsync();
        await _installer.DeleteAsync();
        StatusText = string.Empty;
        IsInstalled = _installer.IsInstalled;
    }
}
