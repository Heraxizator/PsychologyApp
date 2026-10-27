using Moq;
using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>
/// The companion's card holds the two destructive actions of the chat (forget what it knows, delete every chat), behind
/// "Chat settings".
/// </summary>
[Collection("Localization")]
public sealed class CompanionProfileViewModelTests
{
    private readonly Mock<IChatService> _chat = new();
    private readonly Mock<IDialogService> _dialogs = new();
    private readonly Mock<INavigationService> _navigation = new();
    private CompanionProfile _profile = CompanionProfile.Empty with { UserName = "Sam", Chats = 3, UserMessages = 12, Days = 4, StreakDays = 2 };
    private IReadOnlyList<string> _offered = [];

    private async Task<CompanionProfileViewModel> CreateAsync()
    {
        _chat.Setup(c => c.GetProfileAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => _profile);
        _navigation.Setup(n => n.GoToRootAsync()).Returns(Task.CompletedTask);
        CompanionProfileViewModel viewModel = new(_chat.Object, _navigation.Object, _dialogs.Object);
        await viewModel.RefreshAsync();
        return viewModel;
    }

    private void Confirm(bool answer) =>
        _dialogs.Setup(d => d.AskAsync(It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(answer);

    private void PickInSettings(string? option) =>
        _dialogs
            .Setup(d => d.PickOptionAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>()))
            .Callback<string, IReadOnlyList<string>, string>((_, options, _) => _offered = options)
            .ReturnsAsync(option);

    // Deleting chats when there are none would end on a confirmation for nothing.
    [Fact]
    public async Task SettingsOfferDeletingChatsOnlyWhenThereAreSome()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        PickInSettings(null);

        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);
        Assert.Contains(AppStrings.ChatProfileDeleteAll, _offered);

        _profile = CompanionProfile.Empty;
        await viewModel.RefreshAsync();
        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);
        Assert.DoesNotContain(AppStrings.ChatProfileDeleteAll, _offered);
    }

    [Fact]
    public async Task ForgettingNeedsAConfirmation()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        PickInSettings(AppStrings.ChatProfileForget);
        Confirm(false);

        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);

        _chat.Verify(c => c.ForgetMemoryAsync(It.IsAny<CancellationToken>()), Times.Never);

        Confirm(true);
        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);

        _chat.Verify(c => c.ForgetMemoryAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeletingEveryChatNeedsAConfirmationAndThenGoesBackToTheStart()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        PickInSettings(AppStrings.ChatProfileDeleteAll);
        Confirm(false);

        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);

        _chat.Verify(c => c.DeleteAllChatsAsync(It.IsAny<CancellationToken>()), Times.Never);
        _navigation.Verify(n => n.GoToRootAsync(), Times.Never);

        Confirm(true);
        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);

        _chat.Verify(c => c.DeleteAllChatsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _navigation.Verify(n => n.GoToRootAsync(), Times.Once);
    }

    [Fact]
    public async Task EditingTheNameSavesTheEnteredNameAndACancelKeepsIt()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        PickInSettings(AppStrings.ChatProfileNameTitle);
        string? answer = "Alex";
        viewModel.PromptAsync = (_, _, _, _, _) => Task.FromResult<string?>(answer);
        _chat.Setup(c => c.SetUserNameAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);
        answer = null;
        await VmTestHelpers.RunAsync(viewModel.SettingsCommand);

        _chat.Verify(c => c.SetUserNameAsync("Alex", It.IsAny<CancellationToken>()), Times.Once);
        _chat.Verify(c => c.SetUserNameAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // The help tile is the page's way to urgent help; it must lead there, not just look like it.
    [Fact]
    public async Task TheHelpTileOpensUrgentHelp()
    {
        CompanionProfileViewModel viewModel = await CreateAsync();
        _navigation.Setup(n => n.GoToCrisisHubAsync()).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.CrisisCommand);

        _navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Once);
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
