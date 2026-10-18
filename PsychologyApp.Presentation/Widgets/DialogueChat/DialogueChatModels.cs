using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.DialogueChat;

public sealed record DialogueMessage(string Text, bool IsUser)
{
    public LayoutOptions Alignment => IsUser ? LayoutOptions.End : LayoutOptions.Start;
}

/// <summary>Carries its own command so the chip template needs no ancestor binding.</summary>
public sealed record DialogueChoiceItem(int Index, string Label, ICommand Command);

public sealed record DialogueRatingItem(int Value, ICommand Command);

/// <summary>A typical answer under a text question: one tap adds it to the draft, another takes it out.</summary>
public sealed class DialogueSuggestionItem(string label, string value, ICommand command) : System.ComponentModel.INotifyPropertyChanged
{
    private bool _isSelected;

    public string Label { get; } = label;

    /// <summary>What goes into the draft; differs from <see cref="Label"/> for the shortened "same as last time" chip.</summary>
    public string Value { get; } = value;

    public ICommand Command { get; } = command;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
