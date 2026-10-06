using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDataBackup;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>Export and import of the person's whole history, including the passphrase flow.</summary>
[Collection("StaticShims")]
public sealed class DataBackupViewModelTests : IDisposable
{
    private readonly Mock<IBackupService> _backup = new();
    private readonly Mock<IDialogService> _dialogs = new();
    private readonly Mock<IToastService> _toasts = new();
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"backup-vm-{Guid.NewGuid():N}");

    public DataBackupViewModelTests()
    {
        Directory.CreateDirectory(_folder);
        FileSystem.CacheDirectory = _folder;
        Share.Shared.Clear();
        FilePicker.Next = null;
        _backup.Setup(b => b.ExportAsync(It.IsAny<CancellationToken>())).ReturnsAsync("PLAIN-BACKUP");
        _backup.Setup(b => b.ExportEncryptedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync("ENCRYPTED-BACKUP");
    }

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private DataBackupViewModel Create() =>
        new(new Mock<INavigationService>().Object, _backup.Object, new Mock<ISpecialistSummaryService>().Object, _toasts.Object, new MauiUserPreferencesStore(), _dialogs.Object);

    private void Choose(string? option) =>
        _dialogs.Setup(d => d.PickOptionAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>())).ReturnsAsync(option);

    private void Passphrases(params string?[] answers)
    {
        Queue<string?> queue = new(answers);
        _dialogs.Setup(d => d.PromptPasswordAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(() => queue.Count > 0 ? queue.Dequeue() : null);
    }

    private void WriteFile(string content)
    {
        string path = Path.Combine(_folder, $"picked-{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        FilePicker.Next = new FileResult(path);
    }

    [Fact]
    public async Task AProtectedExportAsksTwiceAndSharesTheEncryptedFile()
    {
        Choose(AppStrings.BackupProtectWith);
        Passphrases("correct horse", "correct horse");

        await VmTestHelpers.RunAsync(Create().ExportBackupCommand);

        _backup.Verify(b => b.ExportEncryptedAsync("correct horse", It.IsAny<CancellationToken>()), Times.Once);
        _backup.Verify(b => b.ExportAsync(It.IsAny<CancellationToken>()), Times.Never);
        (string _, string fileName, string content) = Assert.Single(Share.Shared);
        Assert.Contains("-protected-", fileName);
        Assert.Equal("ENCRYPTED-BACKUP", content);
    }

    [Fact]
    public async Task AnUnprotectedExportNeedsNoPassphrase()
    {
        Choose(AppStrings.BackupProtectWithout);

        await VmTestHelpers.RunAsync(Create().ExportBackupCommand);

        _dialogs.Verify(d => d.PromptPasswordAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        (string _, string fileName, string content) = Assert.Single(Share.Shared);
        Assert.DoesNotContain("protected", fileName);
        Assert.Equal("PLAIN-BACKUP", content);
    }

    [Fact]
    public async Task CancellingTheChoiceExportsNothing()
    {
        Choose(null);

        await VmTestHelpers.RunAsync(Create().ExportBackupCommand);

        Assert.Empty(Share.Shared);
        _backup.Verify(b => b.ExportAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("short", null)]
    [InlineData("correct horse", "different one")]
    public async Task ATooShortOrMismatchedPassphraseExportsNothingAndSaysWhy(string first, string? second)
    {
        Choose(AppStrings.BackupProtectWith);
        Passphrases(first, second);

        await VmTestHelpers.RunAsync(Create().ExportBackupCommand);

        Assert.Empty(Share.Shared);
        _backup.Verify(b => b.ExportEncryptedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _toasts.Verify(t => t.LongToast(It.IsAny<string>(), AppToastKind.Error), Times.Once);
    }

    [Fact]
    public async Task TheTemporaryFileIsRemovedAfterSharing()
    {
        Choose(AppStrings.BackupProtectWithout);

        await VmTestHelpers.RunAsync(Create().ExportBackupCommand);

        Assert.Empty(Directory.GetFiles(_folder, "psychologyapp-backup*"));
    }

    [Fact]
    public async Task APlainBackupIsImportedWithoutAskingForAPassphrase()
    {
        WriteFile("{}");
        _backup.Setup(b => b.IsEncrypted(It.IsAny<string>())).Returns(false);
        _backup.Setup(b => b.ImportAsync("{}", It.IsAny<CancellationToken>())).ReturnsAsync(new BackupImportResult(1, 2, 3, 4, 5, false));

        await VmTestHelpers.RunAsync(Create().ImportBackupCommand);

        _backup.Verify(b => b.ImportAsync("{}", It.IsAny<CancellationToken>()), Times.Once);
        _dialogs.Verify(d => d.PromptPasswordAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _toasts.Verify(t => t.LongToast(It.IsAny<string>(), AppToastKind.Info), Times.Once);
    }

    [Fact]
    public async Task AProtectedBackupAllowsThreeTriesAtThePassphrase()
    {
        WriteFile("ENC");
        _backup.Setup(b => b.IsEncrypted("ENC")).Returns(true);
        _backup.Setup(b => b.ImportEncryptedAsync("ENC", It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new BackupPassphraseException("wrong"));
        Passphrases("bad one", "bad two", "bad three", "bad four");

        await VmTestHelpers.RunAsync(Create().ImportBackupCommand);

        _backup.Verify(b => b.ImportEncryptedAsync("ENC", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _toasts.Verify(t => t.ShortToast(AppStrings.BackupPassphraseWrongToast, AppToastKind.Error), Times.Exactly(3));
    }

    [Fact]
    public async Task TheRightPassphraseAfterAWrongOneImports()
    {
        WriteFile("ENC");
        _backup.Setup(b => b.IsEncrypted("ENC")).Returns(true);
        _backup.Setup(b => b.ImportEncryptedAsync("ENC", "wrong", It.IsAny<CancellationToken>())).ThrowsAsync(new BackupPassphraseException("wrong"));
        _backup.Setup(b => b.ImportEncryptedAsync("ENC", "right", It.IsAny<CancellationToken>())).ReturnsAsync(new BackupImportResult(1, 0, 0, 0, 0, false));
        Passphrases("wrong", "right");

        await VmTestHelpers.RunAsync(Create().ImportBackupCommand);

        _backup.Verify(b => b.ImportEncryptedAsync("ENC", "right", It.IsAny<CancellationToken>()), Times.Once);
        _toasts.Verify(t => t.LongToast(It.IsAny<string>(), AppToastKind.Info), Times.Once);
    }

    [Fact]
    public async Task CancellingThePassphrasePromptImportsNothing()
    {
        WriteFile("ENC");
        _backup.Setup(b => b.IsEncrypted("ENC")).Returns(true);
        Passphrases((string?)null);

        await VmTestHelpers.RunAsync(Create().ImportBackupCommand);

        _backup.Verify(b => b.ImportEncryptedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancellingTheFilePickerDoesNothing()
    {
        FilePicker.Next = null;

        await VmTestHelpers.RunAsync(Create().ImportBackupCommand);

        _backup.Verify(b => b.ImportAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _toasts.Verify(t => t.LongToast(It.IsAny<string>(), It.IsAny<AppToastKind>()), Times.Never);
    }

    [Fact]
    public async Task AnUnreadableFileAndAFailedImportAreReportedDifferently()
    {
        WriteFile("junk");
        _backup.Setup(b => b.IsEncrypted(It.IsAny<string>())).Returns(false);
        _backup.SetupSequence(b => b.ImportAsync("junk", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BackupFormatException("not a backup"))
            .ThrowsAsync(new InvalidOperationException("rolled back"));
        DataBackupViewModel viewModel = Create();

        await VmTestHelpers.RunAsync(viewModel.ImportBackupCommand);
        await VmTestHelpers.RunAsync(viewModel.ImportBackupCommand);

        _toasts.Verify(t => t.LongToast(AppStrings.DataBackupImportFailedToast, AppToastKind.Error), Times.Once);
        _toasts.Verify(t => t.LongToast(AppStrings.DataBackupImportRolledBackToast, AppToastKind.Error), Times.Once);
    }
}
