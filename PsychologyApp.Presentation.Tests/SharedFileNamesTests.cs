using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>"Delete all my data" must find every file the app wrote to share, and leave everything else in the cache.</summary>
public sealed class SharedFileNamesTests
{
    private static readonly DateTime Now = new(2026, 10, 21, 9, 5, 0);

    public static TheoryData<string> Shared =>
    [
        SharedFileNames.JournalExport(Now),
        SharedFileNames.Backup(passphraseProtected: false, Now),
        SharedFileNames.Backup(passphraseProtected: true, Now),
        SharedFileNames.SpecialistSummary(Now),
        SharedFileNames.ErrorLog(Now)
    ];

    [Theory]
    [MemberData(nameof(Shared))]
    public void Every_file_the_app_shares_is_found_by_the_eraser(string fileName) =>
        Assert.True(SharedFileNames.IsOwn(fileName), fileName);

    [Theory]
    [InlineData("MaterialIcons-Regular.ttf")]
    [InlineData("3f2a9c.mp3")]
    [InlineData("other.bin")]
    public void Files_that_are_not_the_persons_stay(string fileName) =>
        Assert.False(SharedFileNames.IsOwn(fileName));
}
