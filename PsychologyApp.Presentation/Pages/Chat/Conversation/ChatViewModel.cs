using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.Chat.Conversation;

/// <param name="TimeText">Clock time shown inside the bubble.</param>
/// <param name="DateText">Set on the first message of a day: the divider shown above it ("Today", "21 September").</param>
/// <param name="StartsGroup">First bubble of a run from the same sender: gets more space above it, like in real messengers.</param>
public sealed record ChatBubble(string Text, bool IsUser, string TimeText, string? DateText = null, bool StartsGroup = true)
{
    public LayoutOptions Alignment => IsUser ? LayoutOptions.End : LayoutOptions.Start;

    /// <summary>True for a message that has just arrived: the page slides it in once, then clears the flag. History is never animated.</summary>
    public bool Animate { get; set; }

    public bool HasDate => DateText is not null;

    public Thickness OuterMargin => new(0, StartsGroup ? 10 : 2, 0, 0);
}

/// <summary>A suggestion chip under the last companion message. Carries its own command so the template needs no ancestor binding.</summary>
public sealed record QuickReplyItem(ChatQuickReply Reply, ICommand Command)
{
    public string Label => Reply.Label;
}

/// <summary>One conversation in the messenger. Every message is persisted by <see cref="IChatService"/> before it is shown, so leaving the page never loses anything.</summary>
public sealed class ChatViewModel : BaseViewModel
{
    private const int MinTypingDelayMs = 500;
    private const int MaxTypingDelayMs = 1500;
    private const int PerCharacterDelayMs = 8;
    private const int HandOverDelayMs = 500;

    private readonly IChatService _chat;
    private readonly IChatLanguageProvider _language;
    private readonly TimeProvider _time;
    private readonly CancellationTokenSource _lifetime = new();

    private long? _sessionId;
    private Task? _loading;
    private bool _isLoading = true;
    private bool _busy;
    private string _title = string.Empty;
    private string _draftText = string.Empty;
    private bool _isTyping;
    private DateTime? _lastCreatedUtc;
    private bool? _lastWasUser;

    public ChatViewModel(
        IChatService chat,
        INavigationService navigationService,
        IChatLanguageProvider language,
        TimeProvider time,
        long? sessionId)
    {
        _chat = chat;
        _language = language;
        _time = time;
        _sessionId = sessionId;
        _title = AppStrings.ChatNew;

        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(GoBackAsync);
        OpenProfileCommand = new AsyncCommand(() => NavigationService!.GoToCompanionProfileAsync());
        SendCommand = new Command(() => Run(SendDraftAsync));
        QuickReplyCommand = new Command<ChatQuickReply>(reply => Run(() => SendQuickReplyAsync(reply)));
    }

    public RangeObservableCollection<ChatBubble> Messages { get; } = [];

    public ObservableCollection<QuickReplyItem> QuickReplies { get; } = [];

    public ICommand BackCommand { get; }
    public ICommand OpenProfileCommand { get; }
    public ICommand SendCommand { get; }
    public ICommand QuickReplyCommand { get; }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string DraftText
    {
        get => _draftText;
        set
        {
            if (SetProperty(ref _draftText, value))
            {
                OnPropertyChanged(nameof(CanSend));
            }
        }
    }

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

    public bool HasQuickReplies => QuickReplies.Count > 0;

