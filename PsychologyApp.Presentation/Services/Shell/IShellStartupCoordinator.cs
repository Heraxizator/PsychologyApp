using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.Services.Shell;

public interface IShellStartupCoordinator
{
    Task InitializeAsync();

    Task ShowOnboardingIfNeededAsync(
        INavigation navigation,
        Func<TechniqueId?, Task> onTechniqueSelected);
}
