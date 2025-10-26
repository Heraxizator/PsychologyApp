using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TestViewModels;

public class FindProblemViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;
    public ObservableCollection<AlgorithmRow> AlgorithmRows { get; private set; } = [];

    private readonly Action _nextPageTappedAction = default!;

    public FindProblemViewModel() { }

    public FindProblemViewModel(INavigation navigation, string? describtion, List<string> algorithm, string? comment, Action action)
    {
        ModuleName = "Детектор";
        PageName = "О детекторе";

        Navigation = navigation;

        DescribtionText = describtion;

        InitAlgorithmRows(algorithm);

        CommentText = comment;

        _nextPageTappedAction = action;

        Continue = new Command(ToContinue);
    }

    private void InitAlgorithmRows(List<string> algorithmRows)
    {
        foreach (string algorithmRow in algorithmRows)
        {
            AlgorithmRows.Add(new AlgorithmRow { Text = algorithmRow });
        }
    }

    public void ToContinue()
    {
        _nextPageTappedAction.Invoke();
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

public class AlgorithmRow
{
    public string Text { get; set; } = default!;
}
