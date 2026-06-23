using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.LuscherTest;

public partial class LuscherTestViewModel
{
    public string PageTitle => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardTitle
        : AppStrings.TestsBriefTitle;

    public string ColorInstruction => AppStrings.TestsColorInstruction;

    public string MoreInfoHeader => AppStrings.TestsMoreInfo;

    public string MoreInfoBody => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardDescription
        : AppStrings.TestsBriefDescription;

    public string RestartButtonText => AppStrings.TestsRestart;

    public string BackToListButtonText => AppStrings.TestsBackToList;

    public string FirstColorLabel => AppStrings.TestsFirstColor;

    public string SecondColorLabel => AppStrings.TestsSecondColor;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ColorInstruction),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(RestartButtonText),
            nameof(BackToListButtonText),
            nameof(FirstColorLabel),
            nameof(SecondColorLabel));

        if (_mode == LuscherMode.Brief)
        {
            CurrentInstruction = _colourSelectedItems.Count == 0
                ? AppStrings.TestsLuscherFirstInstruction
                : AppStrings.TestsLuscherSecondInstruction;
            RefreshBriefResultText();
        }
        else
        {
            RefreshStandardResultLabels();
        }
    }
}
