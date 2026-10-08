using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>Remembering which catalog sections are folded.</summary>
public class CollapsedSectionsTests
{
    [Fact]
    public void NothingStoredMeansEverythingIsOpen()
    {
        Assert.Empty(CollapsedSections.Parse(null));
        Assert.Empty(CollapsedSections.Parse(""));
    }

    [Fact]
    public void ASavedSetComesBackAsItWas()
    {
        IReadOnlySet<string> set = CollapsedSections.Parse(CollapsedSections.Format(["Mind", "Body"]));

        Assert.Equal(["Body", "Mind"], set.OrderBy(k => k));
    }

    [Fact]
    public void UnknownOrDamagedKeysAreIgnoredSoNothingElseGetsFolded()
    {
        IReadOnlySet<string> set = CollapsedSections.Parse("Body,Nonsense,None, ,Heart");

        Assert.Equal(["Body", "Heart"], set.OrderBy(k => k));
    }

    [Fact]
    public void ToggleFoldsAnOpenSectionAndOpensAFoldedOne()
    {
        IReadOnlySet<string> folded = CollapsedSections.Toggle(new HashSet<string>(), "Mind");
        Assert.Contains("Mind", folded);

        IReadOnlySet<string> opened = CollapsedSections.Toggle(folded, "Mind");
        Assert.Empty(opened);
    }

    [Fact]
    public void ToggleLeavesTheOtherSectionsAlone()
    {
        IReadOnlySet<string> result = CollapsedSections.Toggle(new HashSet<string> { "Body" }, "Heart");

        Assert.Equal(["Body", "Heart"], result.OrderBy(k => k));
    }

    [Fact]
    public void FormatDoesNotRepeatAKey()
    {
        Assert.Equal("Body", CollapsedSections.Format(["Body", "Body"]));
    }
}
