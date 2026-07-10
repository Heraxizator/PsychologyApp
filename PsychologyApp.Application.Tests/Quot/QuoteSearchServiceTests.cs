using PsychologyApp.Application.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public sealed class QuoteSearchServiceTests
{
    [Fact]
    public async Task SearchCatalogAsync_FindsQuotesByAuthorOrText()
    {
        var provider = new FakeQuotContentProvider();
        var service = new QuoteSearchService(provider);

        IReadOnlyList<Application.Abstractions.Integration.QuotSeed> results =
            await service.SearchCatalogAsync("seneca");

        Assert.Single(results);
        Assert.Contains("Seneca", results[0].Author, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeQuotContentProvider : Application.Abstractions.Integration.IQuotContentProvider
    {
        public Task<IReadOnlyList<Application.Abstractions.Integration.QuotSeed>> LoadAllAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Application.Abstractions.Integration.QuotSeed>>(
            [
                new Application.Abstractions.Integration.QuotSeed("Seneca", "Be calm", "calm"),
                new Application.Abstractions.Integration.QuotSeed("Confucius", "Step forward", "motivation")
            ]);
    }
}

public sealed class QuotePersonalizationPolicyTests
{
    [Fact]
    public void ResolveDailyQuoteIndex_IsDeterministicForSameDate()
    {
        DateOnly date = new(2026, 7, 4);
        int first = QuotePersonalizationPolicy.ResolveDailyQuoteIndex(date, 525);
        int second = QuotePersonalizationPolicy.ResolveDailyQuoteIndex(date, 525);
        Assert.Equal(first, second);
    }

    [Fact]
    public void ResolveThemes_ForAnxiety_IncludesCalmAndAnxiety()
    {
        IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes("anxiety");
        Assert.Contains("anxiety", themes);
        Assert.Contains("calm", themes);
    }

    [Fact]
    public void ResolveThemes_LowMood_PrefersCalmThemes()
    {
        IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes("anxiety", todayMoodLevel: 1);
        Assert.Contains("calm", themes);
        Assert.Contains("acceptance", themes);
        Assert.DoesNotContain("anxiety", themes);
    }

    [Fact]
    public void ResolveThemes_HighMood_PrefersUpliftingThemes()
    {
        IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes("anxiety", todayMoodLevel: 5);
        Assert.Contains("motivation", themes);
        Assert.Contains("happiness", themes);
        Assert.DoesNotContain("anxiety", themes);
    }

    [Fact]
    public void ResolveThemes_NeutralMood_KeepsConcernThemes()
    {
        IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes("anxiety", todayMoodLevel: 3);
        Assert.Contains("anxiety", themes);
        Assert.Contains("calm", themes);
    }
}
