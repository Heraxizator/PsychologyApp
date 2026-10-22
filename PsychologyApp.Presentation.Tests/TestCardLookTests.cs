using System.Text.Json;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>
/// The test list tells its cards apart by icon and colour. The look is keyed by test id, so a misspelt id or a test added
/// to the catalogue without a look would silently fall back to the shared accent icon.
/// </summary>
public sealed class TestCardLookTests
{
    private static IEnumerable<string> CatalogueTestIds()
    {
        foreach (string file in new[] { "beck.json", "pss10.json", "rses.json", "questionnaires.json" })
        {
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(QuestionnaireScoreRangeTests.RawRoot(), file)));
            JsonElement root = doc.RootElement;
            IEnumerable<JsonElement> tests = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : [root];
            foreach (JsonElement test in tests)
            {
                yield return test.GetProperty("analyzerId").GetString()!;
            }
        }

        yield return TestIds.LuscherBrief;
        yield return TestIds.LuscherStandard;
    }

    [Fact]
    public void Every_test_in_the_catalogue_has_its_own_icon_and_colour_group()
    {
        string[] withoutLook = [.. CatalogueTestIds().Where(id =>
            TestCardLook.For(id) is var look && (look.Icon == TestCardLook.DefaultIcon || look.Flavor == TechniqueFlavor.None))];

        Assert.Empty(withoutLook);
    }
}
