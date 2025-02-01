using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TestViewModels;

public class FindProblemViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;
    public new ICommand Finish { get; private set; } = default!;


    private readonly Action _nextPageTappedAction = default!;

    public FindProblemViewModel() { }

    public FindProblemViewModel(INavigation navigation, string? describtion, List<string> algorithm, string? comment, Action action)
    {
        ModuleName = "Детектор";
        PageName = "О детекторе";

        Navigation = navigation;

        DescribtionText = describtion;

        AlgorithmRows = [];

        foreach (string row in algorithm)
        {
            AlgorithmRows.Add
            (
                new AlgorithmItem { Row = row }
            );
        }

        CommentText = comment;

        _nextPageTappedAction = action;

        Continue = new Command(ToContinue);

        Finish = new Command(async () => await Navigation.PopAsync(false));
    }

    private void ToContinue(object obj)
    {
        _ = _nextPageTappedAction.DynamicInvoke();
    }

    #region Public Properties


    private string? _describtion_text;
    public string? DescribtionText
    {
        get => _describtion_text;
        set
        {
            if (_describtion_text != value)
            {
                _describtion_text = value;
                OnPropertyChanged(nameof(DescribtionText));
            }
        }
    }

    public ObservableCollection<AlgorithmItem> AlgorithmRows { get; private set; } = [];

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

    #endregion
}

public class AlgorithmItem
{
    public string? Row { get; set; }
}
