using System.Text.Json;
using PsychologyApp.Application.DataBackup;
using Xunit;

namespace PsychologyApp.Application.Tests.DataBackup;

public sealed class BackupEncryptionTests
{
    private const string Plain = """{"formatVersion":2,"note":"мне было тяжело 😔"}""";

    [Fact]
    public void ADecryptedFileIsExactlyWhatWasEncrypted()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");

        Assert.Equal(Plain, BackupEncryption.Decrypt(file, "correct horse battery"));
    }

    [Fact]
    public void TheEncryptedFileDoesNotContainTheText()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");

        Assert.DoesNotContain("тяжело", file);
        Assert.DoesNotContain("formatVersion", file);
        Assert.True(BackupEncryption.IsEncrypted(file));
    }

    [Fact]
    public void TwoEncryptionsOfTheSameTextDiffer()
    {
        Assert.NotEqual(
            BackupEncryption.Encrypt(Plain, "correct horse battery"),
            BackupEncryption.Encrypt(Plain, "correct horse battery"));
    }

    [Fact]
    public void AWrongPassphraseIsRefused()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");

        Assert.Throws<BackupPassphraseException>(() => BackupEncryption.Decrypt(file, "incorrect horse battery"));
        Assert.Throws<BackupPassphraseException>(() => BackupEncryption.Decrypt(file, string.Empty));
    }

    [Fact]
    public void AChangedFileIsRefusedEvenWithTheRightPassphrase()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");
        EncryptedBackupEnvelope envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope)!;
        byte[] data = Convert.FromBase64String(envelope.Data);
        data[0] ^= 0x01;
        envelope.Data = Convert.ToBase64String(data);
        string changed = JsonSerializer.Serialize(envelope, BackupJsonContext.Default.EncryptedBackupEnvelope);

        Assert.Throws<BackupPassphraseException>(() => BackupEncryption.Decrypt(changed, "correct horse battery"));
    }

    [Fact]
    public void AShortPassphraseIsRefusedBeforeEncrypting()
    {
        Assert.Throws<ArgumentException>(() => BackupEncryption.Encrypt(Plain, "short"));
        Assert.Throws<ArgumentException>(() => BackupEncryption.Encrypt(Plain, string.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("""{"formatVersion":2}""")]
    public void OnlyAnEncryptedBackupCountsAsEncrypted(string file) =>
        Assert.False(BackupEncryption.IsEncrypted(file));

    [Fact]
    public void AFileAskingForBillionsOfIterationsIsRefusedWithoutDoingTheWork()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");
        EncryptedBackupEnvelope envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope)!;
        envelope.Iterations = int.MaxValue;
        string hostile = JsonSerializer.Serialize(envelope, BackupJsonContext.Default.EncryptedBackupEnvelope);

        Assert.Throws<BackupFormatException>(() => BackupEncryption.Decrypt(hostile, "correct horse battery"));
    }

    [Fact]
    public void ANewerVersionIsRefused()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");
        EncryptedBackupEnvelope envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope)!;
        envelope.Version = 99;
        string newer = JsonSerializer.Serialize(envelope, BackupJsonContext.Default.EncryptedBackupEnvelope);

        Assert.Throws<BackupFormatException>(() => BackupEncryption.Decrypt(newer, "correct horse battery"));
    }

    [Fact]
    public void DamagedBase64IsAFormatErrorNotACrash()
    {
        string file = BackupEncryption.Encrypt(Plain, "correct horse battery");
        EncryptedBackupEnvelope envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope)!;
        envelope.Data = "***not base64***";
        string damaged = JsonSerializer.Serialize(envelope, BackupJsonContext.Default.EncryptedBackupEnvelope);

        Assert.Throws<BackupFormatException>(() => BackupEncryption.Decrypt(damaged, "correct horse battery"));
    }
}
