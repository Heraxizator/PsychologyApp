using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.LibraryCard;

/// <summary>An article or a course on a Library page.</summary>
public sealed class LibraryCardItem
{
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required string Icon { get; init; }
    public required string MetaText { get; init; }
    public required ICommand OpenCommand { get; init; }

    /// <summary>Why it is suggested ("For you"); empty elsewhere.</summary>
    public string Reason { get; init; } = string.Empty;
    public bool HasReason => Reason.Length > 0;
    public bool IsRead { get; init; }
    public bool IsSaved { get; init; }
}
