using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Widgets.DialogueChat;

/// <summary>Chat mechanics shared by scripted technique dialogues and the free-form companion: bubbles, typing indicator and the text / choice / rating input modes.</summary>
public abstract class ChatViewModelBase : BaseViewModel
{
    private const int MinTypingDelayMs = 400;
    private const int MaxTypingDelayMs = 1400;
    private const int PerCharacterDelayMs = 8;

    private readonly CancellationTokenSource _lifetime = new();
    private IDialogueSession? _session;
    private bool _isBusy;
    private bool _isClosed;
    private bool _started;

    private ConversationInputKind? _inputKind;
    private string _draftText = string.Empty;
    private string? _inputHint;
    private bool _isTyping;
    private bool _isFinished;
    private string _finishText = string.Empty;
    private bool _offersEmotions;

    protected ChatViewModelBase(INavigationService navigationService)
    {
        BindNavigation(navigationService);

        BackCommand = new AsyncCommand(GoBackAsync);
        OpenProfileCommand = new AsyncCommand(() => NavigationService!.GoToCompanionProfileAsync());
        SendCommand = new Command(() => Run(SendTextAsync));
        ChooseCommand = new Command<int>(index => Run(() => ChooseAsync(index)));
        RateCommand = new Command<int>(rating => Run(() => RateAsync(rating)));
        SuggestCommand = new Command<DialogueSuggestionItem>(ToggleSuggestion);
        FinishCommand = new AsyncCommand(() => OnFinishAsync(FinalStatus, FinalAction));

        RatingItems = Enumerable.Range(0, 11).Select(v => new DialogueRatingItem(v, RateCommand)).ToArray();
    }

    public ObservableCollection<DialogueMessage> Messages { get; } = [];

    public ObservableCollection<DialogueChoiceItem> Choices { get; } = [];

    /// <summary>Typical answers to the current text question, tapped into the draft instead of typed.</summary>
    public ObservableCollection<DialogueSuggestionItem> Suggestions { get; } = [];

    /// <summary>The current text question is about a feeling: the emotion palette is shown above the input.</summary>
    public bool OffersEmotions
    {
        get => _offersEmotions;
        private set => SetProperty(ref _offersEmotions, value);
    }

    public bool HasSuggestions => Suggestions.Count > 0;

    /// <summary>
    /// The chips laid out in two rows that scroll sideways together: a long list (twenty things to see) shows twice as many at once
    /// without taking the conversation's screen. Up to five chips stay in one row.
    /// </summary>
    public ObservableCollection<DialogueSuggestionItem> SuggestionsFirstRow { get; } = [];

    public ObservableCollection<DialogueSuggestionItem> SuggestionsSecondRow { get; } = [];

    public bool HasSecondSuggestionRow => SuggestionsSecondRow.Count > 0;

    private const int OneRowChips = 5;

    private void LayoutSuggestionRows()
    {
        SuggestionsFirstRow.Clear();
        SuggestionsSecondRow.Clear();
        bool twoRows = Suggestions.Count > OneRowChips;
        for (int i = 0; i < Suggestions.Count; i++)
        {
            (twoRows && i % 2 == 1 ? SuggestionsSecondRow : SuggestionsFirstRow).Add(Suggestions[i]);
        }

        OnPropertyChanged(nameof(HasSecondSuggestionRow));
    }

    public IReadOnlyList<DialogueRatingItem> RatingItems { get; }

    public ICommand BackCommand { get; }

    /// <summary>The same companion as in the main chat: its profile opens from the header here too.</summary>
    public ICommand OpenProfileCommand { get; }
    public ICommand SendCommand { get; }
    public ICommand ChooseCommand { get; }
    public ICommand RateCommand { get; }
    public ICommand SuggestCommand { get; }
    public ICommand FinishCommand { get; }

