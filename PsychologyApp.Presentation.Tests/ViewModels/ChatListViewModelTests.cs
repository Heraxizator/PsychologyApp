using Moq;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

public sealed class ChatListViewModelTests
{
    private static readonly DateTime Now = new(2026, 10, 6, 12, 0, 0, DateTimeKind.Utc);

    private sealed class FixedTime : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(Now, TimeSpan.Zero);
    }

    private readonly Mock<IChatService> _chat = new();
    private readonly Mock<IDialogService> _dialogs = new();
    private readonly Mock<INavigationService> _navigation = new();
    private List<ChatSessionDTO> _sessions = [];

    private ChatListViewModel Create()
    {
        _chat.Setup(c => c.GetChatsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => _sessions);
        Mock<IChatLanguageProvider> language = new();
        language.SetupGet(l => l.IsEnglish).Returns(false);
        return new ChatListViewModel(_chat.Object, _navigation.Object, language.Object, _dialogs.Object, new FixedTime());
    }

    private static ChatSessionDTO Session(long id, string title = "Chat", string? preview = "hello", int? first = null, int? last = null, bool spoken = true) =>
        new()
        {
            Id = id,
            Title = title,
            Preview = preview,
            UpdatedAt = Now.AddMinutes(-5),
            FirstIntensity = first,
            LastIntensity = last,
            StateJson = new CompanionState { Turns = spoken ? 1 : 0 }.Serialize()
        };

    [Fact]
    public async Task OnlyChatsWhereSomethingWasSaidAreListed()
    {
        _sessions = [Session(1), Session(2, spoken: false), Session(3)];
        ChatListViewModel viewModel = Create();

        await viewModel.RefreshAsync();

        Assert.Equal([1L, 3L], viewModel.Chats.Select(c => c.Id));
        Assert.False(viewModel.IsEmpty);
    }

    [Fact]
    public async Task NoChatsMeansTheEmptyState()
    {
        _sessions = [Session(1, spoken: false)];
        ChatListViewModel viewModel = Create();

        await viewModel.RefreshAsync();

        Assert.True(viewModel.IsEmpty);
        Assert.Empty(viewModel.Chats);
    }

    [Fact]
    public async Task ComingBackWithNothingChangedKeepsTheSameRows()
    {
        _sessions = [Session(1), Session(2)];
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();
        ChatListItem[] before = [.. viewModel.Chats];

        await viewModel.RefreshAsync();

        Assert.True(before.SequenceEqual(viewModel.Chats));
    }

    [Fact]
    public async Task AChangedRowIsRebuilt()
    {
        _sessions = [Session(1, preview: "old")];
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();

        _sessions = [Session(1, preview: "new")];
        await viewModel.RefreshAsync();

        Assert.Equal("new", Assert.Single(viewModel.Chats).Preview);
    }

    [Theory]
    [InlineData(8, 4, "8 → 4", true, false)]
    [InlineData(3, 7, "3 → 7", false, true)]
    [InlineData(5, 5, "5", false, false)]
    [InlineData(null, 6, "6", false, false)]
    [InlineData(null, null, "", false, false)]
    public async Task TheTensionTrendIsShownAndFlagged(int? first, int? last, string text, bool improving, bool worsening)
    {
        _sessions = [Session(1, first: first, last: last)];
        ChatListViewModel viewModel = Create();

        await viewModel.RefreshAsync();

        ChatListItem item = Assert.Single(viewModel.Chats);
        Assert.Equal(text, item.MoodText);
        Assert.Equal(improving, item.IsImproving);
        Assert.Equal(worsening, item.IsWorsening);
        Assert.Equal(text.Length > 0, item.HasMood);
    }

    [Fact]
    public async Task ALongPreviewIsFlattenedAndCutWithAnEllipsis()
    {
        string preview = "line one\n" + new string('x', 200);
        _sessions = [Session(1, preview: preview)];
        ChatListViewModel viewModel = Create();

        await viewModel.RefreshAsync();

        string shown = Assert.Single(viewModel.Chats).Preview;
        Assert.DoesNotContain('\n', shown);
        Assert.EndsWith("…", shown);
        Assert.True(shown.Length <= 111);
    }

    [Theory]
    [InlineData("Alice", "A")]
    [InlineData("алиса", "А")]
    [InlineData("…", "•")]
    public async Task TheAvatarShowsTheFirstLetterOfTheTitle(string title, string initial)
    {
        _sessions = [Session(1, title: title)];
        ChatListViewModel viewModel = Create();

        await viewModel.RefreshAsync();

        Assert.Equal(initial, Assert.Single(viewModel.Chats).Initial);
    }

    [Fact]
    public async Task DeletingNeedsAConfirmation()
    {
        _sessions = [Session(1)];
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();
        _dialogs.Setup(d => d.AskAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        await VmTestHelpers.RunAsync(viewModel.Chats[0].DeleteCommand);

        _chat.Verify(c => c.DeleteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Single(viewModel.Chats);
    }

    [Fact]
    public async Task ADeletedChatLeavesTheListAfterTheRefresh()
    {
        _sessions = [Session(1), Session(2)];
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();
        _dialogs.Setup(d => d.AskAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _chat.Setup(c => c.DeleteAsync(1, It.IsAny<CancellationToken>())).Callback(() => _sessions = [Session(2)]).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.Chats[0].DeleteCommand);

        Assert.Equal([2L], viewModel.Chats.Select(c => c.Id));
    }

    [Fact]
    public async Task RenamingUsesTheEnteredNameAndIgnoresACancelOrBlank()
    {
        _sessions = [Session(1, title: "Old")];
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();
        string? answer = "New name";
        viewModel.PromptAsync = (_, _, _, _, _) => Task.FromResult<string?>(answer);
        _chat.Setup(c => c.RenameAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.Chats[0].RenameCommand);
        answer = null;
        await VmTestHelpers.RunAsync(viewModel.Chats[0].RenameCommand);
        answer = "   ";
        await VmTestHelpers.RunAsync(viewModel.Chats[0].RenameCommand);

        _chat.Verify(c => c.RenameAsync(1, "New name", It.IsAny<CancellationToken>()), Times.Once);
        _chat.Verify(c => c.RenameAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OpeningARowAndStartingANewChatNavigate()
    {
        _sessions = [Session(7)];
        _navigation.Setup(n => n.GoToChatAsync(It.IsAny<long?>())).Returns(Task.CompletedTask);
        ChatListViewModel viewModel = Create();
        await viewModel.RefreshAsync();

        await VmTestHelpers.RunAsync(viewModel.Chats[0].OpenCommand);
        await VmTestHelpers.RunAsync(viewModel.NewChatCommand);

        _navigation.Verify(n => n.GoToChatAsync(7L), Times.Once);
        _navigation.Verify(n => n.GoToChatAsync(null), Times.Once);
    }
}
