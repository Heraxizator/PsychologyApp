using System.Text.Json.Serialization;
using PsychologyApp.Application.Models.Quotes;

namespace PsychologyApp.Presentation.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<QuoteJsonEntry>))]
public partial class AppJsonSerializerContext : JsonSerializerContext;
