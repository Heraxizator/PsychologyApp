using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Presentation.Infrastructure;

public sealed class MauiReasonContentProvider : IReasonContentProvider
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<Reason>? _cache;

    public async Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    public async Task<IReadOnlyList<Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Reason> cache = await EnsureCacheAsync(cancellationToken);
        return cache
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private async Task<IReadOnlyList<Reason>> EnsureCacheAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            List<Reason> reasons = [];

            await using Stream fileStream = await FileSystem.OpenAppPackageFileAsync("Psyhosomatic.txt");
            using StreamReader reader = new(fileStream);

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                string[] cols = line.Split('\t');
                if (cols.Length < 3)
                {
                    continue;
                }

                string problemText = cols[0];
                string problemReason = cols[1];
                string problemSolution = cols[2];

                if (string.IsNullOrWhiteSpace(problemText) ||
                    string.IsNullOrWhiteSpace(problemReason) ||
                    string.IsNullOrWhiteSpace(problemSolution))
                {
                    continue;
                }

                reasons.Add(Reason.Create(problemText, problemReason, problemSolution));
            }

            _cache = reasons;
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
