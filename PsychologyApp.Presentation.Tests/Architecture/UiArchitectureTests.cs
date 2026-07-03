using System.Text.RegularExpressions;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Architecture;

public sealed class UiArchitectureTests
{
    private static readonly string PresentationRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation"));

    private static readonly Regex RawButtonRegex = new(
        @"<\s*Button\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex RawActivityIndicatorRegex = new(
        @"<\s*ActivityIndicator\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [Fact]
    public void Pages_ShouldNotUseRawButtonOrActivityIndicator()
    {
        List<string> violations = [];
        foreach (string file in Directory.EnumerateFiles(
                     Path.Combine(PresentationRoot, "Pages"),
                     "*.xaml",
                     SearchOption.AllDirectories))
        {
            foreach (string line in File.ReadLines(file))
            {
                if (RawButtonRegex.IsMatch(line) || RawActivityIndicatorRegex.IsMatch(line))
                {
                    violations.Add($"{file}: {line.Trim()}");
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Pages must use ButtonView and ProgressBarView instead of raw MAUI controls:\n"
            + string.Join("\n", violations.Take(20)));
    }

    [Theory]
    [InlineData("Widgets/Audio/MusicTransportControlsView.xaml", "PressFeedbackBehavior", "AttachToAllTapTargets=\"True\"")]
    [InlineData("Widgets/Test/TestHistoryEntryView.xaml", "ListItemRevealBehavior", "ListItemRevealBehavior")]
    public void Phase3Widgets_ShouldDeclareRequiredBehaviors(
        string relativePath,
        string behaviorType,
        string requiredFragment)
    {
        string path = Path.Combine(PresentationRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(path), $"Missing widget XAML: {path}");

        string content = File.ReadAllText(path);
        Assert.Contains(behaviorType, content, StringComparison.Ordinal);
        Assert.Contains(requiredFragment, content, StringComparison.Ordinal);
    }
}
