using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Serialization;
using System.Text.Json;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuestionnaireDetailReaderTests
{
    [Fact]
    public void TryParse_ReturnsDetail_WhenJsonIsValid()
    {
        QuestionnaireResultDetail detail = new(
            "beck",
            "beck",
            DateTime.UtcNow,
            42,
            "Depression",
            [new QuestionnaireResultQuestion(1, "Q1", [1], ["Yes"])]);

        string json = JsonSerializer.Serialize(detail, AppJsonSerializerContext.Default.QuestionnaireResultDetail);
        QuestionnaireDetailReader reader = new();

        QuestionnaireResultDetail? parsed = reader.TryParse(json);

        Assert.NotNull(parsed);
        Assert.Equal("beck", parsed!.TestId);
        Assert.Equal(42, parsed.DurationSeconds);
    }

    [Fact]
    public void TryParse_ReturnsNull_WhenJsonIsInvalid() =>
        Assert.Null(new QuestionnaireDetailReader().TryParse("{not-json"));
}
