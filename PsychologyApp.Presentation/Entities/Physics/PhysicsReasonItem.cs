using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Entities.Physics;

public sealed class PhysicsReasonItem : INotifyPropertyChanged
{
    public long ReasonId { get; init; }
    public string? Title { get; init; }
    public string? Subtitle { get; init; }
    public string? Solution { get; init; }
    // Built on first display, on the UI thread: a search can match hundreds of reasons, and a FormattedString of
    // Spans per match was built up front although only the rows scrolled into view are ever shown.
    private string _searchText = string.Empty;
    private FormattedString? _highlightedTitle;
    private FormattedString? _highlightedSubtitle;

    public FormattedString? HighlightedTitle => _highlightedTitle ??= SearchTextHighlighter.Build(Title, _searchText);
    public FormattedString? HighlightedSubtitle => _highlightedSubtitle ??= SearchTextHighlighter.Build(Subtitle, _searchText);

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
            {
                return;
            }

            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public ICommand? ToggleExpandCommand { get; set; }
    public IReadOnlyList<PhysicsTechniqueSuggestion> SuggestedTechniques { get; init; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public static PhysicsReasonItem FromDto(
        ReasonDTO dto,
        IReadOnlyList<PhysicsTechniqueSuggestion> suggestions,
        string searchText) =>
        new()
        {
            ReasonId = dto.ReasonId,
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Solution = dto.Solution,
            _searchText = searchText,
            SuggestedTechniques = suggestions
        };
}

public sealed class PhysicsTechniqueSuggestion
{
    public string Title { get; init; } = string.Empty;
    public ICommand? OpenCommand { get; init; }
}
