using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.RunTests.FindProblem;

public partial class FindProblemViewModel
{
    private void InitAlgorithmRows(List<string> algorithmRows)
    {
        for (int i = 0; i < algorithmRows.Count; i++)
        {
            AlgorithmRows.Add(AlgorithmRow.From(algorithmRows[i], i));
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
