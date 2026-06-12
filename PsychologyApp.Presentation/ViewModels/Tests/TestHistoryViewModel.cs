using MvvmHelpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public sealed class TestHistoryEntryItem
{
    public string DateText { get; init; } = string.Empty;
    public string SummaryText { get; init; } = string.Empty;
}

public sealed class TestHistoryViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly ITestCatalogService _testCatalogService;
    private readonly INavigationService _navigationService;
    private readonly string _testId;
    private string _testTitle;

    public ObservableRangeCollection<TestHistoryEntryItem> HistoryEntries { get; } = [];
    public string PageTitle => $"{AppStrings.TestHistoryTitle}: {_testTitle}";
    public string EmptyText => AppStrings.TestHistoryEmpty;
    public string LoadingText => AppStrings.TestsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string RetakeButtonText => AppStrings.TestRetakeButton;
    public bool HasEntries => HistoryEntries.Count > 0;

    public ICommand RetakeCommand { get; }

    public TestHistoryViewModel(
        INavigation navigation,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService,
        string testId,
        string testTitle)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _testCatalogService = testCatalogService;
        _testId = testId;
        _testTitle = testTitle;
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestHistoryTitle;

        BindNavigation(navigation, navigationService);
        Reload = new AsyncCommand(LoadAsync);
        RetakeCommand = new AsyncCommand(RetakeAsync);
        LoadAsync().FireAndForget();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(EmptyText), nameof(LoadingText), nameof(FailedText), nameof(RetryText), nameof(RetakeButtonText));
        RefreshTitleAsync().FireAndForget();
    }

    private async Task RefreshTitleAsync()
    {
        PsychologyApp.Presentation.Models.Tests.TestDefinition? definition =
            await _testCatalogService.GetByIdAsync(_testId);
        if (definition is null)
        {
            return;
        }

        _testTitle = definition.Title;
        OnPropertyChanged(nameof(PageTitle));
    }

    private async Task LoadAsync()
    {
        try
        {
            SetInit();
            await AppReadiness.DatabaseReadyAsync;
            await RefreshTitleAsync();

            IReadOnlyList<TestResultDTO> history =
                await _userProgressService.GetTestResultHistoryAsync(_testId, 50);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                HistoryEntries.Clear();
                foreach (TestResultDTO item in history)
                {
                    string date = item.CompletedAt.ToLocalTime().ToString("g");
                    HistoryEntries.Add(new TestHistoryEntryItem
                    {
                        DateText = date,
                        SummaryText = item.Summary
                    });
                }

                OnPropertyChanged(nameof(HasEntries));
                SetDone();
            });
        }
        catch
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
        }
    }

    private async Task RetakeAsync()
    {
        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_testId);
        if (definition is null)
        {
            return;
        }

        TestItem item = TestItemFactory.Create(definition, _navigationService);
        await _navigationService.GoToRootAsync();
        await item.StartAsync();
    }
}
