using System.Text.Json.Serialization;
using PsychologyApp.Presentation.Models.Quotes;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(List<QuoteJsonEntry>))]
[JsonSerializable(typeof(JsonGroupedQuestionnaireDefinition))]
[JsonSerializable(typeof(List<JsonNavigationTestDefinition>))]
[JsonSerializable(typeof(List<JsonSimpleQuestionnaireDefinition>))]
public partial class AppJsonSerializerContext : JsonSerializerContext;
