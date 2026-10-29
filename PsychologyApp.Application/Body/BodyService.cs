using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;

namespace PsychologyApp.Application.Body;

/// <param name="Crisis">The note read as a crisis: the screen leads to help instead of practices.</param>
public sealed record BodyCheckResult(BodyEntry Entry, bool Crisis);

public interface IBodyService
{
    Task<BodyLog> GetLogAsync(CancellationToken cancellationToken = default);

    Task<BodyCheckResult> SaveAsync(
        BodyArea area,
        BodySensation sensation,
        int? intensity,
        IReadOnlyList<BodyFeeling> feelings,
        string? note,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string entryId, CancellationToken cancellationToken = default);
}

public sealed class BodyService(IBodyRepository repository, ICrisisDetector crisisDetector, TimeProvider? timeProvider = null) : IBodyService
{
    public const int MaxNote = 600;

    /// <summary>The oldest check-ins go first beyond this; patterns come from the recent ones anyway.</summary>
    public const int MaxEntries = 500;

    private readonly TimeProvider _time = timeProvider ?? TimeProvider.System;

    // The log is one document: two quick saves must not each read the old version and overwrite the other.
    private readonly SemaphoreSlim _writeGate = new(1, 1);

    public async Task<BodyLog> GetLogAsync(CancellationToken cancellationToken = default) =>
        await repository.GetAsync(cancellationToken).ConfigureAwait(false) ?? new BodyLog();

    public async Task<BodyCheckResult> SaveAsync(
        BodyArea area,
        BodySensation sensation,
        int? intensity,
        IReadOnlyList<BodyFeeling> feelings,
        string? note,
        CancellationToken cancellationToken = default)
    {
        BodyEntry entry = Clean(new BodyEntry(Guid.NewGuid().ToString("N"), _time.GetUtcNow().UtcDateTime, area, sensation, intensity, feelings, note))!;

        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            BodyLog log = await GetLogAsync(cancellationToken).ConfigureAwait(false);
            await repository.SaveAsync(log with { Entries = Trim([.. log.Entries, entry]) }, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeGate.Release();
        }

        // Storage limits must not hide a crisis signal at the end of the original note.
        return new BodyCheckResult(entry, crisisDetector.IsCrisis(note));
    }

    public async Task DeleteAsync(string entryId, CancellationToken cancellationToken = default)
    {
        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            BodyLog log = await GetLogAsync(cancellationToken).ConfigureAwait(false);
            await repository.SaveAsync(log with { Entries = log.Entries.Where(entry => entry.Id != entryId).ToList() }, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _writeGate.Release();
        }
    }

    /// <summary>
    /// Joins a backup's check-ins with the ones on the device: an entry keeps its id, so restoring the same file twice adds nothing and
    /// nothing written since is lost.
    /// </summary>
    public static BodyLog Merge(BodyLog? local, BodyLog? fromBackup)
    {
        IEnumerable<BodyEntry> mine = local?.Entries ?? [];
        HashSet<string> known = mine.Select(entry => entry.Id).ToHashSet(StringComparer.Ordinal);
        // The backup side comes through Sanitize at the import boundary.
        IEnumerable<BodyEntry> added = (fromBackup?.Entries ?? []).Where(entry => !string.IsNullOrWhiteSpace(entry.Id) && known.Add(entry.Id));
        return new BodyLog { Entries = Trim(mine.Concat(added).OrderBy(entry => entry.CreatedUtc).ToList()) };
    }

    /// <summary>
    /// A log from outside (a backup file) as the app would have written it: entries without an id or a date are dropped, the rest
    /// get a list of feelings, a strength in 0..10 and a note within <see cref="MaxNote"/>.
    /// </summary>
    public static BodyLog Sanitize(BodyLog? log) =>
        new() { Entries = [.. (log?.Entries ?? []).Select(Clean).OfType<BodyEntry>()] };

    /// <returns>Null for an entry that cannot be kept.</returns>
    private static BodyEntry? Clean(BodyEntry? entry)
    {
        if (entry is null || string.IsNullOrWhiteSpace(entry.Id) || entry.CreatedUtc == default)
        {
            return null;
        }

        string? note = string.IsNullOrWhiteSpace(entry.Note) ? null : entry.Note.Trim();
        return entry with
        {
            Intensity = entry.Intensity is { } intensity ? Math.Clamp(intensity, 0, 10) : null,
            // Older readers of the list must not trip over a null list in a hand-edited file.
            Feelings = (entry.Feelings ?? []).Distinct().ToList(),
            Note = note is { Length: > MaxNote } ? note[..MaxNote] : note
        };
    }

    private static List<BodyEntry> Trim(List<BodyEntry> entries) =>
        entries.Count <= MaxEntries ? entries : entries.Skip(entries.Count - MaxEntries).ToList();
}
