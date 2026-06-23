using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.App;

public interface IShellStartupCoordinator
{
    Task InitializeAsync();

    Task ShowOnboardingIfNeededAsync(
        INavigation navigation,
        Func<TechniqueId?, Task> onTechniqueSelected);

    Task ShowOnboardingAsync(
        INavigation navigation,
        Func<TechniqueId?, Task> onTechniqueSelected);
}
