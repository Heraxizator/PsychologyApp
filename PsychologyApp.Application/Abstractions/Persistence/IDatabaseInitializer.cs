namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task RecreateDatabaseAsync(CancellationToken cancellationToken = default);
    Task ApplyMigrationsForAppVersionAsync(string appVersion, CancellationToken cancellationToken = default);
}
