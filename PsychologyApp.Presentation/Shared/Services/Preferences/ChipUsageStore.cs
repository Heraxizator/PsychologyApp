using System.Text.Json;
using System.Text.Json.Serialization;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Presentation.Shared.Services.Preferences;

[JsonSerializable(typeof(Dictionary<string, int>))]
internal partial class ChipUsageJsonContext : JsonSerializerContext;

/// <summary>
/// How often each answer chip was picked, so the familiar ones come first next time. Kept in the app's encrypted database with the
/// session drafts (a chip like "A breakup" says something personal), loaded once and saved after each pick.
/// </summary>
public sealed class ChipUsageStore(IUserProgressService progress)
{
    internal const string StorageKey = "practice.chip-usage";
    private const int MaxChips = 600;

    private readonly Dictionary<string, int> _counts = new(StringComparer.Ordinal);
    private Task? _loading;

    public IReadOnlyDictionary<string, int> Counts => _counts;

    public bool IsLoaded { get; private set; }

    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    public void Used(string chip)
    {
        if (string.IsNullOrWhiteSpace(chip))
        {
            return;
        }

        _counts[chip] = _counts.GetValueOrDefault(chip) + 1;
        TrimAndSave();
    }

    /// <summary>
    /// Counts the short pieces typed in a finished practice for one question; a piece typed again and again becomes the person's own chip
    /// (see <see cref="PracticeAnswers.OwnChips"/>).
    /// </summary>
    public void Typed(string question, IReadOnlyList<string> parts)
    {
        if (string.IsNullOrWhiteSpace(question) || parts.Count == 0)
        {
            return;
        }

        foreach (string part in parts)
        {
            string key = PracticeAnswers.OwnChipKey(question, part);
            _counts[key] = _counts.GetValueOrDefault(key) + 1;
        }

        TrimAndSave();
    }

    /// <summary>The person's own chips for a question, from what they typed before.</summary>
    public IReadOnlyList<string> OwnChips(string question, IReadOnlyList<string> offered) =>
        IsLoaded && !string.IsNullOrWhiteSpace(question) ? PracticeAnswers.OwnChips(question, _counts, offered) : [];

    private void TrimAndSave()
    {
        if (_counts.Count > MaxChips)
        {
            // Forget the rarely used ones first: the list only has to remember habits.
            foreach (string rare in _counts.OrderBy(pair => pair.Value).Take(_counts.Count - MaxChips).Select(pair => pair.Key).ToList())
            {
                _counts.Remove(rare);
            }
        }

        SaveAsync().FireAndForget();
    }

    private async Task LoadAsync()
    {
        try
        {
            string? json = await progress.GetSessionDraftAsync(StorageKey);
            if (!string.IsNullOrWhiteSpace(json)
                && JsonSerializer.Deserialize(json, ChipUsageJsonContext.Default.DictionaryStringInt32) is { } stored)
            {
                foreach ((string chip, int count) in stored)
                {
                    _counts[chip] = _counts.GetValueOrDefault(chip) + count;
                }
            }
        }
        catch (Exception bestEffortError)
        {
            // Only the order of the chips depends on it.
            BestEffort.Report(bestEffortError);
        }
        finally
        {
            IsLoaded = true;
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            // Never write over the stored counts before they are read: the load adds them to what was counted meanwhile.
            await EnsureLoadedAsync();
            string json = JsonSerializer.Serialize(new Dictionary<string, int>(_counts), ChipUsageJsonContext.Default.DictionaryStringInt32);
            await progress.SaveSessionDraftAsync(StorageKey, json);
        }
        catch (Exception bestEffortError)
        {
            BestEffort.Report(bestEffortError);
        }
    }
}
