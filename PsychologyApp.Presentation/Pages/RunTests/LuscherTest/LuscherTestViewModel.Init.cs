using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel
{
    private void ToRestart(object obj) => Init();

    private void Init()
    {
        _passNumber = 1;
        _firstPassSelections.Clear();

        if (_mode == LuscherMode.Brief)
        {
            CurrentInstruction = AppStrings.TestsLuscherFirstInstruction;
        }
        else
        {
            CurrentInstruction = AppStrings.TestsColorInstruction;
        }

        _colourSelectedItems.Clear();
        ResultItems = [];
        _recommendedTechnique = null;
        HasRecommendation = false;
        SetColorsVisibility();
        SetStart();
        NotifyBriefProgress();
        NotifyStandardPassProgress();
    }
}
