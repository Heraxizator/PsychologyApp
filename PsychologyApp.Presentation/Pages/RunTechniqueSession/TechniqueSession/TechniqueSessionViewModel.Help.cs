using System.ComponentModel;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

/// <summary>Help with filling the steps without typing: the last problem, ready phrases that fit what was written, and short answers allowed.</summary>
public partial class TechniqueSessionViewModel
{
    private readonly ISupportPhraseSuggester? _phraseSuggester;
    private readonly RecentProblemStore? _recentProblems;
    private readonly PsychologyApp.Presentation.Shared.Services.Preferences.ChipUsageStore? _chipUsage;
    private CancellationTokenSource? _assistLoading;
    private List<EntryItem> _watchedEntries = [];

    private void WatchEntries()
    {
        foreach (EntryItem entry in _watchedEntries)
        {
            entry.PropertyChanged -= OnEntryChanged;
        }

        _watchedEntries = [.. Entries];
        foreach (EntryItem entry in _watchedEntries)
        {
            entry.PropertyChanged += OnEntryChanged;
        }
    }

    private void OnEntryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EntryItem.Text))
        {
            Notify(nameof(NextStepText), nameof(CanFinish), nameof(IsCurrentEmpty), nameof(NextStepVariant));
        }
    }

    /// <summary>A practice about a problem starts from the one worked on last time: one tap instead of typing it again.</summary>
    private async Task OfferRecentProblemAsync()
    {
        if (_recentProblems is null || !Entries.Any(e => e.Role == EntryFieldRole.Problem))
        {
            return;
        }

        try
        {
            string recent = await _recentProblems.GetAsync();
            await UiThread.RunAsync(() =>
            {
                foreach (EntryItem entry in Entries.Where(e => e.Role == EntryFieldRole.Problem))
                {
                    entry.RecentText = recent;
                }
            });
        }
        catch (Exception bestEffortError)
        {
            BestEffort.Report(bestEffortError);
        }
    }

    /// <summary>Balanced thoughts and kind words are the hardest fields to write: offer ready phrases that fit the earlier answers.</summary>
    private void OfferPhrasesForCurrentStep()
    {
        if (_phraseSuggester is null || CurrentEntry is not { Role: EntryFieldRole.Reframe or EntryFieldRole.Kindness or EntryFieldRole.SelfKindness } entry)
        {
            return;
        }

        _assistLoading?.Cancel();
        _assistLoading = new CancellationTokenSource();
        CancellationToken token = _assistLoading.Token;
        string context = string.Join(". ", Entries
            .TakeWhile(e => !ReferenceEquals(e, entry))
            .Where(e => e.Kind == EntryFieldKind.Text && !string.IsNullOrWhiteSpace(e.Text))
            .Select(e => e.Text.Trim()));

        LoadPhrasesAsync(entry, context, token).FireAndForget();
    }

    private async Task LoadPhrasesAsync(EntryItem entry, string context, CancellationToken token)
    {
        try
        {
            IReadOnlyList<string> phrases = await _phraseSuggester!.SuggestAsync(
                entry.Role, context, AppStrings.Language, token);
            if (!token.IsCancellationRequested)
            {
                await UiThread.RunAsync(() => entry.AssistSuggestions = phrases);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception bestEffortError)
        {
            BestEffort.Report(bestEffortError);
        }
    }

    /// <summary>
    /// Counts the short pieces typed into each step of a finished practice; what recurs becomes a chip of the person's own next time.
    /// Feelings are left out (they have the palette) and so are scales.
    /// </summary>
    private void RememberTypedAnswers()
    {
        if (_chipUsage is null)
        {
            return;
        }

        foreach (EntryItem entry in Entries.Where(e => e.Kind == EntryFieldKind.Text && e.Role != EntryFieldRole.Emotion))
        {
            _chipUsage.Typed(entry.Title, PracticeAnswers.TypedParts(entry.Text, entry.Suggestions));
        }
    }

    private Task RememberProblemAsync()
    {
        string? problem = Entries.FirstOrDefault(e => e.Role == EntryFieldRole.Problem && !string.IsNullOrWhiteSpace(e.Text))?.Text;
        return _recentProblems is null || problem is null ? Task.CompletedTask : _recentProblems.RememberAsync(problem);
    }
}