    /// <summary>True until the history has loaded once. The page shows a loading state instead of a blank list.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                OnPropertyChanged(nameof(HasLoaded));
            }
        }
    }

    public bool HasLoaded => !IsLoading;
    public string LoadingText => AppStrings.ChatLoadingText;

    /// <summary>The send button is lit only when there is something to send.</summary>
    public bool CanSend => !string.IsNullOrWhiteSpace(_draftText);

    /// <summary>Subtitle under the chat name: "typing..." while the companion answers, otherwise a privacy reminder.</summary>
    public string StatusText => IsTyping ? AppStrings.DialogueTyping : AppStrings.ChatStatusIdle;

    public string OpenProfileText => AppStrings.ChatProfileOpen;
    public string Placeholder => AppStrings.ChatInputPlaceholder;
    public string SendText => AppStrings.Send;
    public string TypingText => AppStrings.DialogueTyping;
    public string ScrollToNewestText => AppStrings.ChatScrollToNewest;
    public string ScrollToOldestText => AppStrings.ChatScrollToOldest;

    /// <summary>Loads the history (or starts a new chat) once; safe to call on every appearing, also concurrently. A failed load can be retried.</summary>
    public async Task LoadAsync()
    {
        Task load = _loading ??= LoadCoreAsync();
        try
        {
            await load;
        }
        catch
        {
            _loading = null;
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        long id = _sessionId ?? (await _chat.StartNewChatAsync(_lifetime.Token)).Session.Id;

        // The session and its history do not depend on each other: read both at once instead of waiting twice.
        Task<ChatSessionDTO?> sessionTask = _chat.GetChatAsync(id, _lifetime.Token);
        Task<IReadOnlyList<ChatMessageDTO>> historyTask = _chat.GetMessagesAsync(id, _lifetime.Token);
        await Task.WhenAll(sessionTask, historyTask);

        ChatSessionDTO? session = sessionTask.Result;
        if (session is null)
        {
            return;
        }

        IReadOnlyList<ChatMessageDTO> history = historyTask.Result;

        // The state changes only after everything has been read, so a failed load leaves nothing half-done.
        _sessionId = id;
        Title = session.Title;
        Messages.AddRange(history.Select(m => CreateBubble(m.Text, m.Role == ChatRole.User, m.CreatedAt, animate: false)).ToList());

        ChatMessageDTO? last = history.LastOrDefault();
        SetQuickReplies(last is { Role: ChatRole.Companion } ? last.QuickReplies : []);
    }

    /// <summary>Called whenever the page appears: after a practice is finished the companion asks how the person feels.</summary>
    public async Task OnAppearedAsync()
    {
        if (_busy)
        {
            return;
        }

        _busy = true;
        try
        {
            await LoadAsync();
            if (_sessionId is not { } id)
            {
                return;
            }

            ChatTurnResult? followUp = await _chat.CheckPracticeFollowUpAsync(id, _lifetime.Token);
            if (followUp is not null)
            {
                Title = followUp.Session.Title;
                await RevealAsync(followUp.NewMessages);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _busy = false;
        }
    }

    /// <summary>Stops pending typing delays when the page is unloaded. Nothing is lost: messages are already stored.</summary>
    public void Close() => _lifetime.Cancel();

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(Placeholder), nameof(SendText), nameof(TypingText), nameof(StatusText), nameof(OpenProfileText),
            nameof(LoadingText), nameof(ScrollToNewestText), nameof(ScrollToOldestText));

    private void Run(Func<Task> action)
    {
        if (_busy || _sessionId is null)
        {
            return;
        }

        _busy = true;
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
            // Page closed while the companion was "typing".
        }
        catch (Exception ex)
        {
            IsTyping = false;
            AsyncCommandExtensions.DefaultErrorHandler?.Invoke(ex);
        }
        finally
        {
            _busy = false;
        }
    }

    private Task SendDraftAsync()
    {
        string text = ChatText.Capitalize(DraftText);
        if (text.Length == 0)
        {
            return Task.CompletedTask;
        }

        DraftText = string.Empty;
        return TakeTurnAsync(text, () => _chat.SendTextAsync(_sessionId!.Value, text, _lifetime.Token));
    }

    private Task SendQuickReplyAsync(ChatQuickReply reply) =>
        TakeTurnAsync(reply.Label, () => _chat.SendQuickReplyAsync(_sessionId!.Value, reply, _lifetime.Token));

    private async Task TakeTurnAsync(string userText, Func<Task<ChatTurnResult>> send)
    {
        Messages.Add(CreateBubble(userText, isUser: true, Now(), animate: true));
        SetQuickReplies([]);
        IsTyping = true;

        ChatTurnResult result = await send();
        Title = result.Session.Title;

        // NewMessages[0] is the user's own message, which is already on screen.
        await RevealAsync(result.NewMessages.Skip(1).ToList());
        await HandleActionAsync(result.Action);
    }

    private async Task RevealAsync(IReadOnlyList<ChatMessageDTO> companionMessages)
    {
        foreach (ChatMessageDTO message in companionMessages)
        {
            IsTyping = true;
            await Task.Delay(Math.Clamp(message.Text.Length * PerCharacterDelayMs, MinTypingDelayMs, MaxTypingDelayMs), _lifetime.Token);
            IsTyping = false;
            Messages.Add(CreateBubble(message.Text, message.Role == ChatRole.User, message.CreatedAt, animate: true));
        }

        IsTyping = false;
        SetQuickReplies(companionMessages.LastOrDefault()?.QuickReplies ?? []);
    }

    private async Task HandleActionAsync(DialogueAction? action)
    {
        switch (action)
        {
            case { Kind: DialogueActionKind.StartTechnique, TechniqueId: { } technique }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToTechniqueAsync(technique);
                break;

            case { Kind: DialogueActionKind.OpenCrisisHub }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToCrisisHubAsync();
                break;

            case { Kind: DialogueActionKind.OpenTests }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToTestsTabAsync();
                break;

            case { Kind: DialogueActionKind.OpenSomatic }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToPhysicsSearchAsync();
                break;

            case { Kind: DialogueActionKind.OpenPrayers }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToPrayersTabAsync();
                break;

            case { Kind: DialogueActionKind.OpenQuotes }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToQuotesTabAsync();
                break;

            case { Kind: DialogueActionKind.OpenTestHistory, TestId: { } testId }:
                await Task.Delay(HandOverDelayMs, _lifetime.Token);
                await NavigationService!.GoToTestHistoryAsync(testId, AppStrings.ChatStressTestTitle);
                break;
        }
    }

    private void SetQuickReplies(IReadOnlyList<ChatQuickReply> replies)
    {
        QuickReplies.Clear();
        foreach (ChatQuickReply reply in replies)
        {
            QuickReplies.Add(new QuickReplyItem(reply, QuickReplyCommand));
        }

        OnPropertyChanged(nameof(HasQuickReplies));
    }

    /// <summary>Builds a bubble with a day divider above the first message of a day and tighter spacing inside a run from one sender. Bubbles must be created in display order.</summary>
    private ChatBubble CreateBubble(string text, bool isUser, DateTime createdUtc, bool animate)
    {
        bool newDay = _lastCreatedUtc is not { } last || !ChatTimeFormatter.SameDay(last, createdUtc);
        ChatBubble bubble = new(
            text,
            isUser,
            ChatTimeFormatter.Clock(createdUtc),
            newDay ? ChatTimeFormatter.DayLabel(createdUtc, Now(), _language.IsEnglish) : null,
            StartsGroup: newDay || _lastWasUser != isUser)
        {
            Animate = animate
        };
        _lastCreatedUtc = createdUtc;
        _lastWasUser = isUser;
        return bubble;
    }

    private DateTime Now() => _time.GetUtcNow().UtcDateTime;
}
