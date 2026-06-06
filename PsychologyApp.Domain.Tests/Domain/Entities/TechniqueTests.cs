using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Tests.Domain.Entities;

public class TechniqueTests
{
    private static Technique CreateValid() =>
        Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BuildShouldThrowExceptionIfNumberIsNotSet(string number)
    {
        var action = () => Technique.Create(-1, number, "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfAlgorithmIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", string.Empty, "Any Image");
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldCreateIfEveryPropertyIsSet()
    {
        Technique technique = CreateValid();

        Assert.Equal("Any Number", technique.Number);
        Assert.Equal("Any Header", technique.Header);
        Assert.False(technique.IsCompleted);
    }

    [Fact]
    public void SetNumberIsSuccessfulIfValueIsSet()
    {
        Technique technique = CreateValid();
        technique.SetNumber("New Number");
        Assert.Equal("New Number", technique.Number);
    }

    [Fact]
    public void SetHeaderIsSuccessfulIfValueIsSet()
    {
        Technique technique = CreateValid();
        technique.SetHeader("New Header");
        Assert.Equal("New Header", technique.Header);
    }

    [Fact]
    public void MarkAsCompletedSetsFlag()
    {
        Technique technique = CreateValid();
        technique.MarkAsCompleted();
        Assert.True(technique.IsCompleted);
    }
}
