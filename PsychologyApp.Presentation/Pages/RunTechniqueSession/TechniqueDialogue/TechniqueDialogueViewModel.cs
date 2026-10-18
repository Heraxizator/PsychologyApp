using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Serialization;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Widgets.DialogueChat;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;

/// <summary>A scripted technique (Observer, Grounding...) run as a chat instead of a form.</summary>
public sealed class TechniqueDialogueViewModel : ChatViewModelBase
{
    /// <summary>
    /// Captures named <c>f0</c>, <c>f1</c>... are the answers to the practice's form fields by index: they are saved as the session
    /// payload exactly like a form would save them, so the history shows a dialogue practice the same way.
    /// </summary>
    private const string FieldCapturePrefix = "f";

    /// <summary>The Observer scenario predates the field captures and keeps the situation under this name.</summary>
    private const string SituationCapture = "situation";

    private readonly TechniqueId _techniqueId;
    private readonly IConversationScenarioProvider _scenarioProvider;
    private readonly ICrisisDetector _crisisDetector;
    private readonly IUserProgressService _userProgressService;
    private readonly TechniqueSessionCompletionService _completionService;
    private readonly RecentProblemStore? _recentProblems;
    private readonly ChipUsageStore? _chipUsage;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    private ConversationSession? _scenarioSession;

