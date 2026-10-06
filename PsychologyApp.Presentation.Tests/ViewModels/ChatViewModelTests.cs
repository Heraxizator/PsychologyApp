using Moq;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Presentation.Pages.Chat.Conversation;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The conversation screen: what is shown, what is sent, and where a companion action leads.</summary>
[Collection("AsyncErrorHooks")]
public sealed class ChatViewModelTests
{
    private static readonly DateTime Now = new(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc);

    private sealed class FixedTime : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(Now, TimeSpan.Zero);
    }

    private readonly Mock<IChatService> _chat = new();
    private readonly Mock<INavigationService> _navigation = new();

    private ChatViewModel Create(long? sessionId = 5)
    {
        Mock<IChatLanguageProvider> language = new();
        language.SetupGet(l => l.IsEnglish).Returns(false);
        _navigation.Setup(n => n.GoToCrisisHubAsync()).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToTechniqueAsync(It.IsAny<TechniqueId>())).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToTestsTabAsync()).Returns(Task.CompletedTask);
        return new ChatViewModel(_chat.Object, _navigation.Object, language.Object, new FixedTime(), sessionId);
    }

    private void History(params ChatMessageDTO[] messages)
    {
        _chat.Setup(c => c.GetChatAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(new ChatSessionDTO { Id = 5, Title = "My chat" });
        _chat.Setup(c => c.GetMessagesAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(messages);
    }

    private static ChatMessageDTO Message(ChatRole role, string text, DateTime at, params ChatQuickReply[] chips) =>
        new() { SessionId = 5, Role = role, Text = text, CreatedAt = at, QuickReplies = chips };

    private static ChatTurnResult Turn(DialogueAction? action, params string[] companionTexts) =>
        new(
            new ChatSessionDTO { Id = 5, Title = "My chat" },
            [Message(ChatRole.User, "user", Now), .. companionTexts.Select(t => Message(ChatRole.Companion, t, Now))],
            action);

    [Fact]
    public async Task TheHistoryBecomesBubblesInOrderWithADividerAndGrouping()
    {
        History(
            Message(ChatRole.Companion, "hello", Now.AddDays(-1)),
            Message(ChatRole.Companion, "how are you", Now.AddDays(-1).AddMinutes(1)),
            Message(ChatRole.User, "tired", Now.AddMinutes(-10)));
        ChatViewModel viewModel = Create();

        await viewModel.LoadAsync();

        Assert.Equal(["hello", "how are you", "tired"], viewModel.Messages.Select(m => m.Text));
        Assert.Equal("My chat", viewModel.Title);
        Assert.True(viewModel.Messages[0].HasDate);
        Assert.False(viewModel.Messages[1].HasDate);
        Assert.True(viewModel.Messages[2].HasDate);
        Assert.True(viewModel.Messages[0].StartsGroup);
        Assert.False(viewModel.Messages[1].StartsGroup);
        Assert.True(viewModel.Messages[2].StartsGroup);
        Assert.All(viewModel.Messages, m => Assert.False(m.Animate));
        Assert.True(viewModel.HasLoaded);
    }

    [Fact]
    public async Task OnlyTheLastCompanionMessageOffersChips()
    {
        ChatQuickReply chip = new(ChatQuickReplyKinds.Emotion, "Anxiety", "Anxiety");
        History(Message(ChatRole.Companion, "pick", Now, chip));
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();
        Assert.True(viewModel.HasQuickReplies);
        Assert.Equal("Anxiety", Assert.Single(viewModel.QuickReplies).Label);

        History(Message(ChatRole.Companion, "pick", Now, chip), Message(ChatRole.User, "thanks", Now.AddMinutes(1)));
        ChatViewModel afterUser = Create();
        await afterUser.LoadAsync();
        Assert.False(afterUser.HasQuickReplies);
    }

    [Fact]
    public async Task WithoutASessionANewChatIsStartedAndShown()
    {
        _chat.Setup(c => c.StartNewChatAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatTurnResult(new ChatSessionDTO { Id = 5, Title = "New" }, [], null));
        History(Message(ChatRole.Companion, "hi", Now));
        ChatViewModel viewModel = Create(sessionId: null);

        await viewModel.LoadAsync();

        _chat.Verify(c => c.StartNewChatAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(["hi"], viewModel.Messages.Select(m => m.Text));
    }

    [Fact]
    public async Task LoadingTwiceAtOnceReadsTheHistoryOnce()
    {
        History(Message(ChatRole.Companion, "hi", Now));
        ChatViewModel viewModel = Create();

        await Task.WhenAll(viewModel.LoadAsync(), viewModel.LoadAsync());
        await viewModel.LoadAsync();

        _chat.Verify(c => c.GetMessagesAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Single(viewModel.Messages);
    }

    [Fact]
    public async Task AFailedLoadCanBeRetried()
    {
        int calls = 0;
        _chat.Setup(c => c.GetChatAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(new ChatSessionDTO { Id = 5, Title = "My chat" });
        _chat.Setup(c => c.GetMessagesAsync(5, It.IsAny<CancellationToken>()))
            .Returns(() => ++calls == 1 ? throw new InvalidOperationException("db") : Task.FromResult<IReadOnlyList<ChatMessageDTO>>([Message(ChatRole.Companion, "hi", Now)]));
        ChatViewModel viewModel = Create();

        await Assert.ThrowsAsync<InvalidOperationException>(viewModel.LoadAsync);
        Assert.Empty(viewModel.Messages);

        await viewModel.LoadAsync();
        Assert.Single(viewModel.Messages);
    }

    [Fact]
    public async Task SendingCapitalisesTheDraftShowsItAtOnceAndAddsTheAnswer()
    {
        History();
        string? sent = null;
        _chat.Setup(c => c.SendTextAsync(5, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<long, string, CancellationToken>((_, text, _) => sent = text)
            .ReturnsAsync(Turn(null, "I hear you"));
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();
        viewModel.DraftText = "i feel low";
        Assert.True(viewModel.CanSend);

        await VmTestHelpers.RunAsync(viewModel.SendCommand);
        await VmTestHelpers.WaitUntilAsync(() => viewModel.Messages.Count == 2 && !viewModel.IsTyping, 5000);

        Assert.Equal("I feel low", sent);
        Assert.Equal(string.Empty, viewModel.DraftText);
        Assert.False(viewModel.CanSend);
        Assert.Equal(["I feel low", "I hear you"], viewModel.Messages.Select(m => m.Text));
        Assert.True(viewModel.Messages[0].IsUser);
        Assert.False(viewModel.Messages[1].IsUser);
        Assert.True(viewModel.Messages[1].Animate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ABlankDraftSendsNothing(string draft)
    {
        History();
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();
        viewModel.DraftText = draft;

        viewModel.SendCommand.Execute(null);
        await Task.Delay(100);

        _chat.Verify(c => c.SendTextAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Empty(viewModel.Messages);
    }

    [Fact]
    public async Task ACrisisActionOpensTheCrisisScreenAfterTheAnswer()
    {
        History();
        _chat.Setup(c => c.SendTextAsync(5, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Turn(new DialogueAction(DialogueActionKind.OpenCrisisHub), "You are not alone"));
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();
        viewModel.DraftText = "help";

        await VmTestHelpers.RunAsync(viewModel.SendCommand);
        await VmTestHelpers.WaitUntilAsync(() => _navigation.Invocations.Any(i => i.Method.Name == nameof(INavigationService.GoToCrisisHubAsync)), 5000);

        _navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Once);
    }

    [Fact]
    public async Task AnOfferedPracticeStartsThatPractice()
    {
        History();
        _chat.Setup(c => c.SendTextAsync(5, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Turn(new DialogueAction(DialogueActionKind.StartTechnique, TechniqueId: TechniqueId.Grounding), "Let us try"));
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();
        viewModel.DraftText = "ok";

        await VmTestHelpers.RunAsync(viewModel.SendCommand);
        await VmTestHelpers.WaitUntilAsync(() => _navigation.Invocations.Any(i => i.Method.Name == nameof(INavigationService.GoToTechniqueAsync)), 5000);

        _navigation.Verify(n => n.GoToTechniqueAsync(TechniqueId.Grounding), Times.Once);
    }

    [Fact]
    public async Task AFailedSendStopsTheTypingIndicatorReportsTheErrorAndAllowsTryingAgain()
    {
        History();
        _chat.SetupSequence(c => c.SendTextAsync(5, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db"))
            .ReturnsAsync(Turn(null, "back"));
        List<Exception> reported = [];
        Action<Exception>? previous = AsyncCommandExtensions.DefaultErrorHandler;
        AsyncCommandExtensions.DefaultErrorHandler = reported.Add;
        try
        {
            ChatViewModel viewModel = Create();
            await viewModel.LoadAsync();

            viewModel.DraftText = "first";
            await VmTestHelpers.RunAsync(viewModel.SendCommand);
            await VmTestHelpers.WaitUntilAsync(() => reported.Count == 1);
            Assert.False(viewModel.IsTyping);

            viewModel.DraftText = "second";
            await VmTestHelpers.RunAsync(viewModel.SendCommand);
            await VmTestHelpers.WaitUntilAsync(() => viewModel.Messages.Any(m => m.Text == "back"), 5000);
        }
        finally
        {
            AsyncCommandExtensions.DefaultErrorHandler = previous;
        }
    }

    [Fact]
    public async Task ATappedChipIsSentAsAQuickReply()
    {
        ChatQuickReply chip = new(ChatQuickReplyKinds.Emotion, "Anxiety", "Anxiety");
        History(Message(ChatRole.Companion, "pick", Now, chip));
        ChatQuickReply? sent = null;
        _chat.Setup(c => c.SendQuickReplyAsync(5, It.IsAny<ChatQuickReply>(), It.IsAny<CancellationToken>()))
            .Callback<long, ChatQuickReply, CancellationToken>((_, reply, _) => sent = reply)
            .ReturnsAsync(Turn(null, "Understood"));
        ChatViewModel viewModel = Create();
        await viewModel.LoadAsync();

        viewModel.QuickReplies[0].Command.Execute(chip);
        await VmTestHelpers.WaitUntilAsync(() => sent is not null, 5000);

        Assert.Equal(chip, sent);
        Assert.False(viewModel.HasQuickReplies);
    }
}
