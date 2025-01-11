using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.API.Quots;

public class QuotAPI
{
    [JsonPropertyName("quoteText")]
    public string? QuoteText { get; set; }

    [JsonPropertyName("quoteAuthor")]
    public string? QuoteAuthor { get; set; }

    [JsonPropertyName("senderName")]
    public string? SenderName { get; set; }

    [JsonPropertyName("senderLink")]
    public string? SenderLink { get; set; }

    [JsonPropertyName("quoteLink")]
    public string? QuoteLink { get; set; }
}
