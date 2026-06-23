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
    public FormattedString? HighlightedTitle { get; init; }
    public FormattedString? HighlightedSubtitle { get; init; }

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
            HighlightedTitle = SearchTextHighlighter.Build(dto.Title, searchText),
            HighlightedSubtitle = SearchTextHighlighter.Build(dto.Subtitle, searchText),
            SuggestedTechniques = suggestions
        };
}

public sealed class PhysicsTechniqueSuggestion
{
    public string Title { get; init; } = string.Empty;
    public ICommand? OpenCommand { get; init; }
}
