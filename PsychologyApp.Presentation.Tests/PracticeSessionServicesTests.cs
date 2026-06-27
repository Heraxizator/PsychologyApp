using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using System.Text.Json;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TechniqueSessionCompletionServiceTests
{
    [Fact]
    public async Task CompleteStandardSessionAsync_RecordsProgress_DeletesDraft_AndNavigates()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        Mock<IDialogService> dialog = new();
        dialog.Setup(d => d.AskAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(false);
        TechniqueSessionCompletionService service = new();
        DateTime startedAt = DateTime.UtcNow.AddMinutes(-2);

        await service.CompleteStandardSessionAsync(
            progress.Object,
            navigation.Object,
            dialog.Object,
            "Paper",
            "Practice",
            "Paper technique",
            startedAt);

        progress.Verify(
            p => p.RecordTechniqueCompletionAsync(
                "Paper",
                "Practice",
                "Paper technique",
                It.IsInRange(110, 130, Moq.Range.Inclusive),
                It.IsAny<CancellationToken>()),
            Times.Once);
        progress.Verify(p => p.DeleteSessionDraftAsync("Paper", It.IsAny<CancellationToken>()), Times.Once);
        dialog.Verify(
            d => d.AskAsync(
                AppStrings.PracticeCompletedTitle,
                AppStrings.PracticeCompletedBody(3),
                AppStrings.PracticeGoHomeButton,
                AppStrings.PracticeMoreButton),
            Times.Once);
        navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task CompleteStandardSessionAsync_SkipsDraftDelete_WhenDisabled()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToRootAsync()).Returns(Task.CompletedTask);
        Mock<IDialogService> dialog = new();
        dialog.Setup(d => d.AskAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);
        TechniqueSessionCompletionService service = new();

        await service.CompleteStandardSessionAsync(
            progress.Object,
            navigation.Object,
            dialog.Object,
            "custom_5",
            "Practice",
            "Custom",
            DateTime.UtcNow,
            deleteDraft: false);

        progress.Verify(p => p.DeleteSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        navigation.Verify(n => n.GoToRootAsync(), Times.Once);
    }
}

public sealed class EntryDraftCoordinatorTests
{
    [Fact]
    public async Task LoadAsync_RestoresSavedFieldValues()
    {
        TechniqueId techniqueId = TechniqueId.Spin;
        string key = techniqueId.ToString();
        string json = JsonSerializer.Serialize(new { Fields = new Dictionary<string, string> { ["0"] = "Saved text" } });
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetSessionDraftAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(json);
        List<EntryItem> entries = [new EntryItem { Title = "Episode" }];
        EntryDraftCoordinator coordinator = new(NullLogger<EntryDraftCoordinator>.Instance);
        coordinator.Attach(techniqueId, entries, progress.Object);

        await coordinator.LoadAsync(() => { });

        Assert.Equal("Saved text", entries[0].Text);
    }

    [Fact]
    public async Task SaveAsync_PersistsNonEmptyFields()
    {
        TechniqueId techniqueId = TechniqueId.Spin;
        string key = techniqueId.ToString();
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.SaveSessionDraftAsync(key, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        List<EntryItem> entries =
        [
            new EntryItem { Title = "Episode", Text = "Draft value" },
            new EntryItem { Title = "Feeling" }
        ];
        EntryDraftCoordinator coordinator = new(NullLogger<EntryDraftCoordinator>.Instance);
        coordinator.Attach(techniqueId, entries, progress.Object);

        await coordinator.SaveAsync();

        progress.Verify(
            p => p.SaveSessionDraftAsync(
                key,
                It.Is<string>(payload => payload.Contains("Draft value", StringComparison.Ordinal)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

public sealed class CustomTechniqueSessionOperationsTests
{
    [Fact]
    public async Task RemoveAsync_WhenConfirmed_DeletesAndSendsRemoveMessage()
    {
        Mock<ITechniqueService> techniqueService = new();
        TechniqueDTO dto = new() { TechniqueId = 9, Header = "Custom" };
        techniqueService.Setup(s => s.GetTechniqueByIdAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        techniqueService.Setup(s => s.DeleteTechniqueAsync(dto, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        Mock<ITechniqueMessenger> messenger = new();
        Mock<IDialogService> dialog = new();
        dialog.Setup(d => d.AskAsync(It.IsAny<string>(), AppStrings.PracticeDeleteConfirm, AppStrings.Yes, AppStrings.No))
            .ReturnsAsync(true);
        CustomTechniqueSessionOperations operations = new();

        bool removed = await operations.RemoveAsync(
            9,
            techniqueService.Object,
            messenger.Object,
            dialog.Object,
            CancellationToken.None);

        Assert.True(removed);
        techniqueService.Verify(s => s.DeleteTechniqueAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
        messenger.Verify(
            m => m.Send(It.Is<TechniqueMessage>(msg => msg.MessageType == TechniqueMessageType.Remove && msg.Technique.TechniqueId == 9)),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenCancelled_DoesNotDelete()
    {
        Mock<ITechniqueService> techniqueService = new();
        Mock<ITechniqueMessenger> messenger = new();
        Mock<IDialogService> dialog = new();
        dialog.Setup(d => d.AskAsync(It.IsAny<string>(), AppStrings.PracticeDeleteConfirm, AppStrings.Yes, AppStrings.No))
            .ReturnsAsync(false);
        CustomTechniqueSessionOperations operations = new();

        bool removed = await operations.RemoveAsync(
            9,
            techniqueService.Object,
            messenger.Object,
            dialog.Object,
            CancellationToken.None);

        Assert.False(removed);
        techniqueService.Verify(s => s.DeleteTechniqueAsync(It.IsAny<TechniqueDTO>(), It.IsAny<CancellationToken>()), Times.Never);
        messenger.Verify(m => m.Send(It.IsAny<TechniqueMessage>()), Times.Never);
    }
}
