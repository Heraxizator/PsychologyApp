using MvvmHelpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public sealed class TestHistoryEntryItem
{
    public string DateText { get; init; } = string.Empty;
    public string SummaryText { get; init; } = string.Empty;
}

public sealed class TestHistoryViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly string _testId;

    public ObservableRangeCollection<TestHistoryEntryItem> HistoryEntries { get; } = [];
    public string PageTitle { get; }
    public string EmptyText => AppStrings.TestHistoryEmpty;
    public string LoadingText => AppStrings.TestsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public bool HasEntries => HistoryEntries.Count > 0;

    public TestHistoryViewModel(
        INavigation navigation,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        string testId,
        string testTitle)
    {
        _userProgressService = userProgressService;
        _testId = testId;
        PageTitle = $"{AppStrings.TestHistoryTitle}: {testTitle}";
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestHistoryTitle;

        BindNavigation(navigation, navigationService);
        Reload = new AsyncCommand(LoadAsync);
        LoadAsync().FireAndForget();
    }

    private async Task LoadAsync()
    {
        try
        {
            SetInit();
            await AppReadiness.DatabaseReadyAsync;

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
}
