using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

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

    public int BriefStep => _colourSelectedItems.Count;

    public int BriefStepCount => 2;

    public string BriefStepLabel => AppStrings.TestsStepOf(BriefStep + 1, BriefStepCount);

    public bool ShowBriefProgress => IsBriefMode && IsStart;

    private void NotifyBriefProgress() =>
        Notify(nameof(BriefStep), nameof(BriefStepLabel), nameof(ShowBriefProgress));

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
            nameof(SecondColorLabel),
            nameof(BriefStepLabel));

        if (_mode == LuscherMode.Brief)
        {
            CurrentInstruction = _colourSelectedItems.Count == 0
                ? AppStrings.TestsLuscherFirstInstruction
                : AppStrings.TestsLuscherSecondInstruction;
            NotifyBriefProgress();
            RefreshBriefResultText();
        }
        else
        {
            RefreshStandardResultLabels();
        }
    }
}
