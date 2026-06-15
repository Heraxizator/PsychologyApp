using Microsoft.Extensions.Logging;
using MvvmHelpers;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestHistoryViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly ITestCatalogService _testCatalogService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly TestHistoryLoader _historyLoader;
    private readonly TestRetakeOperations _retakeOperations;
    private readonly ILogger<TestHistoryViewModel> _logger;
    private readonly string _testId;
    private string _testTitle;

    public ObservableRangeCollection<TestHistoryEntryItem> HistoryEntries { get; } = [];
    public bool HasEntries => HistoryEntries.Count > 0;

    public ICommand RetakeCommand { get; }

    public TestHistoryViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService,
        IDatabaseReadySignal databaseReadySignal,
        TestHistoryLoader historyLoader,
        TestRetakeOperations retakeOperations,
        ILogger<TestHistoryViewModel> logger,
        string testId,
        string testTitle)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _testCatalogService = testCatalogService;
        _databaseReadySignal = databaseReadySignal;
        _historyLoader = historyLoader;
        _retakeOperations = retakeOperations;
        _logger = logger;
        _testId = testId;
        _testTitle = testTitle;
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestHistoryTitle;

        BindNavigation(navigationService);
        Reload = new AsyncCommand(LoadAsync);
        RetakeCommand = new AsyncCommand(RetakeAsync);
        LoadAsync().FireAndForget();
    }

    private Task RetakeAsync() =>
        _retakeOperations.RetakeAsync(_testId, _testCatalogService, _navigationService);
}
