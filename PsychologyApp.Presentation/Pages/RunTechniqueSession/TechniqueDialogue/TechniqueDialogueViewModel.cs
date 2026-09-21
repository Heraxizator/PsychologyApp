using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;

public sealed record DialogueMessage(string Text, bool IsUser)
{
    public LayoutOptions Alignment => IsUser ? LayoutOptions.End : LayoutOptions.Start;
}

public sealed record DialogueChoiceItem(int Index, string Label);

public sealed class TechniqueDialogueViewModel : BaseViewModel
{
    private const int MinTypingDelayMs = 400;
    private const int MaxTypingDelayMs = 1400;
    private const int PerCharacterDelayMs = 8;

    private readonly TechniqueId _techniqueId;
    private readonly IConversationScenarioProvider _scenarioProvider;
    private readonly ICrisisDetector _crisisDetector;
    private readonly IUserProgressService _userProgressService;
    private readonly TechniqueSessionCompletionService _completionService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    private ConversationSession? _session;
    private bool _isBusy;
    private bool _isClosed;
    private bool _started;
    private ConversationStatus _finalStatus;

    private ConversationInputKind? _inputKind;
    private string _draftText = string.Empty;
    private string? _inputHint;
    private bool _isTyping;
    private bool _isFinished;
    private bool _isCrisis;
    private string _finishText = string.Empty;

    public TechniqueDialogueViewModel(
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITechniqueCatalogService techniqueCatalogService,
        IConversationScenarioProvider scenarioProvider,
        ICrisisDetector crisisDetector,
        TechniqueSessionCompletionService completionService)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _scenarioProvider = scenarioProvider;
        _crisisDetector = crisisDetector;
        _completionService = completionService;
        TechniqueCatalogService = techniqueCatalogService;

        BindNavigation(navigationService);

        BackCommand = new AsyncCommand(GoBackAsync);
        SendCommand = new Command(() => RunAsync(SendTextAsync));
        ChooseCommand = new Command<int>(index => RunAsync(() => ChooseAsync(index)));
        RateCommand = new Command<int>(rating => RunAsync(() => RateAsync(rating)));
        FinishCommand = new AsyncCommand(FinishAsync);
    }

    public ObservableCollection<DialogueMessage> Messages { get; } = [];

    public ObservableCollection<DialogueChoiceItem> Choices { get; } = [];

    public IReadOnlyList<int> RatingValues { get; } = Enumerable.Range(0, 11).ToArray();

    public ICommand BackCommand { get; }
    public ICommand SendCommand { get; }
    public ICommand ChooseCommand { get; }
    public ICommand RateCommand { get; }
    public ICommand FinishCommand { get; }

    public string DraftText
    {
        get => _draftText;
        set => SetProperty(ref _draftText, value);
    }

    public string InputPlaceholder => string.IsNullOrWhiteSpace(_inputHint) ? AppStrings.DialogueInputPlaceholder : _inputHint;

    public string SendText => AppStrings.Send;

    public string TypingText => AppStrings.DialogueTyping;

    public string RatingScaleText => AppStrings.DialogueRatingScale;

    public bool IsTyping
    {
        get => _isTyping;
        private set => SetProperty(ref _isTyping, value);
    }

    public bool IsTextInput => !_isFinished && _inputKind == ConversationInputKind.Text;

    public bool IsChoiceInput => !_isFinished && _inputKind == ConversationInputKind.Choice;

    public bool IsRatingInput => !_isFinished && _inputKind == ConversationInputKind.Rating;

    public bool IsFinished
    {
        get => _isFinished;
        private set
        {
            if (SetProperty(ref _isFinished, value))
            {
                NotifyInputChanged();
            }
        }
    }

    public bool IsCrisis
    {
        get => _isCrisis;
        private set => SetProperty(ref _isCrisis, value);
    }

    public string FinishText
    {
        get => _finishText;
        private set => SetProperty(ref _finishText, value);
    }

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        await InitializeTechniqueAsync(_techniqueId);

        ConversationScenario? scenario = await _scenarioProvider.LoadAsync(_techniqueId);
        if (scenario is null)
        {
            AppendMessage(AppStrings.DialogueLoadFailed, isUser: false);
            EndConversation(ConversationStatus.Paused);
            return;
        }

        _session = new ConversationSession(scenario, _crisisDetector);
        await ShowTurnAsync(_session.Start());
    }

    public void Close() => _isClosed = true;

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(InputPlaceholder), nameof(SendText), nameof(TypingText), nameof(RatingScaleText));

    private void RunAsync(Func<Task> action)
    {
        if (_isBusy || _isClosed || _session is null)
        {
            return;
        }

        _isBusy = true;
        _ = RunGuardedAsync(action);
    }

    private async Task RunGuardedAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            AsyncCommandExtensions.DefaultErrorHandler?.Invoke(ex);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task SendTextAsync()
    {
        string text = DraftText.Trim();
        if (text.Length == 0)
        {
            return;
        }

        DraftText = string.Empty;
        AppendMessage(text, isUser: true);
        await ShowTurnAsync(_session!.SubmitText(text));
    }

    private async Task ChooseAsync(int index)
    {
        DialogueChoiceItem? choice = Choices.FirstOrDefault(c => c.Index == index);
        if (choice is null)
        {
            return;
        }

        AppendMessage(choice.Label, isUser: true);
        await ShowTurnAsync(_session!.SubmitChoice(index));
    }

    private async Task RateAsync(int rating)
    {
        AppendMessage(rating.ToString(System.Globalization.CultureInfo.InvariantCulture), isUser: true);
        await ShowTurnAsync(_session!.SubmitRating(rating));
    }

    private async Task ShowTurnAsync(ConversationTurn turn)
    {
        SetInput(null, null);

        foreach (string message in turn.BotMessages)
        {
            IsTyping = true;
            await Task.Delay(Math.Clamp(message.Length * PerCharacterDelayMs, MinTypingDelayMs, MaxTypingDelayMs));
            if (_isClosed)
            {
                return;
            }

            IsTyping = false;
            AppendMessage(message, isUser: false);
        }

        IsTyping = false;

        if (turn.Status == ConversationStatus.WaitingForInput && turn.Prompt is { } prompt)
        {
            SetInput(prompt.Kind, prompt);
            return;
        }

        EndConversation(turn.Status);
    }

    private void SetInput(ConversationInputKind? kind, ConversationPrompt? prompt)
    {
        _inputKind = kind;
        _inputHint = prompt?.Hint;

        Choices.Clear();
        if (prompt is { Kind: ConversationInputKind.Choice })
        {
            for (int i = 0; i < prompt.Choices.Count; i++)
            {
                Choices.Add(new DialogueChoiceItem(i, prompt.Choices[i]));
            }
        }

        NotifyInputChanged();
        OnPropertyChanged(nameof(InputPlaceholder));
    }

    private void EndConversation(ConversationStatus status)
    {
        _inputKind = null;
        IsCrisis = status == ConversationStatus.Interrupted;
        FinishText = status == ConversationStatus.Completed ? AppStrings.DialogueFinish : AppStrings.DialogueClose;
        _finalStatus = status;
        IsFinished = true;
    }

    private void NotifyInputChanged() =>
        Notify(nameof(IsTextInput), nameof(IsChoiceInput), nameof(IsRatingInput));

    private void AppendMessage(string text, bool isUser) =>
        Messages.Add(new DialogueMessage(text, isUser));

    private async Task FinishAsync()
    {
        _isClosed = true;

        if (_finalStatus != ConversationStatus.Completed)
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
            preIntensity: _session?.GetRating("rating_before"),
            deleteDraft: false);
    }
}
