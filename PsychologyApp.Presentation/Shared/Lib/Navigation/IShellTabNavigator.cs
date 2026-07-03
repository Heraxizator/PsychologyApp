namespace PsychologyApp.Presentation.Shared.Lib.Navigation;

/// <summary>
/// Port for shell tab navigation from Shared without referencing the app shell implementation.
/// </summary>
public interface IShellTabNavigator
{
    void OpenPracticeTabAndPendingTechnique();
}
