using Moq;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The settings screen saves what the person picks (after a pause or on "Apply"), applies it, and puts the old look back when they leave without saving.</summary>
public sealed class SettingsViewModelTests
{
    private readonly Mock<IUserPreferencesStore> _store = new();
    private readonly Mock<IDialogService> _dialogs = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<ILanguageContentReloader> _reloader = new();
    private readonly Mock<IPracticeReminderCoordinator> _practice = new();
    private readonly Mock<IQuoteReminderCoordinator> _quotes = new();
    private readonly Mock<IMoodReminderCoordinator> _mood = new();
    private readonly Mock<IChatReminderCoordinator> _chat = new();
    private readonly List<UserPreferencesState> _saved = [];
    private UserPreferencesState _state = new();

    private SettingsViewModel Create(bool remindersSupported = true)
    {
        _store.Setup(s => s.Load()).Returns(() => _state);
        _store.Setup(s => s.Save(It.IsAny<UserPreferencesState>())).Callback<UserPreferencesState>(s =>
        {
            _saved.Add(s);
            _state = s;
        });
        _reloader.Setup(r => r.EnsureReloadedAsync()).Returns(Task.CompletedTask);
        _practice.Setup(c => c.SyncAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _quotes.Setup(c => c.SyncAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mood.Setup(c => c.SyncAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _chat.Setup(c => c.SyncAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _dialogs.Setup(d => d.ShowAsync(It.IsAny<string?>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.ShowOnboardingAsync()).Returns(Task.CompletedTask);
        Mock<IPracticeReminderScheduler> scheduler = new();
        scheduler.SetupGet(s => s.IsSupported).Returns(remindersSupported);

        return new SettingsViewModel(
            _dialogs.Object,
            _navigation.Object,
            _store.Object,
            new PsychologyApp.Presentation.Features.ManageProfile.SettingsPreferencesPresenter(),
            _reloader.Object,
            _practice.Object,
            _quotes.Object,
            _mood.Object,
            _chat.Object,
            scheduler.Object);
    }

    [Fact]
    public void TheSavedChoicesAreShownWhenTheScreenOpens()
    {
        _state = new UserPreferencesState { Language = "en", Theme = "dark", Color = "green", Form = "square", Size = "large", IsBold = true, QuestionnaireAutoAdvance = false };

        SettingsViewModel viewModel = Create();

        Assert.Equal("en", viewModel.Language);
        Assert.Equal("dark", viewModel.Theme);
        Assert.Equal("green", viewModel.Color);
        Assert.Equal("square", viewModel.Form);
        Assert.Equal("large", viewModel.Size);
        Assert.True(viewModel.IsThick);
        Assert.False(viewModel.QuestionnaireAutoAdvance);
    }

    [Fact]
    public void RemindersAreHiddenWhereTheyAreNotSupported()
    {
        Assert.True(Create(remindersSupported: true).AreRemindersSupported);
        Assert.False(Create(remindersSupported: false).AreRemindersSupported);
    }

    [Fact]
    public async Task APickedLanguageIsSavedAfterAPauseAndEverythingIsRefreshed()
    {
        SettingsViewModel viewModel = Create();

        viewModel.Language = "English";
        Assert.Empty(_saved);
        await VmTestHelpers.WaitUntilAsync(() => _saved.Count == 1, 3000);
        await VmTestHelpers.WaitUntilAsync(() => _chat.Invocations.Count > 0, 3000);

        Assert.Equal("en", _saved[0].Language);
        _store.Verify(s => s.ApplyAll(), Times.AtLeastOnce);
        _reloader.Verify(r => r.EnsureReloadedAsync(), Times.Once);
        _practice.Verify(c => c.SyncAsync(It.IsAny<CancellationToken>()), Times.Once);
        _quotes.Verify(c => c.SyncAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mood.Verify(c => c.SyncAsync(It.IsAny<CancellationToken>()), Times.Once);
        _chat.Verify(c => c.SyncAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SeveralQuickChangesAreSavedOnceWithTheLastValues()
    {
        SettingsViewModel viewModel = Create();

        viewModel.Theme = "dark";
        await Task.Delay(100);
        viewModel.Color = "red";
        await Task.Delay(100);
        viewModel.Size = "large";
        await VmTestHelpers.WaitUntilAsync(() => _saved.Count >= 1, 3000);
        await Task.Delay(300);

        UserPreferencesState only = Assert.Single(_saved);
        Assert.Equal(("dark", "red", "large"), (only.Theme, only.Color, only.Size));
    }

    [Fact]
    public async Task EnglishDisplayTextsAreStoredAsTheirKeys()
    {
        SettingsViewModel viewModel = Create();

        viewModel.Form = "Square corners";
        viewModel.Theme = "Light";
        await VmTestHelpers.WaitUntilAsync(() => _saved.Count == 1, 3000);

        Assert.Equal("square", _saved[0].Form);
        Assert.Equal("light", _saved[0].Theme);
    }

    [Fact]
    public async Task ApplySavesAtOnceTellsThePersonAndGoesBack()
    {
        SettingsViewModel viewModel = Create();
        viewModel.Color = "yellow";

        await VmTestHelpers.RunAsync(viewModel.ApplyCommand);

        Assert.Equal("yellow", Assert.Single(_saved).Color);
        _dialogs.Verify(d => d.ShowAsync(It.IsAny<string?>(), It.IsAny<string>()), Times.Once);
        _navigation.Verify(n => n.GoBackAsync(), Times.Once);

        await Task.Delay(1000);
        Assert.Single(_saved);
    }

    [Fact]
    public async Task LeavingWithoutApplyingCancelsThePendingSaveAndRestoresTheSavedLook()
    {
        SettingsViewModel viewModel = Create();
        viewModel.Theme = "dark";

        await VmTestHelpers.RunAsync(viewModel.Finish);
        await Task.Delay(1000);

        Assert.Empty(_saved);
        _store.Verify(s => s.ApplyAll(), Times.Once);
        _navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task ReplayingTheOnboardingResetsItAndOpensIt()
    {
        SettingsViewModel viewModel = Create();

        await VmTestHelpers.RunAsync(viewModel.ReplayOnboardingCommand);

        _store.Verify(s => s.ResetOnboardingCompletion(), Times.Once);
        _navigation.Verify(n => n.ShowOnboardingAsync(), Times.Once);
    }

    [Fact]
    public async Task ReminderChoicesAreSavedAsHours()
    {
        SettingsViewModel viewModel = Create();

        viewModel.QuoteRemindersEnabled = true;
        viewModel.QuoteReminderHour = "10:00";
        viewModel.PracticeReminderHour = "21:00";
        await VmTestHelpers.WaitUntilAsync(() => _saved.Count == 1, 3000);

        Assert.True(_saved[0].QuoteRemindersEnabled);
        Assert.Equal(10, _saved[0].QuoteReminderHour);
        Assert.Equal(21, _saved[0].PracticeReminderHour);
    }
}
