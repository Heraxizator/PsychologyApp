using Microsoft.Extensions.Logging.Abstractions;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Testing.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Catalog;

public sealed class CachedTestCatalogServiceTests
{
    public CachedTestCatalogServiceTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task GetCatalogAsync_LoadsInnerServiceOnlyOnceUntilInvalidated()
    {
        var reader = new FakeTestAssetReader()
            .Register("tests/beck.json", MinimalGroupedJson("beck"))
            .Register("tests/luscher.json", "[]")
            .Register("tests/questionnaires.json", "[]");

        var inner = new TestCatalogService(reader, NullLogger<TestCatalogService>.Instance);
        var cache = new CachedTestCatalogService(inner, NullLogger<CachedTestCatalogService>.Instance);

        IReadOnlyList<TestDefinition> first = await cache.GetCatalogAsync();
        IReadOnlyList<TestDefinition> second = await cache.GetCatalogAsync();

        Assert.Single(first);
        Assert.Same(first, second);

        cache.Invalidate();

        IReadOnlyList<TestDefinition> third = await cache.GetCatalogAsync();
        Assert.Single(third);
        Assert.NotSame(first, third);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMatchingDefinition()
    {
        var reader = new FakeTestAssetReader()
            .Register("tests/beck.json", MinimalGroupedJson("beck"))
            .Register("tests/luscher.json", """
                [{"title":"Brief","subtitle":"S","description":"D","algorithm":["1"],"comment":"C","navigationTarget":"brief"}]
                """)
            .Register("tests/questionnaires.json", "[]");

        var cache = new CachedTestCatalogService(
            new TestCatalogService(reader, NullLogger<TestCatalogService>.Instance),
            NullLogger<CachedTestCatalogService>.Instance);

        TestDefinition? definition = await cache.GetByIdAsync(TestIds.LuscherBrief);

        Assert.NotNull(definition);
        Assert.Equal("Brief", definition!.Title);
    }

    private static string MinimalGroupedJson(string analyzerId) => $$"""
        {
          "title": "Test",
          "subtitle": "Sub",
          "description": "Desc",
          "algorithm": ["Step"],
          "comment": "Note",
          "analyzerId": "{{analyzerId}}",
          "singleAnswer": true,
          "questions": [
            { "answers": [ { "ball": 1, "text": "A" }, { "ball": 0, "text": "B" } ] }
          ]
        }
        """;
}
