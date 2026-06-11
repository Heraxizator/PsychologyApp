using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Testing.Integration;

public sealed class FakeReasonContentProvider : IReasonContentProvider
{
    private readonly IReadOnlyList<Reason> _reasons;

    public FakeReasonContentProvider(params Reason[] reasons)
    {
        _reasons = reasons.Length > 0
            ? reasons
            : [Reason.Create("Head pain", "subtitle", "solution")];
    }

    public int LoadCallCount { get; private set; }

    public int GetPageCallCount { get; private set; }

    public Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default)
    {
        LoadCallCount++;
        return Task.FromResult<IEnumerable<Reason>>(_reasons);
    }

    public Task<IReadOnlyList<Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        GetPageCallCount++;
        return Task.FromResult<IReadOnlyList<Reason>>([]);
    }
}
