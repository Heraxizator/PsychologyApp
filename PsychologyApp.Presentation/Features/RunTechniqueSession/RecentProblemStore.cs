using Microsoft.Extensions.Logging;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Serialization;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>
/// The problem the person worked on in their last practice, so the next practice can start from it with one tap instead of
/// typing it again. Kept with the session drafts in the app's encrypted database, never in plain preferences.
/// </summary>
public sealed class RecentProblemStore(IUserProgressService progress, ILogger<RecentProblemStore> logger)
{
    internal const string StorageKey = "practice.recent-problem";
    private const string Field = "text";
    private const int MaxLength = 500;

    private string? _cached;

    public async Task<string> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is not null)
        {
            return _cached;
        }

        try
        {
            EntryDraft? stored = await SessionDraftStore.LoadAsync(
                progress, StorageKey, SessionDraftJsonSerializerContext.Default.EntryDraft, logger, cancellationToken);
            _cached = stored?.Fields.GetValueOrDefault(Field) ?? string.Empty;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            BestEffort.Report(ex);
            _cached = string.Empty;
        }

        return _cached;
    }

    public async Task RememberAsync(string? problem, CancellationToken cancellationToken = default)
    {
        string text = problem?.Trim() ?? string.Empty;
        if (text.Length == 0)
        {
            return;
        }

        if (text.Length > MaxLength)
        {
            text = text[..MaxLength];
        }

        _cached = text;
        try
        {
            await SessionDraftStore.SaveAsync(
                progress,
                StorageKey,
                new EntryDraft { Fields = new Dictionary<string, string>(StringComparer.Ordinal) { [Field] = text } },
                SessionDraftJsonSerializerContext.Default.EntryDraft,
                cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Only a convenience for the next practice.
            BestEffort.Report(ex);
        }
    }
}
