using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class FindProblemViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ObservableCollection<AlgorithmRow> AlgorithmRows { get; private set; } = [];

    public string PageTitle => AppStrings.TestsAboutPassageTitle;
    public string DescriptionHeader => AppStrings.TestsDescriptionHeader;
    public string AlgorithmHeader => AppStrings.TestsAlgorithmHeader;
    public string NoteHeader => AppStrings.TestsNoteHeader;
    public string StartButtonText => AppStrings.TestsStartButton;

    private readonly INavigationService _navigationService = null!;
    private readonly ITestCatalogService _testCatalogService = null!;
    private readonly string? _testId;
    private readonly Func<Task> _startTestAsync = () => Task.CompletedTask;

    public FindProblemViewModel() { }

    public FindProblemViewModel(
        INavigation navigation,
        INavigationService navigationService,
        ITestCatalogService testCatalogService,
        string? description,
        List<string> algorithm,
        string? comment,
        Func<Task> startTest,
        string? testId = null)
    {
        _navigationService = navigationService;
        _testCatalogService = testCatalogService;
        _testId = testId;
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsAboutPassageTitle;

        BindNavigation(navigation, navigationService);

        DescriptionText = description;
        InitAlgorithmRows(algorithm);
        CommentText = comment;

        _startTestAsync = startTest;

        Continue = new AsyncCommand(_startTestAsync);
        BackCommand = new AsyncCommand(GoToRootAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(DescriptionHeader),
            nameof(AlgorithmHeader),
            nameof(NoteHeader),
            nameof(StartButtonText));

        ReloadLocalizedTestContent();
    }

    partial void ReloadLocalizedTestContent();

    private void InitAlgorithmRows(List<string> algorithmRows)
    {
        foreach (string algorithmRow in algorithmRows)
        {
            AlgorithmRows.Add(new AlgorithmRow { Text = algorithmRow });
        }
    }

    private string? _descriptionText;
    public string? DescriptionText
    {
        get => _descriptionText;
        set
        {
            if (_descriptionText != value)
            {
                _descriptionText = value;
                OnPropertyChanged(nameof(DescriptionText));
            }
        }
    }

    private string? _comment_text;
    public string? CommentText
    {
        get => _comment_text;
        set
        {
            if (_comment_text != value)
            {
                _comment_text = value;
                OnPropertyChanged(nameof(CommentText));
            }
        }
    }
}

public class AlgorithmRow
{
    public string Text { get; set; } = default!;
}
