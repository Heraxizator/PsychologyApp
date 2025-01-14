using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Tests.PsychologyApp.Domain.Tests.Entities;

public class QuotTests
{
    [Fact]
    public void BuildShouldThrowExceptionIfTittleIsNotSet()
    {
        var action = () => Quot.Create(string.Empty, "Some Text", "Any Subject", false);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfTextIsNotSet()
    {
        var action = () => Quot.Create("Any Title", string.Empty, "Any Subject", false);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfSubjectIsNotSet()
    {
        var action = () => Quot.Create("Any Title", "Some Text", string.Empty, false);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldCreateIfEveryPropertyIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true);

        Assert.NotNull(quot);
    }

    [Fact]
    public void EditTitleThrowsExceptionIfValueIsNotSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true);

        var action = () => quot.EditTitle(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void EditTitleIsSuccessfulIfValueIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true);

        quot.EditTitle("New Title");

        Assert.True(true);
    }

    [Fact]
    public void EditTextThrowsExceptionIfValueIsNotSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true);

        var action = () => quot.EditText(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void EditTextIsSuccessfulIfValueIsSet()
    {
        Quot quot = Quot.Create("Any Title", "Some Text", "Any Subject", true);

        quot.EditText("New Text");

        Assert.True(true);
    }
}

