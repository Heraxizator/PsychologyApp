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
    private bool _loaded;
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
        SendCommand = new Command(() => Run(SendDraftAsync));
        QuickReplyCommand = new Command<ChatQuickReply>(reply => Run(() => SendQuickReplyAsync(reply)));
    }

    public ObservableCollection<ChatBubble> Messages { get; } = [];

    public ObservableCollection<QuickReplyItem> QuickReplies { get; } = [];

    public ICommand BackCommand { get; }
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

    /// <summary>The send button is lit only when there is something to send.</summary>
    public bool CanSend => !string.IsNullOrWhiteSpace(_draftText);

    /// <summary>Subtitle under the chat name: "typing..." while the companion answers, otherwise a privacy reminder.</summary>
    public string StatusText => IsTyping ? AppStrings.DialogueTyping : AppStrings.ChatStatusIdle;

    public string Placeholder => AppStrings.ChatInputPlaceholder;
    public string SendText => AppStrings.Send;
    public string TypingText => AppStrings.DialogueTyping;

    /// <summary>Loads the history (or starts a new chat) once; safe to call on every appearing.</summary>
    public async Task LoadAsync()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        if (_sessionId is null)
        {
            _sessionId = (await _chat.StartNewChatAsync(_lifetime.Token)).Session.Id;
        }

        ChatSessionDTO? session = await _chat.GetChatAsync(_sessionId.Value, _lifetime.Token);
        if (session is null)
        {
            return;
        }

        Title = session.Title;
        IReadOnlyList<ChatMessageDTO> history = await _chat.GetMessagesAsync(_sessionId.Value, _lifetime.Token);
        foreach (ChatMessageDTO message in history)
        {
            AddBubble(message.Text, message.Role == ChatRole.User, message.CreatedAt);
        }

        ChatMessageDTO? last = history.LastOrDefault();
        SetQuickReplies(last is { Role: ChatRole.Companion } ? last.QuickReplies : []);
    }

    /// <summary>Called whenever the page appears: after a practice is finished the companion asks how the person feels.</summary>
    public async Task OnAppearedAsync()
    {
        await LoadAsync();
        if (_sessionId is not { } id || _busy)
        {
            return;
        }

        _busy = true;
        try
        {
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
        Notify(nameof(Placeholder), nameof(SendText), nameof(TypingText), nameof(StatusText));

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
        string text = DraftText.Trim();
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
        AddBubble(userText, isUser: true, Now());
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
            AddBubble(message.Text, message.Role == ChatRole.User, message.CreatedAt);
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

    /// <summary>Adds a bubble, putting a day divider above the first message of a day and tightening the spacing inside a run from one sender.</summary>
    private void AddBubble(string text, bool isUser, DateTime createdUtc)
    {
        DateTime now = Now();
        bool newDay = _lastCreatedUtc is not { } last || !ChatTimeFormatter.SameDay(last, createdUtc);
        Messages.Add(new ChatBubble(
            text,
            isUser,
            ChatTimeFormatter.Clock(createdUtc),
            newDay ? ChatTimeFormatter.DayLabel(createdUtc, now, _language.IsEnglish) : null,
            StartsGroup: newDay || _lastWasUser != isUser));
        _lastCreatedUtc = createdUtc;
        _lastWasUser = isUser;
    }

    private DateTime Now() => _time.GetUtcNow().UtcDateTime;
}
