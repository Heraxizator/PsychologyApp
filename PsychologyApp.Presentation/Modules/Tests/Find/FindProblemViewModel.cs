using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class FindProblemViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ObservableCollection<AlgorithmRow> AlgorithmRows { get; private set; } = [];

    public string PageTitle => AppStrings.TestsAboutPassageTitle;
    public string DescriptionHeader => AppStrings.TestsDescriptionHeader;
    public string AlgorithmHeader => AppStrings.TestsAlgorithmHeader;
    public string NoteHeader => AppStrings.TestsNoteHeader;
    public string StartButtonText => AppStrings.TestsStartButton;

    private readonly Action _nextPageTappedAction = default!;

    public FindProblemViewModel() { }

    public FindProblemViewModel(
        INavigation navigation,
        INavigationService navigationService,
        string? description,
        List<string> algorithm,
        string? comment,
        Action action)
    {
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsAboutPassageTitle;

        BindNavigation(navigation, navigationService);

        DescriptionText = description;
        InitAlgorithmRows(algorithm);
        CommentText = comment;

        _nextPageTappedAction = action;

        Continue = new AsyncCommand(() =>
        {
            ToContinue();
            return Task.CompletedTask;
        });
        BackCommand = new AsyncCommand(GoToRootAsync);
        UserPreferences.Changed += OnPreferencesChanged;
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(DescriptionHeader));
        OnPropertyChanged(nameof(AlgorithmHeader));
        OnPropertyChanged(nameof(NoteHeader));
        OnPropertyChanged(nameof(StartButtonText));
    }

    private void InitAlgorithmRows(List<string> algorithmRows)
    {
        foreach (string algorithmRow in algorithmRows)
        {
            AlgorithmRows.Add(new AlgorithmRow { Text = algorithmRow });
        }
    }

    public void ToContinue() => _nextPageTappedAction.Invoke();

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
