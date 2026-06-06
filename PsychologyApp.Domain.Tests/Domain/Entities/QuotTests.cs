using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Tests.Domain.Entities;

public class QuotTests
{
    [Fact]
    public void BuildShouldThrowExceptionIfTextIsNotSet()
    {
        var action = () => Quot.Create("Any Title", string.Empty, "Any Subject", false, false);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldCreateIfEveryPropertyIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true, false);

        Assert.Equal("Any Title", quot.Title);
        Assert.Equal("Some Text", quot.Text);
        Assert.Equal("Any Subject", quot.Theme);
        Assert.True(quot.IsReaded);
        Assert.False(quot.IsFavourite);
    }

    [Fact]
    public void EditTitleThrowsExceptionIfValueIsNotSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true, false);

        var action = () => quot.EditTitle(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void EditTitleIsSuccessfulIfValueIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true, false);

        quot.EditTitle("New Title");

        Assert.Equal("New Title", quot.Title);
    }

    [Fact]
    public void EditTextThrowsExceptionIfValueIsNotSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true, false);

        var action = () => quot.EditText(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void EditTextIsSuccessfulIfValueIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true, false);

        quot.EditText("New Text");

        Assert.Equal("New Text", quot.Text);
    }

    [Fact]
    public void MarkAsReaded_IsIdempotent()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", false, false);

        quot.MarkAsReaded();
        quot.MarkAsReaded();

        Assert.True(quot.IsReaded);
    }
}
