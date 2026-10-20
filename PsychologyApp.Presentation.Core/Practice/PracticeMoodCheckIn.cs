using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Presentation.Common;

/// <summary>
/// The mood picked on the practice completion screen. The first face adds a journal entry; picking another face corrects
/// that entry, so changing one's mind ("3, no, 4") does not leave several entries and check-in marks behind.
/// </summary>
public sealed class PracticeMoodCheckIn(IUserProgressService progress)
{
    private long? _entryId;

    public async Task SaveAsync(int moodLevel, string? note, CancellationToken cancellationToken = default)
    {
        if (_entryId is long entryId)
        {
            await progress.UpdateMoodEntryAsync(entryId, moodLevel, note, cancellationToken).ConfigureAwait(false);
            return;
        }

        _entryId = await progress.RecordMoodAsync(moodLevel, note, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
