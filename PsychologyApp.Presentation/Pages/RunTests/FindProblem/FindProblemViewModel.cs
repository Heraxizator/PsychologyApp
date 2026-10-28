using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.FindProblem;

public partial class FindProblemViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ObservableCollection<AlgorithmRow> AlgorithmRows { get; private set; } = [];

    private readonly INavigationService _navigationService = null!;
    private readonly ITestCatalogService _testCatalogService = null!;
    private readonly FindProblemContentLoader _contentLoader = null!;
    private readonly TestRunCoordinator _testRunCoordinator = null!;
    private readonly string? _testId;
    private readonly Func<Task> _startTestAsync = () => Task.CompletedTask;
    private readonly Func<Task<IReadOnlyList<TestHistoryEntryItem>>>? _loadHistoryAsync;
    private IReadOnlyList<TestHistoryEntryItem> _pastResults = [];
    public FindProblemViewModel() { }

    public FindProblemViewModel(
        INavigationService navigationService,
        ITestCatalogService testCatalogService,
        FindProblemContentLoader contentLoader,
        TestRunCoordinator testRunCoordinator,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null,
        Func<Task<IReadOnlyList<TestHistoryEntryItem>>>? loadHistory = null)
    {
        _navigationService = navigationService;
        _testCatalogService = testCatalogService;
        _contentLoader = contentLoader;
        _testRunCoordinator = testRunCoordinator;
        _testId = testId;
        _loadHistoryAsync = loadHistory;
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsAboutPassageTitle;

        BindNavigation(navigationService);

        DescriptionText = description;
        InitAlgorithmRows(algorithm);
        CommentText = comment;

        _startTestAsync = startTest;

        Continue = new AsyncCommand(ContinueAsync);
        BackCommand = new AsyncCommand(GoToRootAsync);
        OpenHistoryCommand = new AsyncCommand(() => _navigationService.GoToTestHistoryAsync(_testId ?? string.Empty, PageName ?? string.Empty));
        OnConstructed();
        LoadPastResultsAsync().FireAndForget();
    }

    public ICommand OpenHistoryCommand { get; private set; } = default!;

    /// <summary>
    /// The last three results of this test on the intro, newest first: before taking it again the person sees whether it changed, and
    /// the history page is one tap away instead of being the only place to look.
    /// </summary>
    public IReadOnlyList<TestHistoryEntryItem> PastResults
    {
        get => _pastResults;
        private set
        {
            if (SetProperty(ref _pastResults, value))
            {
                OnPropertyChanged(nameof(HasPastResults));
            }
        }
    }

    public bool HasPastResults => PastResults.Count > 0;
    public string PastResultsTitle => AppStrings.TestsPastResults;
    public string OpenHistoryText => AppStrings.TestOpenHistory;

    private async Task LoadPastResultsAsync()
    {
        if (_loadHistoryAsync is null)
        {
            return;
        }

        try
        {
            IReadOnlyList<TestHistoryEntryItem> entries = await _loadHistoryAsync();
            await UiThread.RunAsync(() => PastResults = [.. entries.Take(3)]);
        }
        catch (Exception loadError)
        {
            // The intro works without them; the history page reports its own failure.
            BestEffort.Report(loadError);
        }
    }

    partial void OnConstructed();

    private async Task ContinueAsync()
    {
        if (!string.IsNullOrWhiteSpace(_testId))
        {
            TestDefinition? definition = await _testCatalogService.GetByIdAsync(_testId);
            if (definition is not null)
            {
                await _testRunCoordinator.StartAsync(definition, _navigationService);
                return;
            }
        }

        await _startTestAsync();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(DescriptionHeader),
            nameof(AlgorithmHeader),
            nameof(NoteHeader),
            nameof(StartButtonText),
            nameof(PastResultsTitle),
            nameof(OpenHistoryText));

        ReloadLocalizedTestContent();
    }

    partial void ReloadLocalizedTestContent();
}
