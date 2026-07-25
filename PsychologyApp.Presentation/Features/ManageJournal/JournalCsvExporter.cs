using System.Globalization;
using System.Text;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public static class JournalCsvExporter
{
    public static string BuildCsv(IEnumerable<MoodEntryDTO> entries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Date,Time,Mood,Factors,Note");
        foreach (MoodEntryDTO entry in entries.OrderBy(item => item.RecordedAt))
        {
            DateTime local = entry.RecordedAt.ToLocalTime();
            string factors = string.Join(
                ';',
                JournalNoteFactors.ExtractActiveLabels(entry.Note));
            string note = JournalNoteFactors.StripFactorLines(entry.Note);
            builder.Append(Csv(local.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))).Append(',');
            builder.Append(Csv(local.ToString("HH:mm", CultureInfo.InvariantCulture))).Append(',');
            builder.Append(entry.MoodLevel.ToString(CultureInfo.InvariantCulture)).Append(',');
            builder.Append(Csv(factors)).Append(',');
            builder.Append(Csv(note));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    public static async Task ShareAsync(
        IReadOnlyList<MoodEntryDTO> entries,
        CancellationToken cancellationToken = default)
    {
        string fileName = $"journal-{DateTime.Now:yyyyMMdd-HHmm}.csv";
        string path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(path, BuildCsv(entries), Encoding.UTF8, cancellationToken);
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = AppStrings.JournalExportTitle,
            File = new ShareFile(path)
        });
    }

    private static string Csv(string? value)
    {
        string text = value ?? string.Empty;
        if (text.Contains('"') || text.Contains(',') || text.Contains('\n') || text.Contains('\r'))
        {
            return $"\"{text.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
        }

        return text;
    }
}
