using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class JournalScreenCoordinatorTests
{
    [Fact]
    public async Task OpenEditorDay_WhenHubBelow_SetsPendingAndGoesBack()
    {
        var editorContext = new JournalEditorContext();
        var coordinator = new JournalScreenCoordinator(editorContext);
        var navigation = new FakeNavigation();
        navigation.Stack.Add(new HubStubPage());
        navigation.Stack.Add(new ContentPage());
        var navService = new StackNavigationService(navigation);

        await coordinator.OpenEditorDayAsync(new DateOnly(2026, 7, 20), navService);

        Assert.Equal(new DateOnly(2026, 7, 20), editorContext.PendingEditorDay);
        Assert.Equal(1, navService.GoBackCalls);
        Assert.Equal(0, navService.GoToJournalCalls);
    }

    [Fact]
    public async Task OpenEditorDay_WhenNoHub_SetsPendingAndOpensJournal()
    {
        var editorContext = new JournalEditorContext();
        var coordinator = new JournalScreenCoordinator(editorContext);
        var navigation = new FakeNavigation();
        navigation.Stack.Add(new ContentPage());
        var navService = new StackNavigationService(navigation);

        await coordinator.OpenEditorDayAsync(new DateOnly(2026, 7, 21), navService);

        Assert.Equal(new DateOnly(2026, 7, 21), editorContext.PendingEditorDay);
        Assert.Equal(0, navService.GoBackCalls);
        Assert.Equal(1, navService.GoToJournalCalls);
    }

    private sealed class HubStubPage : ContentPage, IJournalHubPage;

    private sealed class FakeNavigation : INavigation
    {
        public List<Page> Stack { get; } = [];

        public IReadOnlyList<Page> NavigationStack => Stack;
        public IReadOnlyList<Page> ModalStack => Array.Empty<Page>();

        public void InsertPageBefore(Page page, Page before) => throw new NotSupportedException();
        public Task<Page> PopAsync() => throw new NotSupportedException();
        public Task<Page> PopAsync(bool animated) => throw new NotSupportedException();
        public Task<Page> PopModalAsync() => throw new NotSupportedException();
        public Task<Page> PopModalAsync(bool animated) => throw new NotSupportedException();
        public Task PopToRootAsync() => throw new NotSupportedException();
        public Task PopToRootAsync(bool animated) => throw new NotSupportedException();
        public Task PushAsync(Page page) => throw new NotSupportedException();
        public Task PushAsync(Page page, bool animated) => throw new NotSupportedException();
        public Task PushModalAsync(Page page) => throw new NotSupportedException();
        public Task PushModalAsync(Page page, bool animated) => throw new NotSupportedException();
        public void RemovePage(Page page) => throw new NotSupportedException();
    }

    private sealed class StackNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public int GoBackCalls { get; private set; }
        public int GoToJournalCalls { get; private set; }

        public override Task GoBackAsync()
        {
            GoBackCalls++;
            return Task.CompletedTask;
        }

        public override Task GoToJournalAsync()
        {
            GoToJournalCalls++;
            return Task.CompletedTask;
        }
    }
}
