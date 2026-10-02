using PsychologyApp.Application.Exceptions;

namespace PsychologyApp.Application.DataBackup;

/// <summary>The file is not a backup this version can read (not JSON, wrong shape, too new or too large).</summary>
public sealed class BackupFormatException : AppException
{
    public BackupFormatException(string message) : base(message)
    {
    }

    public BackupFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