    public string DraftText
    {
        get => _draftText;
        set
        {
            if (SetProperty(ref _draftText, value))
            {
                OnPropertyChanged(nameof(CanSend));
                RefreshSuggestionSelection();
            }
        }
    }

    /// <summary>There is something to send, so the send button is drawn filled.</summary>
    public bool CanSend => !string.IsNullOrWhiteSpace(_draftText);

    public string InputPlaceholder => string.IsNullOrWhiteSpace(_inputHint) ? AppStrings.DialogueInputPlaceholder : _inputHint;

    public string SendText => AppStrings.Send;

    public string TypingText => AppStrings.DialogueTyping;

    public string OpenProfileText => AppStrings.ChatProfileOpen;

    /// <summary>Under the title in the header: the typing hint, otherwise that this is a practice with the same companion.</summary>
    public string StatusText => IsTyping ? AppStrings.DialogueTyping : AppStrings.ChatStatusPractice;

    public string RatingScaleText => AppStrings.DialogueRatingScale;

    public bool IsTyping
    {
        get => _isTyping;
        private set
        {
            if (SetProperty(ref _isTyping, value))
            {
                OnPropertyChanged(nameof(StatusText));
            }
        }
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

    public string FinishText
    {
        get => _finishText;
        private set => SetProperty(ref _finishText, value);
    }

    protected ConversationStatus FinalStatus { get; private set; }

    protected DialogueAction? FinalAction { get; private set; }

    protected CancellationToken Lifetime => _lifetime.Token;

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        await OnStartingAsync();

        _session = await CreateSessionAsync();
        if (_session is null)
        {
            AppendMessage(AppStrings.DialogueLoadFailed, isUser: false);
            await EndAsync(ConversationStatus.Paused, null);
            return;
        }

        await ShowTurnAsync(await AwaitModelAsync(_session.StartAsync(Lifetime)));
    }

    /// <summary>Called when the page goes away: stops typing delays and cancels any pending model call.</summary>
    public void Close()
    {
        _isClosed = true;
        _lifetime.Cancel();
    }

    protected virtual Task OnStartingAsync() => Task.CompletedTask;

    /// <summary>The problem from the last practice, offered as the first chip of a question that asks for it; none by default.</summary>
    protected virtual Task<string?> GetRecentProblemAsync() => Task.FromResult<string?>(null);

    /// <summary>The chips of a question and their order; by default as written in the scenario. <paramref name="capture"/> names the answer.</summary>
    protected virtual IReadOnlyList<string> RankSuggestions(IReadOnlyList<string> suggestions, string? capture) => suggestions;

    /// <summary>A chip was picked into the answer.</summary>
    protected virtual void OnSuggestionChosen(string suggestion)
    {
    }

    protected abstract Task<IDialogueSession?> CreateSessionAsync();

    protected abstract string GetFinishText(ConversationStatus status, DialogueAction? action);

    protected abstract Task OnFinishAsync(ConversationStatus status, DialogueAction? action);

