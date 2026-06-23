using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.FindProblem;

public partial class FindProblemViewModel
{
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
