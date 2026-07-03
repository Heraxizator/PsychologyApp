namespace PsychologyApp.Presentation.Shared.Navigation;

/// <summary>
/// Main-thread dispatcher for navigation. MAUI assigns <see cref="InvokeAsync"/> at startup.
/// </summary>
public static class NavigationThread
{
    public static Func<Func<Task>, Task> InvokeAsync { get; set; } = static action => action();
}
