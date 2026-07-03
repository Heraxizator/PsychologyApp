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

    private static readonly Dictionary<string, string[]> AllowedFeatureCrossImports = new(StringComparer.Ordinal)
    {
        ["ManageProfile"] = ["ManageQuotes", "RunTechniqueSession"],
        ["RunTests"] = ["RunTechniqueSession"],
        ["SearchPhysics"] = ["RunTechniqueSession"],
    };

    private static readonly Dictionary<string, string[]> AllowedPageCrossImports = new(StringComparer.Ordinal)
    {
        ["ManageProfile"] = ["ManageQuotes", "RunTechniqueSession"],
        ["RunTests"] = ["RunTechniqueSession"],
    };
    private static readonly string[] CanonicalPageSlices =
    [
        "RunTests",
        "RunTechniqueSession",
        "ManageProfile",
        "ManageQuotes",
        "SearchPhysics",
        "SendReviewForm",
        "Onboarding",
        "PlayMusic"
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
    public void SharedLayer_ShouldNotContainFeatureSpecificFolders()
    {
        string[] forbidden = ["Practice"];
        List<string> found = [];
        foreach (string folder in forbidden)
        {
            string path = Path.Combine(PresentationRoot, "Shared", folder);
            if (Directory.Exists(path))
            {
                found.Add(path);
            }
        }

        Assert.True(found.Count == 0, "Remove feature-specific folders from Shared: " + string.Join(", ", found));
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
                if (string.Equals(importedSlice, slice, StringComparison.Ordinal))
                {
                    continue;
                }

                if (IsAllowedCrossImport(AllowedFeatureCrossImports, slice, importedSlice))
                {
                    continue;
                }

                violations.Add($"{file}: {line.Trim()}");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Cross-feature imports are forbidden:\n" + string.Join("\n", violations.Take(30)));
    }

    [Fact]
    public void PageFolders_ShouldLiveUnderCanonicalFeatureSlices()
    {
        string pagesRoot = Path.Combine(PresentationRoot, "Pages");
        List<string> violations = [];

        foreach (string directory in Directory.GetDirectories(pagesRoot))
        {
            string slice = Path.GetFileName(directory);
            if (!CanonicalPageSlices.Contains(slice, StringComparer.Ordinal))
            {
                violations.Add($"Unexpected top-level Pages folder: {slice}");
                continue;
            }

            if (string.Equals(slice, "Onboarding", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (string pageFolder in Directory.GetDirectories(directory))
            {
                foreach (string nested in Directory.GetDirectories(pageFolder))
                {
                    string nestedName = Path.GetFileName(nested);
                    if (!string.Equals(nestedName, "Index", StringComparison.Ordinal)
                        && !string.Equals(nestedName, "SubViewModels", StringComparison.Ordinal))
                    {
                        violations.Add($"Page folder may only contain an Index subfolder: {slice}/{Path.GetFileName(pageFolder)}/{nestedName}");
                    }
                }
            }
        }

        Assert.True(violations.Count == 0, string.Join("\n", violations));
    }

    [Fact]
    public void PageSlices_ShouldNotImportOtherFeatureSlices()
    {
        List<string> violations = [];
        foreach (string file in ScanCsFilesUnder("Pages"))
        {
            string slice = GetPageSliceName(file);
            if (string.IsNullOrWhiteSpace(slice))
            {
                continue;
            }

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
                if (string.Equals(importedSlice, slice, StringComparison.Ordinal))
                {
                    continue;
                }

                if (IsAllowedCrossImport(AllowedPageCrossImports, slice, importedSlice))
                {
                    continue;
                }

                violations.Add($"{file}: {line.Trim()}");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Pages must not import other feature slices:\n" + string.Join("\n", violations.Take(30)));
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
            .OfType<string>()
            .Where(name => name.StartsWith("Legacy", StringComparison.Ordinal));

        Assert.True(!legacyFiles.Any(), "Remove legacy provider files: " + string.Join(", ", legacyFiles));
    }

    [Fact]
    public void GlobalUsings_ShouldNotImportFeatureSlices()
    {
        string globalUsingsPath = Path.Combine(PresentationRoot, "GlobalUsings.cs");
        Assert.True(File.Exists(globalUsingsPath), "GlobalUsings.cs is missing.");

        List<string> violations = [];
        foreach (string line in File.ReadLines(globalUsingsPath))
        {
            Match match = UsingRegex.Match(line);
            if (!match.Success)
            {
                continue;
            }

            string ns = match.Groups["ns"].Value;
            if (ns.StartsWith("PsychologyApp.Presentation.Features.", StringComparison.Ordinal))
            {
                violations.Add(line.Trim());
            }

            if (string.Equals(ns, "PsychologyApp.Presentation.App", StringComparison.Ordinal))
            {
                violations.Add(line.Trim());
            }
        }

        Assert.True(
            violations.Count == 0,
            "GlobalUsings must not import feature slices or App root namespace:\n"
            + string.Join("\n", violations));
    }

    [Fact]
    public void SharedLayer_ShouldNotReferenceAppNamespace()
    {
        List<string> violations = [];
        foreach (string file in ScanCsFilesUnder("Shared"))
        {
            foreach (string line in File.ReadLines(file))
            {
                Match match = UsingRegex.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                string ns = match.Groups["ns"].Value;
                if (ns.StartsWith("PsychologyApp.Presentation.App", StringComparison.Ordinal)
                    && !ns.StartsWith("PsychologyApp.Presentation.App.Routes", StringComparison.Ordinal))
                {
                    violations.Add($"{file}: {line.Trim()}");
                }
            }

            string content = File.ReadAllText(file);
            if (content.Contains("AppShell", StringComparison.Ordinal))
            {
                violations.Add($"{file}: references AppShell");
            }
        }

        Assert.True(
            violations.Count == 0,
            "Shared must not reference App namespace or AppShell:\n" + string.Join("\n", violations.Take(30)));
    }

    [Fact]
    public void Presentation_ShouldNotReferenceInfrastructure()
    {
        List<string> violations = [];
        foreach (string file in Directory.EnumerateFiles(PresentationRoot, "*.cs", SearchOption.AllDirectories)
                     .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                     .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)))
        {
            foreach (string line in File.ReadLines(file))
            {
                if (line.Contains("PsychologyApp.Infrastructure", StringComparison.Ordinal))
                {
                    violations.Add($"{file}: {line.Trim()}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Presentation must not reference Infrastructure:\n" + string.Join("\n", violations.Take(30)));
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

    private static string GetPageSliceName(string filePath)
    {
        string relative = Path.GetRelativePath(Path.Combine(PresentationRoot, "Pages"), filePath);
        string[] parts = relative.Split(Path.DirectorySeparatorChar);
        if (parts.Length < 2)
        {
            return parts[0];
        }

        return parts[0];
    }

    private static bool IsAllowedCrossImport(
        Dictionary<string, string[]> allowlist,
        string sourceSlice,
        string importedSlice) =>
        allowlist.TryGetValue(sourceSlice, out string[]? allowed)
        && allowed.Contains(importedSlice, StringComparer.Ordinal);

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
