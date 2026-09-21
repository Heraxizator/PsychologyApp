using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.DialogueChat;

public sealed record DialogueMessage(string Text, bool IsUser)
{
    public LayoutOptions Alignment => IsUser ? LayoutOptions.End : LayoutOptions.Start;
}

/// <summary>Carries its own command so the chip template needs no ancestor binding.</summary>
public sealed record DialogueChoiceItem(int Index, string Label, ICommand Command);

public sealed record DialogueRatingItem(int Value, ICommand Command);
