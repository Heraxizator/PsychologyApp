using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface ITestResultProgressRepository
{
    Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetMostRecentTestResultAsync(TimeSpan within, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetAllTestResultsAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetLatestTestResultsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(string TestId, int Count)>> GetTestResultCountsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default);
    Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default);
}
