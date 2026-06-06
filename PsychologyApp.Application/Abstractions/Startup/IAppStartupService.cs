namespace PsychologyApp.Application.Abstractions.Startup;

public interface IAppStartupService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
