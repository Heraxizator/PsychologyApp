using System.Xml.Linq;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Architecture;

public sealed class LayerDependencyArchitectureTests
{
    private static readonly string SolutionRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Theory]
    [InlineData("PsychologyApp.Application/PsychologyApp.Application.csproj", "PsychologyApp.Domain")]
    public void Application_ShouldOnlyReferenceAllowedProjects(string projectPath, params string[] allowed)
    {
        AssertProjectReferences(projectPath, allowed);
    }

    [Theory]
    [InlineData(
        "PsychologyApp.Infrastructure/PsychologyApp.Infrastructure.csproj",
        "PsychologyApp.Application",
        "PsychologyApp.Domain")]
    public void Infrastructure_ShouldOnlyReferenceAllowedProjects(string projectPath, params string[] allowed)
    {
        AssertProjectReferences(projectPath, allowed);
    }

    [Theory]
    [InlineData(
        "PsychologyApp.Bootstrap/PsychologyApp.Bootstrap.csproj",
        "PsychologyApp.Application",
        "PsychologyApp.Infrastructure")]
    public void Bootstrap_ShouldOnlyReferenceAllowedProjects(string projectPath, params string[] allowed)
    {
        AssertProjectReferences(projectPath, allowed);
    }

    [Theory]
    [InlineData(
        "PsychologyApp.Presentation.Core/PsychologyApp.Presentation.Core.csproj",
        "PsychologyApp.Application",
        "PsychologyApp.Domain")]
    public void PresentationCore_ShouldOnlyReferenceAllowedProjects(string projectPath, params string[] allowed)
    {
        AssertProjectReferences(projectPath, allowed);
    }

    [Fact]
    public void Domain_ShouldNotReferenceOtherSolutionProjects()
    {
        AssertProjectReferences("PsychologyApp.Domain/PsychologyApp.Domain.csproj");
    }

    [Fact]
    public void Presentation_ShouldNotReferenceInfrastructureDirectly()
    {
        string[] references = ReadProjectReferences("PsychologyApp.Presentation/PsychologyApp.Presentation.csproj");
        Assert.DoesNotContain(
            references,
            reference => reference.Contains("PsychologyApp.Infrastructure", StringComparison.Ordinal));
    }

    private static void AssertProjectReferences(string relativeProjectPath, params string[] allowed)
    {
        string[] actual = ReadProjectReferences(relativeProjectPath);
        string[] expected = allowed
            .Select(NormalizeProjectName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        string[] actualNormalized = actual
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            actualNormalized.SequenceEqual(expected, StringComparer.Ordinal),
            $"{relativeProjectPath} references [{string.Join(", ", actualNormalized)}] "
            + $"but expected [{string.Join(", ", expected)}].");
    }

    private static string[] ReadProjectReferences(string relativeProjectPath)
    {
        string projectFile = Path.Combine(SolutionRoot, relativeProjectPath);
        Assert.True(File.Exists(projectFile), $"Project file not found: {projectFile}");

        XDocument document = XDocument.Load(projectFile);
        return document
            .Descendants()
            .Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => element.Attribute("Include")?.Value ?? string.Empty)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => NormalizeProjectName(Path.GetFileNameWithoutExtension(path)))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static string NormalizeProjectName(string projectName) =>
        projectName.Replace('\\', '/').Split('/').Last();
}
