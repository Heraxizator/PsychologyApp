using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public sealed class CachedQuotContentProviderTests
{
    [Fact]
    public async Task LoadAllAsync_LoadsInnerProviderOnlyOnce()
    {
        var inner = new CountingQuotProvider(
        [
            new QuotSeed("Title", "Text", "Theme")
        ]);
        var provider = new CachedQuotContentProvider(inner);

        await provider.LoadAllAsync();
        await provider.LoadAllAsync();

        Assert.Equal(1, inner.LoadCount);
    }

    [Fact]
    public async Task Invalidate_ForcesReload()
    {
        var inner = new CountingQuotProvider(
        [
            new QuotSeed("Title", "Text", "Theme")
        ]);
        var provider = new CachedQuotContentProvider(inner);

        await provider.LoadAllAsync();
        provider.Invalidate();
        await provider.LoadAllAsync();

        Assert.Equal(2, inner.LoadCount);
    }

    private sealed class CountingQuotProvider(IReadOnlyList<QuotSeed> seeds) : IQuotContentProvider
    {
        public int LoadCount { get; private set; }

        public Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default)
        {
            LoadCount++;
            return Task.FromResult(seeds);
        }
    }
}