    public TechniqueDialogueViewModel(
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITechniqueCatalogService techniqueCatalogService,
        IConversationScenarioProvider scenarioProvider,
        ICrisisDetector crisisDetector,
        TechniqueSessionCompletionService completionService,
        RecentProblemStore? recentProblems = null,
        ChipUsageStore? chipUsage = null) : base(navigationService)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _scenarioProvider = scenarioProvider;
        _crisisDetector = crisisDetector;
        _completionService = completionService;
        _recentProblems = recentProblems;
        _chipUsage = chipUsage;
        TechniqueCatalogService = techniqueCatalogService;
    }

    protected override async Task OnStartingAsync()
    {
        await InitializeTechniqueAsync(_techniqueId);
        if (_chipUsage is not null)
        {
            await _chipUsage.EnsureLoadedAsync();
        }
    }

    /// <summary>The familiar chips first, and before them what the person keeps typing for this question, as chips of their own.</summary>
    protected override IReadOnlyList<string> RankSuggestions(IReadOnlyList<string> suggestions, string? capture)
    {
        if (_chipUsage is not { IsLoaded: true } usage)
        {
            return suggestions;
        }

        IReadOnlyList<string> own = QuestionOf(capture) is { } question ? usage.OwnChips(question, suggestions) : [];
        return [.. own, .. PracticeAnswers.Rank(suggestions, usage.Counts)];
    }

    /// <summary>The form question an <c>fN</c> capture answers: own chips are kept per question, shared with the form of the practice.</summary>
    private string? QuestionOf(string? capture)
    {
        if (capture is null
            || !capture.StartsWith(FieldCapturePrefix, StringComparison.Ordinal)
            || !int.TryParse(capture[FieldCapturePrefix.Length..], out int index))
        {
            return null;
        }

        var entries = AppliedDefinition?.Entries?.ToList();
        return entries is not null && index >= 0 && index < entries.Count ? entries[index].Title : null;
    }

    /// <summary>Counts the short pieces typed in the finished dialogue, per question, so what recurs becomes a chip next time.</summary>
    private void RememberTypedAnswers()
    {
        var entries = AppliedDefinition?.Entries?.ToList();
        if (_chipUsage is null || entries is null)
        {
            return;
        }

        foreach ((string key, string answer) in CapturedFields())
        {
            int index = int.Parse(key, System.Globalization.CultureInfo.InvariantCulture);
            if (index < entries.Count && entries[index] is { Kind: EntryFieldKind.Text } entry && entry.Role != EntryFieldRole.Emotion)
            {
                _chipUsage.Typed(entry.Title, PracticeAnswers.TypedParts(answer, entry.Suggestions ?? []));
            }
        }
    }

    protected override void OnSuggestionChosen(string suggestion) => _chipUsage?.Used(suggestion);

    protected override async Task<string?> GetRecentProblemAsync() =>
        _recentProblems is null ? null : await _recentProblems.GetAsync(Lifetime);

    protected override async Task<IDialogueSession?> CreateSessionAsync()
    {
        ConversationScenario? scenario = await _scenarioProvider.LoadAsync(_techniqueId, Lifetime);
        if (scenario is null)
        {
            return null;
        }

        _scenarioSession = new ConversationSession(scenario, _crisisDetector);
        return _scenarioSession;
    }

    protected override string GetFinishText(ConversationStatus status, DialogueAction? action) => status switch
    {
        ConversationStatus.Completed => AppStrings.DialogueFinish,
        ConversationStatus.Interrupted => AppStrings.CrisisHubTitle,
        _ => AppStrings.DialogueClose
    };

    protected override async Task OnFinishAsync(ConversationStatus status, DialogueAction? action)
    {
        Close();

        if (status == ConversationStatus.Interrupted)
        {
            await NavigationService!.GoToCrisisHubAsync();
            return;
        }

        if (status != ConversationStatus.Completed)
        {
            await GoBackAsync();
            return;
        }

        bool savedFields = await SaveAnswersAsync();
        await RememberProblemAsync();
        RememberTypedAnswers();

        await _completionService.CompleteStandardSessionAsync(
            _userProgressService,
            NavigationService!,
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt,
            preIntensity: PreIntensity(),
            deleteDraft: savedFields);
    }

    /// <summary>The rating taken before the practice: the scenario's "rating_before", or else the first rating field of the form (Spin asks it as f2).</summary>
    private int? PreIntensity()
    {
        if (_scenarioSession?.GetRating("rating_before") is int before)
        {
            return before;
        }

        int index = AppliedDefinition?.Entries?.ToList().FindIndex(e => e.Kind == EntryFieldKind.Rating0To10) ?? -1;
        return index >= 0 ? _scenarioSession?.GetRating(FieldCapturePrefix + index.ToString(System.Globalization.CultureInfo.InvariantCulture)) : null;
    }

    private Dictionary<string, string> CapturedFields() =>
        (_scenarioSession?.Captured ?? new Dictionary<string, string>())
            .Where(pair => pair.Key.StartsWith(FieldCapturePrefix, StringComparison.Ordinal)
                           && int.TryParse(pair.Key[FieldCapturePrefix.Length..], out _))
            .ToDictionary(pair => pair.Key[FieldCapturePrefix.Length..], pair => pair.Value, StringComparer.Ordinal);

    /// <summary>Writes the answers as the session draft (replacing any older form draft), so completion records them as the payload.</summary>
    private async Task<bool> SaveAnswersAsync()
    {
        Dictionary<string, string> fields = CapturedFields();
        if (fields.Count == 0)
        {
            return false;
        }

        try
        {
            await SessionDraftStore.SaveAsync(
                _userProgressService,
                _techniqueId.ToString(),
                new PsychologyApp.Presentation.Common.EntryDraft { Fields = fields },
                SessionDraftJsonSerializerContext.Default.EntryDraft);
            return true;
        }
        catch (Exception bestEffortError)
        {
            BestEffort.Report(bestEffortError);
            return false;
        }
    }

    private async Task RememberProblemAsync()
    {
        if (_recentProblems is null || _scenarioSession is null)
        {
            return;
        }

        Dictionary<string, string> fields = CapturedFields();
        int problemIndex = AppliedDefinition?.Entries?.ToList().FindIndex(e => e.Role == EntryFieldRole.Problem) ?? -1;
        string? problem = problemIndex >= 0 && fields.TryGetValue(problemIndex.ToString(System.Globalization.CultureInfo.InvariantCulture), out string? answer)
            ? answer
            : _scenarioSession.Captured.GetValueOrDefault(SituationCapture);

        await _recentProblems.RememberAsync(problem);
    }
}
