using System.Text.Json;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class EntryDraftTests
{
    [Fact]
    public void EntryDraft_RoundTrips_rating_value()
    {
        var draft = new EntryDraft
        {
            Fields = new Dictionary<string, string>
            {
                ["2"] = "8"
            }
        };

        string json = JsonSerializer.Serialize(draft);
        EntryDraft? restored = JsonSerializer.Deserialize<EntryDraft>(json);

        Assert.NotNull(restored);
        Assert.Equal("8", restored.Fields["2"]);
    }

    [Fact]
    public void EntryDraft_RoundTripsThroughJson()
    {
        var draft = new EntryDraft
        {
            Fields = new Dictionary<string, string>
            {
                ["0"] = "Episode text",
                ["1"] = "Feeling text"
            }
        };

        string json = JsonSerializer.Serialize(draft);
        EntryDraft? restored = JsonSerializer.Deserialize<EntryDraft>(json);

        Assert.NotNull(restored);
        Assert.Equal("Episode text", restored.Fields["0"]);
        Assert.Equal("Feeling text", restored.Fields["1"]);
    }
}
