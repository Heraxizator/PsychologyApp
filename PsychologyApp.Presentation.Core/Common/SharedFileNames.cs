using System.Globalization;

namespace PsychologyApp.Presentation.Common;

/// <summary>
/// The names of the files the app writes to its cache to share them (an exported journal, a backup, a summary for a specialist,
/// the error log). They hold the person's records, so "delete all my data" removes them by these names: one place decides both,
/// and a file renamed here is still found by the eraser.
/// </summary>
public static class SharedFileNames
{
    private const string JournalPrefix = "journal-";
    private const string AppPrefix = "psychologyapp-";
    private const string SummaryPrefix = "specialist-summary-";

    public static string JournalExport(DateTime localNow) => $"{JournalPrefix}{Stamp(localNow)}.csv";

    public static string Backup(bool passphraseProtected, DateTime localNow) =>
        $"{AppPrefix}backup{(passphraseProtected ? "-protected" : string.Empty)}-{Stamp(localNow)}.json";

    public static string SpecialistSummary(DateTime localNow) => $"{SummaryPrefix}{Stamp(localNow)}.txt";

    public static string ErrorLog(DateTime localNow) => $"{AppPrefix}errors-{Stamp(localNow)}.log";

    /// <summary>
    /// Whether a file in the cache is one of these. A picked backup the file picker copied there keeps its exported name, so it
    /// counts as well.
    /// </summary>
    public static bool IsOwn(string fileName) =>
        fileName.StartsWith(JournalPrefix, StringComparison.OrdinalIgnoreCase)
        || fileName.StartsWith(AppPrefix, StringComparison.OrdinalIgnoreCase)
        || fileName.StartsWith(SummaryPrefix, StringComparison.OrdinalIgnoreCase);

    private static string Stamp(DateTime localNow) => localNow.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
}
