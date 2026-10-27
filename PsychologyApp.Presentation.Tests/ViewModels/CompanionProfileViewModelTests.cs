using Moq;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The companion's profile holds the two destructive actions of the chat (forget what it knows, delete every chat).</summary>
[Collection("Localization")]
public sealed class CompanionProfileViewModelTests
{
    private readonly Mock<IChatService> _chat = new();
    private readonly Mock<IDialogService> _dialogs = new();
    private readonly Mock<INavigationService> _navigation = new();
    private CompanionProfile _profile = CompanionProfile.Empty with { UserName = "Sam", Chats = 3, UserMessages = 12, Days = 4, StreakDays = 2 };

    private async Task<CompanionProfileViewModel> CreateAsync()
    {
        _chat.Setup(c => c.GetProfileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => _profile);
        _navigation.Setup(n => n.GoToRootAsync()).Returns(Task.CompletedTask);
        Mock<IChatLanguageProvider> language = new();
        language.SetupGet(l => l.IsEnglish).Returns(true);
        CompanionProfileViewModel viewModel = new(_chat.Object, _navigation.Object, language.Object, _dialogs.Object);
        await viewModel.RefreshAsync();
        return viewModel;
    }

    private void Confirm(bool answer) =>
        _dialogs.Setup(d => d.AskAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(answer);

    [Fact]
    public async Task TheNameAndTheHistoryAreShown()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();

        Assert.True(viewModel.HasName);
        Assert.Equal("Sam", viewModel.NameText);
        Assert.True(viewModel.HasLoaded);
        Assert.True(viewModel.HasHistory);
    }

    [Fact]
    public async Task WithoutANameTheEmptyTextIsShown()
    {
        _profile = CompanionProfile.Empty;

        CompanionProfileViewModel viewModel = await CreateAsync();

        Assert.False(viewModel.HasName);
        Assert.Equal(AppStrings.ChatProfileNameEmpty, viewModel.NameText);
        Assert.False(viewModel.HasHistory);
    }

    [Fact]
    public async Task ForgettingNeedsAConfirmation()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        Confirm(false);

        await VmTestHelpers.RunAsync(viewModel.ForgetCommand);

        _chat.Verify(c => c.ForgetMemoryAsync(It.IsAny<CancellationToken>()), Times.Never);

        Confirm(true);
        await VmTestHelpers.RunAsync(viewModel.ForgetCommand);

        _chat.Verify(c => c.ForgetMemoryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletingEveryChatNeedsAConfirmationAndThenGoesBackToTheStart()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        Confirm(false);

        await VmTestHelpers.RunAsync(viewModel.DeleteAllCommand);

        _chat.Verify(c => c.DeleteAllChatsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _navigation.Verify(n => n.GoToRootAsync(), Times.Never);

        Confirm(true);
        await VmTestHelpers.RunAsync(viewModel.DeleteAllCommand);

        _chat.Verify(c => c.DeleteAllChatsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _navigation.Verify(n => n.GoToRootAsync(), Times.Once);
    }

    [Fact]
    public async Task EditingTheNameSavesTheEnteredNameAndALeftBlankClearsIt()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        string? answer = "Alex";
        viewModel.PromptAsync = (_, _, _, _, _) => Task.FromResult<string?>(answer);
        _chat.Setup(c => c.SetUserNameAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.EditNameCommand);
        answer = null;
        await VmTestHelpers.RunAsync(viewModel.EditNameCommand);

        _chat.Verify(c => c.SetUserNameAsync("Alex", It.IsAny<CancellationToken>()), Times.Once);
        _chat.Verify(c => c.SetUserNameAsync(null, It.IsAny<CancellationToken>()), Times.Never);
        _chat.Verify(c => c.SetUserNameAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NewChatOpensAnEmptyConversation()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        _navigation.Setup(n => n.GoToChatAsync(It.IsAny<long?>())).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.NewChatCommand);

        _navigation.Verify(n => n.GoToChatAsync(null), Times.Once);
    }
}
