using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Testing.Integration;
using Xunit;

namespace PsychologyApp.Application.Tests.ReasonSearch;

public sealed class ReasonSearchServiceTests
{
    [Fact]
    public void Search_TitleMatchRanksHigherThanSolution()
    {
        var service = new ReasonSearchService(new FakeReasonContentProvider());
        var reasons = new List<ReasonDTO>
        {
            new() { Title = "Other", Subtitle = "s", Solution = "headache relief" },
            new() { Title = "Head pain", Subtitle = "s", Solution = "rest" }
        };

        IReadOnlyList<RankedReason> results = service.Search(reasons, "head");

        Assert.Equal(2, results.Count);
        Assert.Equal("Head pain", results[0].Reason.Title);
        Assert.True(results[0].MatchScore > results[1].MatchScore);
    }

    [Fact]
    public void Search_EmptyQuery_ReturnsEmpty()
    {
        var service = new ReasonSearchService(new FakeReasonContentProvider());
        var reasons = new List<ReasonDTO> { new() { Title = "A", Subtitle = "b", Solution = "c" } };

        IReadOnlyList<RankedReason> results = service.Search(reasons, "   ");

        Assert.Empty(results);
    }

    [Fact]
    public async Task LoadReasonsAsync_UsesContentProviderLoad_NotPagedReasonService()
    {
        var provider = new FakeReasonContentProvider();
        var service = new ReasonSearchService(provider);

        IReadOnlyList<ReasonDTO> loaded = await service.LoadReasonsAsync();

        Assert.Single(loaded);
        Assert.Equal("Head pain", loaded[0].Title);
        Assert.Equal(1, provider.LoadCallCount);
        Assert.Equal(0, provider.GetPageCallCount);
    }

    [Fact]
    public void Search_SubtitleMatchRanksHigherThanSolution()
    {
        var service = new ReasonSearchService(new FakeReasonContentProvider());
        var reasons = new List<ReasonDTO>
        {
            new() { Title = "Other", Subtitle = "s", Solution = "neck pain" },
            new() { Title = "Other", Subtitle = "neck tension", Solution = "rest" }
        };

        IReadOnlyList<RankedReason> results = service.Search(reasons, "neck");

        Assert.Equal(2, results.Count);
        Assert.Equal("neck tension", results[0].Reason.Subtitle);
        Assert.Equal(2, results[0].MatchScore);
        Assert.Equal(1, results[1].MatchScore);
    }

}
