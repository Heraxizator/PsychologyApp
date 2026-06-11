using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class TestsListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IUserProgressService _userProgressService;
    private readonly ITestCatalogService _testCatalogService;

    public string PageTitle => AppStrings.TestsDetectorTitle;
    public string EmptyTitle => AppStrings.TestsEmptyTitle;
    public string EmptyBody => AppStrings.TestsEmptyBody;
    public string LoadingText => AppStrings.TestsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    public ObservableCollection<TestItem> TestItemCollection { get; private set; } = [];

    public TestsListViewModel(
        INavigation navigation,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _testCatalogService = testCatalogService;
        BindNavigation(navigation, navigationService);
        Reload = new AsyncCommand(InitAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(EmptyTitle),
            nameof(EmptyBody),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText));
        InitAsync().FireAndForget();
    }

    private async Task HandleSelectionAsync(TestItem testItem) =>
        await _navigationService.GoToFindProblemAsync(
            testItem.Description,
            testItem.Algorithm,
            testItem.Comment,
            testItem.Action,
            testItem.TestId);

    public async Task InitAsync()
    {
        try
        {
            await AppReadiness.DatabaseReadyAsync;

            SetInit();

            IReadOnlyList<TestDefinition> definitions = await _testCatalogService.GetCatalogAsync();
            List<TestItem> items = [];

            foreach (TestDefinition definition in definitions)
            {
                if (string.IsNullOrWhiteSpace(definition.TestId))
                {
                    continue;
                }

                TestItem item = TestItemFactory.Create(definition, _navigationService);

                TestResultDTO? latest = await _userProgressService.GetLatestTestResultAsync(item.TestId);
                if (latest is not null)
                {
                    item.LastResultSummary = AppStrings.TestLastResult(latest.Summary);
                }

                IReadOnlyList<TestResultDTO> history =
                    await _userProgressService.GetTestResultHistoryAsync(item.TestId, 2);
                item.HasMultipleResults = history.Count > 1;

                TestItem selected = item;
                item.TapCommand = new AsyncCommand(() => HandleSelectionAsync(selected));
                item.OpenHistoryCommand = new AsyncCommand(() =>
                    _navigationService.GoToTestHistoryAsync(selected.TestId, selected.Title));

                items.Add(item);
            }

            TestItemCollection = new ObservableCollection<TestItem>(items);
            OnPropertyChanged(nameof(TestItemCollection));

            SetDone();
        }
        catch (Exception)
        {
            SetFail();
        }
    }
}
