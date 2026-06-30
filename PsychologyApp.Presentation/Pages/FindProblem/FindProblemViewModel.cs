using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.FindProblem;

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
        string? testId = null)
    {
        _navigationService = navigationService;
        _testCatalogService = testCatalogService;
        _contentLoader = contentLoader;
        _testRunCoordinator = testRunCoordinator;
        _testId = testId;
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsAboutPassageTitle;

        BindNavigation(navigationService);

        DescriptionText = description;
        InitAlgorithmRows(algorithm);
        CommentText = comment;

        _startTestAsync = startTest;

        Continue = new AsyncCommand(ContinueAsync);
        BackCommand = new AsyncCommand(GoToRootAsync);
        OnConstructed();
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
            nameof(IntroLeadText));

        ReloadLocalizedTestContent();
    }

    partial void ReloadLocalizedTestContent();
}
