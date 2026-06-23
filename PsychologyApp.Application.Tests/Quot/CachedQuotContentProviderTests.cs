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
    public void Invalidate_ForcesReload()
    {
        var inner = new CountingQuotProvider(
        [
            new QuotSeed("Title", "Text", "Theme")
        ]);
        var provider = new CachedQuotContentProvider(inner);

        provider.LoadAllAsync().GetAwaiter().GetResult();
        provider.Invalidate();
        provider.LoadAllAsync().GetAwaiter().GetResult();

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
