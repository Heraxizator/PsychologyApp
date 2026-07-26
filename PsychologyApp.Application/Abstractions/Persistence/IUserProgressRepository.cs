namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>
/// Composite progress port kept for existing consumers.
/// Prefer the narrower mood / test-result / practice interfaces for new code.
/// </summary>
public interface IUserProgressRepository :
    IMoodProgressRepository,
    ITestResultProgressRepository,
    IPracticeProgressRepository
{
}
