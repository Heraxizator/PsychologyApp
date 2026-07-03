using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.Polarity;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.PaperList;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PracticeSessionViewModelTests
{
    public PracticeSessionViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public void PolarityViewModel_Add_AddsItemAndClearsFields()
    {
        PolarityViewModel viewModel = CreatePolarityViewModel();

        viewModel.Positive = "Calm";
        viewModel.Negative = "Anxiety";
        viewModel.Add.Execute(null);

        Assert.Single(viewModel.polarities);
        Assert.Equal(string.Empty, viewModel.Positive);
        Assert.Equal(string.Empty, viewModel.Negative);
        Assert.True(viewModel.IsFull);
    }

    [Fact]
    public void PaperListViewModel_Add_AddsPaperAndClearsTextWhenConfigured()
    {
        PaperListViewModel viewModel = CreatePaperListViewModel(clearTextAfterAdd: true);

        viewModel.Text = "Worry thought";
        viewModel.AddCommand.Execute(null);

        Assert.Single(viewModel.PapersObservableCollection);
        Assert.Equal(string.Empty, viewModel.Text);
        Assert.True(viewModel.IsFull);
    }

    [Fact]
    public async Task TechniqueSessionViewModel_Constructor_AttachesBeforeLoadingEntryDraft()
    {
        TaskCompletionSource draftLoadStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string key, CancellationToken _) =>
            {
                draftLoadStarted.TrySetResult();
                return Task.FromResult<string?>(null);
            });
        EntryDraftCoordinator draftCoordinator = new(NullLogger<EntryDraftCoordinator>.Instance);
        ListTechniqueSessionHelper sessionHelper = CreateSessionHelper(progress.Object);

        _ = new TechniqueSessionViewModel(
            TechniqueId.Spin,
            CreateNavigationService(),
            progress.Object,
            sessionHelper,
            draftCoordinator);

        await draftLoadStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));

        progress.Verify(
            p => p.GetSessionDraftAsync(nameof(TechniqueId.Spin), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void TechniqueSessionViewModel_MarkSessionCompleted_StopsDraftSave()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        EntryDraftCoordinator draftCoordinator = new(NullLogger<EntryDraftCoordinator>.Instance);
        ListTechniqueSessionHelper sessionHelper = CreateSessionHelper(progress.Object);

        TechniqueSessionViewModel viewModel = new(
            TechniqueId.Spin,
            CreateNavigationService(),
            progress.Object,
            sessionHelper,
            draftCoordinator);

        draftCoordinator.MarkSessionCompleted();
        viewModel.SaveEntryDraftIfNeeded();

        progress.Verify(
            p => p.SaveSessionDraftAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static PolarityViewModel CreatePolarityViewModel()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        PolarityListDraftCoordinator coordinator = new(NullLogger<PolarityListDraftCoordinator>.Instance);
        coordinator.Attach(TechniqueId.Polarity.ToString(), progress.Object);

        return new PolarityViewModel(
            CreateNavigationService(),
            progress.Object,
            CreateSessionHelper(progress.Object),
            coordinator);
    }

    private static PaperListViewModel CreatePaperListViewModel(bool clearTextAfterAdd)
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);
        PaperListDraftCoordinator coordinator = new(NullLogger<PaperListDraftCoordinator>.Instance);
        coordinator.Attach(TechniqueId.Paper.ToString(), progress.Object);

        return new PaperListViewModel(
            CreateNavigationService(),
            TechniqueId.Paper,
            clearTextAfterAdd,
            progress.Object,
            CreateSessionHelper(progress.Object),
            coordinator);
    }

    private static TestNavigationService CreateNavigationService()
    {
        Mock<INavigation> navigation = new();
        return new TestNavigationService(navigation.Object);
    }

    private static ListTechniqueSessionHelper CreateSessionHelper(IUserProgressService progress)
    {
        Mock<INavigationService> navigation = new();
        return new ListTechniqueSessionHelper(
            new TechniqueSessionCompletionService(Mock.Of<IPracticeReminderCoordinator>()),
            progress,
            navigation.Object);
    }
}
