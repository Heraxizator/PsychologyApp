using System.Text.Json.Serialization;
using PsychologyApp.Application.Models.Tests;

namespace PsychologyApp.Application.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(JsonGroupedQuestionnaireDefinition))]
[JsonSerializable(typeof(List<JsonNavigationTestDefinition>))]
[JsonSerializable(typeof(List<JsonSimpleQuestionnaireDefinition>))]
[JsonSerializable(typeof(QuestionnaireResultDetail))]
public partial class TestJsonSerializerContext : JsonSerializerContext;
