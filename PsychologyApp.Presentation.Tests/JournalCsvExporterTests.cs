using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Features.ManageJournal;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("StaticShims")]
public sealed class JournalCsvExporterTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Sharing_removes_the_temporary_file_containing_private_notes(bool shareFails)
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string previous = FileSystem.CacheDirectory;
        FileSystem.CacheDirectory = directory;
        Share.FileShareError = shareFails ? new IOException("Share unavailable") : null;
        try
        {
            Task sharing = JournalCsvExporter.ShareAsync([new MoodEntryDTO
            {
                MoodLevel = 3, Note = "private journal note", RecordedAt = DateTime.UtcNow
            }]);
            if (shareFails)
            {
                await Assert.ThrowsAsync<IOException>(() => sharing);
            }
            else
            {
                await sharing;
                Assert.Contains("private journal note", Share.Shared[^1].Content);
            }
            Assert.Empty(Directory.EnumerateFiles(directory));
        }
        finally
        {
            FileSystem.CacheDirectory = previous;
            Share.FileShareError = null;
            Directory.Delete(directory, recursive: true);
        }
    }
}
