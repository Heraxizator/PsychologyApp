using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Reason;
using DomainReason = PsychologyApp.Domain.Entities.Reason;
using Xunit;

namespace PsychologyApp.Application.Tests.Reason;

public class CachedReasonContentProviderTests
{
    [Fact]
    public async Task GetPageAsync_ReturnsRequestedSlice()
    {
        var inner = new CountingReasonProvider(
            Enumerable.Range(1, 5)
                .Select(i => DomainReason.Create($"Title{i}", $"Sub{i}", $"Sol{i}"))
                .ToList());
        var provider = new CachedReasonContentProvider(inner);

        IReadOnlyList<DomainReason> page = await provider.GetPageAsync(1, 2);

        Assert.Equal(2, page.Count);
        Assert.Equal("Title3", page[0].Title);
        Assert.Equal(1, inner.LoadCount);
    }

    [Fact]
    public async Task LoadReasonsAsync_LoadsInnerProviderOnlyOnce()
    {
        var inner = new CountingReasonProvider(
        [
            DomainReason.Create("Title", "Subtitle", "Solution")
        ]);
        var provider = new CachedReasonContentProvider(inner);

        await provider.LoadReasonsAsync();
        await provider.LoadReasonsAsync();

        Assert.Equal(1, inner.LoadCount);
    }

    private sealed class CountingReasonProvider(IReadOnlyList<DomainReason> reasons) : IReasonContentProvider
    {
        public int LoadCount { get; private set; }

        public Task<IEnumerable<DomainReason>> LoadReasonsAsync(CancellationToken cancellationToken = default)
        {
            LoadCount++;
            return Task.FromResult<IEnumerable<DomainReason>>(reasons);
        }

        public Task<IReadOnlyList<DomainReason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<DomainReason>>(reasons.Skip(page * pageSize).Take(pageSize).ToList());
    }
}
