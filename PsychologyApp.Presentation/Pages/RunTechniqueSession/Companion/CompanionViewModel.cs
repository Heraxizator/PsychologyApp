using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Widgets.DialogueChat;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Companion;

/// <summary>Free-form entry point: the person describes the situation, the companion answers and hands over to the fitting practice.</summary>
public sealed class CompanionViewModel : ChatViewModelBase
{
    private const int HandOverDelayMs = 600;

    private readonly ISituationAnalyzer _analyzer;
    private readonly ICrisisDetector _crisisDetector;
    private readonly ILocalLanguageModel _model;

    public CompanionViewModel(
        INavigationService navigationService,
        ISituationAnalyzer analyzer,
        ICrisisDetector crisisDetector,
        ILocalLanguageModel model) : base(navigationService)
    {
        _analyzer = analyzer;
        _crisisDetector = crisisDetector;
        _model = model;
        PageName = AppStrings.CompanionTitle;
    }

    protected override Task OnStartingAsync()
    {
        // Load the model while the person is still reading the greeting and typing.
        _ = WarmUpModelAsync();
        return Task.CompletedTask;
    }

    private async Task WarmUpModelAsync()
    {
        try
        {
            await _model.WarmUpAsync(Lifetime);
        }
        catch (Exception)
        {
            // Warm-up is an optimisation only; generation reports its own failures.
        }
    }

    protected override Task<IDialogueSession?> CreateSessionAsync() =>
        Task.FromResult<IDialogueSession?>(new CompanionSession(
            _analyzer,
            _crisisDetector,
            _model,
            AppStrings.IsEnglish(AppStrings.Language)));

    protected override string GetFinishText(ConversationStatus status, DialogueAction? action) =>
        status == ConversationStatus.Interrupted ? AppStrings.CrisisHubTitle : AppStrings.DialogueClose;

    protected override async Task OnEndedAsync(ConversationStatus status, DialogueAction? action)
    {
        switch (action)
        {
            case { Kind: DialogueActionKind.StartTechnique, TechniqueId: { } techniqueId }:
                await Task.Delay(HandOverDelayMs, Lifetime);
                await NavigationService!.GoToTechniqueAsync(techniqueId);
                break;

            case { Kind: DialogueActionKind.OpenCrisisHub }:
                await Task.Delay(HandOverDelayMs, Lifetime);
                await NavigationService!.GoToCrisisHubAsync();
                break;
        }
    }

    protected override Task OnFinishAsync(ConversationStatus status, DialogueAction? action) =>
        status == ConversationStatus.Interrupted
            ? NavigationService!.GoToCrisisHubAsync()
            : GoBackAsync();
}