    /// <summary>Runs after the closing messages are shown, before the user presses the finish button.</summary>
    protected virtual Task OnEndedAsync(ConversationStatus status, DialogueAction? action) => Task.CompletedTask;

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(InputPlaceholder), nameof(SendText), nameof(TypingText), nameof(RatingScaleText));

    private void Run(Func<Task> action)
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
        catch (OperationCanceledException)
        {
            // Page closed while the model was thinking.
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
        SetInput(null, null);
        await ShowTurnAsync(await AwaitModelAsync(_session!.SubmitTextAsync(text, Lifetime)));
    }

    private async Task ChooseAsync(int index)
    {
        DialogueChoiceItem? choice = Choices.FirstOrDefault(c => c.Index == index);
        if (choice is null)
        {
            return;
        }

        AppendMessage(choice.Label, isUser: true);
        SetInput(null, null);
        await ShowTurnAsync(await AwaitModelAsync(_session!.SubmitChoiceAsync(index, Lifetime)));
    }

    private async Task RateAsync(int rating)
    {
        AppendMessage(rating.ToString(CultureInfo.InvariantCulture), isUser: true);
        SetInput(null, null);
        await ShowTurnAsync(await AwaitModelAsync(_session!.SubmitRatingAsync(rating, Lifetime)));
    }

    /// <summary>Shows the typing indicator while a turn is being produced (instant for scripts, seconds for a language model).</summary>
    private async Task<ConversationTurn> AwaitModelAsync(Task<ConversationTurn> turn)
    {
        IsTyping = true;
        try
        {
            return await turn;
        }
        finally
        {
            IsTyping = false;
        }
    }

    private async Task ShowTurnAsync(ConversationTurn turn)
    {
        foreach (string message in turn.BotMessages)
        {
            IsTyping = true;
            await Task.Delay(Math.Clamp(message.Length * PerCharacterDelayMs, MinTypingDelayMs, MaxTypingDelayMs), Lifetime);
            IsTyping = false;
            AppendMessage(message, isUser: false);
        }

        if (turn.Status == ConversationStatus.WaitingForInput && turn.Prompt is { } prompt)
        {
            SetInput(prompt.Kind, prompt);
            return;
        }

        await EndAsync(turn.Status, turn.Action);
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
                Choices.Add(new DialogueChoiceItem(i, prompt.Choices[i], ChooseCommand));
            }
        }

        Suggestions.Clear();
        foreach (string suggestion in prompt is { Kind: ConversationInputKind.Text } ? RankSuggestions(prompt.Suggestions ?? [], prompt.Capture) : [])
        {
            Suggestions.Add(new DialogueSuggestionItem(suggestion, suggestion, SuggestCommand));
        }

        OffersEmotions = prompt is { Kind: ConversationInputKind.Text, OffersEmotions: true };
        if (prompt is { Kind: ConversationInputKind.Text, OffersRecent: true })
        {
            OfferRecentProblemAsync().FireAndForget();
        }

        LayoutSuggestionRows();
        OnPropertyChanged(nameof(HasSuggestions));
        RefreshSuggestionSelection();

        NotifyInputChanged();
        OnPropertyChanged(nameof(InputPlaceholder));
    }

    private async Task EndAsync(ConversationStatus status, DialogueAction? action)
    {
        _inputKind = null;
        FinalStatus = status;
        FinalAction = action;
        FinishText = GetFinishText(status, action);
        IsFinished = true;

        await OnEndedAsync(status, action);
    }

    private void ToggleSuggestion(DialogueSuggestionItem? item)
    {
        if (item is null || _inputKind != ConversationInputKind.Text)
        {
            return;
        }

        DraftText = AnswerText.Toggle(DraftText, item.Value);
        if (AnswerText.Contains(DraftText, item.Value))
        {
            OnSuggestionChosen(item.Value);
        }
    }

    private void RefreshSuggestionSelection()
    {
        foreach (DialogueSuggestionItem item in Suggestions)
        {
            item.IsSelected = AnswerText.Contains(DraftText, item.Value);
        }
    }

    private async Task OfferRecentProblemAsync()
    {
        string? recent = await GetRecentProblemAsync();
        if (string.IsNullOrWhiteSpace(recent) || _inputKind != ConversationInputKind.Text)
        {
            return;
        }

        Suggestions.Insert(0, new DialogueSuggestionItem(AppStrings.PracticeRecentProblem(AnswerText.Shorten(recent, 32)), recent, SuggestCommand));
        LayoutSuggestionRows();
        OnPropertyChanged(nameof(HasSuggestions));
        RefreshSuggestionSelection();
    }

    private void NotifyInputChanged() =>
        Notify(nameof(IsTextInput), nameof(IsChoiceInput), nameof(IsRatingInput));

    private void AppendMessage(string text, bool isUser) =>
        Messages.Add(new DialogueMessage(text, isUser));
}
