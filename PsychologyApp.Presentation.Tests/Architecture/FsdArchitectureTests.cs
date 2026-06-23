using System.Text.RegularExpressions;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Architecture;

public sealed class FsdArchitectureTests
{
    private static readonly string PresentationRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation"));

    private static readonly Regex UsingRegex = new(
        @"^\s*using\s+(?<ns>[\w\.]+)\s*;",
        RegexOptions.Compiled);

    private static readonly string[] ForbiddenSharedImports =
    [
        ".Pages.",
        ".Widgets.",
        ".Features.",
        ".Entities.",
        ".App."
    ];

    [Fact]
    public void SharedLayer_ShouldNotImportUpperLayers()
    {
        IEnumerable<string> violations = ScanCsFilesUnder("Shared")
            .SelectMany(file => ReadViolations(file, ForbiddenSharedImports, excludeLibNavigationPort: true));

        Assert.True(
            !violations.Any(),
            "Shared layer must not import pages/widgets/features/entities/app slices:\n"
            + string.Join("\n", violations.Take(30)));
    }

    [Fact]
    public void FeatureSlices_ShouldNotImportOtherFeatureSlices()
    {
        Dictionary<string, string> featureFiles = ScanCsFilesUnder("Features")
            .ToDictionary(path => path, GetFeatureSliceName);

        List<string> violations = [];
        foreach ((string file, string slice) in featureFiles)
        {
            foreach (string line in File.ReadLines(file))
            {
                Match match = UsingRegex.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                string ns = match.Groups["ns"].Value;
                if (!ns.StartsWith("PsychologyApp.Presentation.Features.", StringComparison.Ordinal))
                {
                    continue;
                }

                string importedSlice = ns.Split('.')[3];
                if (!string.Equals(importedSlice, slice, StringComparison.Ordinal))
                {
                    violations.Add($"{file}: {line.Trim()}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Cross-feature imports are forbidden:\n" + string.Join("\n", violations.Take(30)));
    }

    [Fact]
    public void CanonicalSliceFolders_ShouldExist()
    {
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "Pages")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "Widgets")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "Entities")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "Features")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "Shared", "Lib")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "App", "Routes")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "App", "Providers")));
        Assert.True(Directory.Exists(Path.Combine(PresentationRoot, "App", "DependencyInjection")));
    }

    [Fact]
    public void FeatureSlices_ShouldExposePublicApiEntryPoint()
    {
        string[] activeSlices = Directory.GetDirectories(Path.Combine(PresentationRoot, "Features"))
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToArray();

        List<string> missing = [];
        foreach (string slice in activeSlices)
        {
            string publicApi = Path.Combine(PresentationRoot, "Features", slice, "Index", $"{slice}PublicApi.cs");
            if (!File.Exists(publicApi))
            {
                missing.Add(slice);
            }
        }

        Assert.True(missing.Count == 0, "Each feature slice needs Index/{Slice}PublicApi.cs: " + string.Join(", ", missing));
    }

    [Fact]
    public void AppProviders_ShouldNotUseLegacyNames()
    {
        IEnumerable<string> legacyFiles = Directory
            .EnumerateFiles(Path.Combine(PresentationRoot, "App", "Providers"), "*.cs", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(name => name is not null && name.StartsWith("Legacy", StringComparison.Ordinal));

        Assert.True(!legacyFiles.Any(), "Remove legacy provider files: " + string.Join(", ", legacyFiles));
    }

    private static IEnumerable<string> ScanCsFilesUnder(string relativeFolder) =>
        Directory.EnumerateFiles(Path.Combine(PresentationRoot, relativeFolder), "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    private static string GetFeatureSliceName(string filePath)
    {
        string relative = Path.GetRelativePath(Path.Combine(PresentationRoot, "Features"), filePath);
        return relative.Split(Path.DirectorySeparatorChar)[0];
    }

    private static IEnumerable<string> ReadViolations(
        string filePath,
        string[] forbiddenMarkers,
        bool excludeLibNavigationPort)
    {
        foreach (string line in File.ReadLines(filePath))
        {
            Match match = UsingRegex.Match(line);
            if (!match.Success)
            {
                continue;
            }

            string ns = match.Groups["ns"].Value;
            if (!ns.StartsWith("PsychologyApp.Presentation.", StringComparison.Ordinal))
            {
                continue;
            }

            if (excludeLibNavigationPort
                && ns.StartsWith("PsychologyApp.Presentation.Shared.Lib.", StringComparison.Ordinal))
            {
                continue;
            }

            if (forbiddenMarkers.Any(marker => ns.Contains(marker, StringComparison.Ordinal)))
            {
                yield return $"{filePath}: {line.Trim()}";
            }
        }
    }
}
