using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The numbered steps of a practice are split into a badge and the words.</summary>
public class AlgorithmStepTests
{
    [Theory]
    [InlineData("1. Оцените напряжение до практики", 1, "Оцените напряжение до практики")]
    [InlineData("5. Пауза на 4 счёта — повторите 4 цикла", 5, "Пауза на 4 счёта — повторите 4 цикла")]
    [InlineData("12) Twelfth step", 12, "Twelfth step")]
    [InlineData("  3.   Padded  ", 3, "Padded")]
    public void ANumberedStepGivesItsNumberAndWords(string raw, int number, string text)
    {
        AlgorithmStep step = AlgorithmStep.Parse(raw);

        Assert.Equal((number, text), (step.Number, step.Text));
    }

    [Theory]
    [InlineData("Просто текст без номера")]
    [InlineData("2024 год был трудным")]
    [InlineData("3.5 литра воды")]
    [InlineData("")]
    public void AnythingElseStaysAsItIs(string raw)
    {
        AlgorithmStep step = AlgorithmStep.Parse(raw);

        Assert.Null(step.Number);
        Assert.Equal(raw.Trim(), step.Text);
    }

    [Fact]
    public void NothingGivesAnEmptyStepNotAnError()
    {
        Assert.Equal(new AlgorithmStep(null, string.Empty), AlgorithmStep.Parse(null));
    }
}
