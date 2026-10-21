namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Replaces the database with an empty one. <paramref name="whileExclusive"/> runs after the old files are gone and before any
    /// queued read or write may touch the new ones: "delete all my data" restarts the app there, so a save that was already on
    /// its way cannot land in the fresh database.
    /// </summary>
    Task RecreateDatabaseAsync(Func<Task>? whileExclusive = null, CancellationToken cancellationToken = default);
    Task ApplyMigrationsForAppVersionAsync(string appVersion, CancellationToken cancellationToken = default);
}
