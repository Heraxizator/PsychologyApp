using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel
{
    public string PageTitle => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardTitle
        : AppStrings.TestsBriefTitle;

    public string ColorInstruction
    {
        get
        {
            if (_mode == LuscherMode.Standard && IsStart)
            {
                return string.IsNullOrWhiteSpace(CurrentInstruction)
                    ? AppStrings.TestsColorInstruction
                    : CurrentInstruction;
            }

            return AppStrings.TestsColorInstruction;
        }
    }

    public string MoreInfoHeader => AppStrings.TestsMoreInfo;

    public string MoreInfoBody => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardDescription
        : AppStrings.TestsBriefDescription;

    public string RestartButtonText => AppStrings.TestsRestart;

    public string BackToListButtonText => AppStrings.TestsBackToList;

    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;

    public string FirstColorLabel => AppStrings.TestsFirstColor;

    public string SecondColorLabel => AppStrings.TestsSecondColor;

    public int BriefStep => _colourSelectedItems.Count;

    public int BriefStepCount => 2;

    public string BriefStepLabel => AppStrings.TestsStepOf(BriefStep + 1, BriefStepCount);

    public bool ShowBriefProgress => IsBriefMode && IsStart;

    public int StandardPassNumber => _passNumber;

    public int StandardPassCount => 2;

    public string StandardPassLabel => AppStrings.TestsLuscherPassOf(StandardPassNumber, StandardPassCount);

    public bool ShowStandardProgress => IsStandardMode && IsStart;

    private void NotifyBriefProgress() =>
        Notify(nameof(BriefStep), nameof(BriefStepLabel), nameof(ShowBriefProgress));

    private void NotifyStandardPassProgress() =>
        Notify(
            nameof(StandardPassNumber),
            nameof(StandardPassLabel),
            nameof(ShowStandardProgress),
            nameof(ColorInstruction));

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ColorInstruction),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(RestartButtonText),
            nameof(BackToListButtonText),
            nameof(TryTechniqueButtonText),
            nameof(FirstColorLabel),
            nameof(SecondColorLabel),
            nameof(BriefStepLabel),
            nameof(StandardPassLabel));

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
            NotifyStandardPassProgress();
        }
    }
}
