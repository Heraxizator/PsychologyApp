using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Widgets.DialogueChat;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;

/// <summary>A scripted technique (Observer, Grounding...) run as a chat instead of a form.</summary>
public sealed class TechniqueDialogueViewModel : ChatViewModelBase
{
    private readonly TechniqueId _techniqueId;
    private readonly IConversationScenarioProvider _scenarioProvider;
    private readonly ICrisisDetector _crisisDetector;
    private readonly IUserProgressService _userProgressService;
    private readonly TechniqueSessionCompletionService _completionService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    private ConversationSession? _scenarioSession;

    public TechniqueDialogueViewModel(
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITechniqueCatalogService techniqueCatalogService,
        IConversationScenarioProvider scenarioProvider,
        ICrisisDetector crisisDetector,
        TechniqueSessionCompletionService completionService) : base(navigationService)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _scenarioProvider = scenarioProvider;
        _crisisDetector = crisisDetector;
        _completionService = completionService;
        TechniqueCatalogService = techniqueCatalogService;
    }

    protected override Task OnStartingAsync() => InitializeTechniqueAsync(_techniqueId);

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

        await _completionService.CompleteStandardSessionAsync(
            _userProgressService,
            NavigationService!,
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt,
            preIntensity: _scenarioSession?.GetRating("rating_before"),
            deleteDraft: false);
    }
}
