using System.Text.Json.Serialization;

namespace PsychologyApp.Application.Models.Tests;

public sealed class LuscherStandardColorDetail
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class LuscherStandardResultDetail
{
    [JsonPropertyName("co")]
    public int Co { get; set; }

    [JsonPropertyName("bk")]
    public double Bk { get; set; }

    [JsonPropertyName("colors")]
    public List<LuscherStandardColorDetail> Colors { get; set; } = [];
}

public sealed class LuscherBriefColorDetail
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public sealed class LuscherBriefResultDetail
{
    [JsonPropertyName("first")]
    public LuscherBriefColorDetail? First { get; set; }

    [JsonPropertyName("second")]
    public LuscherBriefColorDetail? Second { get; set; }
}
