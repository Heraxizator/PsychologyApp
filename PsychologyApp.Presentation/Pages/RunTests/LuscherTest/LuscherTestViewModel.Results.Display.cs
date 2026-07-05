using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel
{
    public string ResultsTitle => AppStrings.TestsLuscherResultsTitle;

    public string FirstColorRoleLabel => AppStrings.TestsLuscherWantedRole;

    public string SecondColorRoleLabel => AppStrings.TestsLuscherUnwantedRole;

    public string StandardFirstPassTitle => AppStrings.TestsLuscherHistoryFirstPass;

    public string StandardSecondPassTitle => AppStrings.TestsLuscherHistorySecondPass;

    public IReadOnlyList<LuscherColorDisplayItem> StandardFirstPassDisplay { get; private set; } = [];

    public IReadOnlyList<LuscherColorDisplayItem> StandardSecondPassDisplay { get; private set; } = [];

    private void RefreshStandardPassDisplay()
    {
        StandardFirstPassDisplay = LuscherColorDisplayFactory.FromSelections(_firstPassSelections);
        StandardSecondPassDisplay = LuscherColorDisplayFactory.FromSelections(_colourSelectedItems);
        Notify(
            nameof(StandardFirstPassDisplay),
            nameof(StandardSecondPassDisplay),
            nameof(StandardFirstPassTitle),
            nameof(StandardSecondPassTitle));
    }

    private void NotifyBriefResultDisplay() =>
        Notify(
            nameof(ResultsTitle),
            nameof(FirstColorRoleLabel),
            nameof(SecondColorRoleLabel));
}
