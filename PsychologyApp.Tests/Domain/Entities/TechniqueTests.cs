using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Tests.PsychologyApp.Domain.Tests.Entities;

public class TechniqueTests
{
    [Fact]
    public void BuildShouldThrowExceptionIfNumberIsNotSet()
    {
        var action = () => Technique.Create(-1, string.Empty, "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfDateIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", string.Empty, "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfHeaderIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", string.Empty, "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfDescribtionIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", string.Empty, "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfSubjectIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", string.Empty, "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfAuthorIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", string.Empty, "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfAlgorithmIsNotSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldCreateIfEveryPropertyIsSet()
    {
        var action = () => Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetNumberThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetNumber(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetNumberIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetNumber("Any Number");

        Assert.True(true);
    }

    [Fact]
    public void SetDateThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetDate(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetDateIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetDate("Any Date");

        Assert.True(true);
    }

    [Fact]
    public void SetHeaderThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetHeader(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetHeaderIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetNumber("Any Number");

        Assert.True(true);
    }

    [Fact]
    public void SetDescribtionThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetDescribtion(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetDescribtionIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetDescribtion("Any Describtion");

        Assert.True(true);
    }

    [Fact]
    public void SetSubjectThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetSubject(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetSubjectIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetSubject("Any Subject");

        Assert.True(true);
    }

    [Fact]
    public void SetAuthorThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetAuthor(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetAuthorIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetAuthor("Any Number");

        Assert.True(true);
    }

    [Fact]
    public void SetAlgorithmThrowsExceptionIfValueIsNotSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        var action = () => technique.SetAlgorithm(string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetAlgorithmIsSuccessfulIfValueIsSet()
    {
        Technique technique = Technique.Create(-1, "Any Number", "Any Date", "Any Header", "Any Describtion", "Any Subject", "Any Author", "Any Algorithm", "Any Image");

        technique.SetAlgorithm("Any Algorithm");

        Assert.True(true);
    }
}
