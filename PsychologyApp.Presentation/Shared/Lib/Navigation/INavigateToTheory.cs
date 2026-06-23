namespace PsychologyApp.Presentation.Shared.Lib.Navigation;

/// <summary>
/// Port for navigating to technique theory content without referencing page types from shared layer consumers.
/// </summary>
public interface INavigateToTheory
{
    Task NavigateToTheoryAsync(string content, string? techniqueId = null);
}
