using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PsychologyApp.Application.Articles;

namespace PsychologyApp.Presentation.Widgets.ArticleBlocks;

/// <summary>A statement of a "do I recognise this?" list, or an answer of a question; both are tapped.</summary>
public sealed class ArticleChoiceItem : INotifyPropertyChanged
{
    private bool _isChecked;
    private bool _isRight;
    private bool _isWrong;

    public required string Text { get; init; }
    public required int Index { get; init; }

    /// <summary>An answer of a quiz rather than a statement to tick.</summary>
    public bool IsQuizOption { get; init; }
    public ICommand? TapCommand { get; set; }

    public bool IsChecked
    {
        get => _isChecked;
        set => Set(ref _isChecked, value);
    }

    /// <summary>After a quiz answer: this option is the right one.</summary>
    public bool IsRight
    {
        get => _isRight;
        set => Set(ref _isRight, value);
    }

    /// <summary>After a quiz answer: this option was chosen and is not right.</summary>
    public bool IsWrong
    {
        get => _isWrong;
        set => Set(ref _isWrong, value);
    }

    public string Icon => IsQuizOption
        ? IsRight ? "CheckCircle" : IsWrong ? "Cancel" : "RadioButtonUnchecked"
        : IsChecked ? "CheckBox" : "CheckBoxOutlineBlank";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set(ref bool field, bool value, [CallerMemberName] string? name = null)
    {
        if (field != value)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
        }
    }
}

/// <summary>One block of an article or a course day on screen; the template shows the part that matches its kind.</summary>
public sealed class ArticleBlockItem : INotifyPropertyChanged
{
    private string _feedback = string.Empty;

    public required ArticleBlockKind Kind { get; init; }
    public string Text { get; init; } = string.Empty;
    public string BulletsText { get; init; } = string.Empty;
    public string LinkCaption { get; init; } = string.Empty;
    public string LinkIcon { get; init; } = string.Empty;
    public ICommand? LinkCommand { get; init; }

    /// <summary>Statements of a checklist, answers of a quiz, steps of a cycle.</summary>
    public IReadOnlyList<ArticleChoiceItem> Choices { get; init; } = [];

    /// <summary>The verdict of a checklist or the explanation after a quiz answer; empty until something is tapped.</summary>
    public string Feedback
    {
        get => _feedback;
        set
        {
            if (_feedback != value)
            {
                _feedback = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Feedback)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasFeedback)));
            }
        }
    }

    public bool HasFeedback => Feedback.Length > 0;

    /// <summary>What a screen reader says for a drawn loop: the steps in order and that it starts again.</summary>
    public string CycleDescription { get; init; } = string.Empty;

    public bool IsLink => Kind is ArticleBlockKind.Practice or ArticleBlockKind.FearLadder or ArticleBlockKind.CrisisHelp
        or ArticleBlockKind.Tests or ArticleBlockKind.Article or ArticleBlockKind.TraumaChat;

    public event PropertyChangedEventHandler? PropertyChanged;
}
