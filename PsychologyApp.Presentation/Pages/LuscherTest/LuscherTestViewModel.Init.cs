using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Pages.LuscherTest;

public partial class LuscherTestViewModel
{
    private void ToRestart(object obj) => Init();

    private void Init()
    {
        if (_mode == LuscherMode.Brief)
        {
            CurrentInstruction = AppStrings.TestsLuscherFirstInstruction;
        }

        _colourSelectedItems.Clear();
        ResultItems = [];
        SetColorsVisibility();
        SetStart();
        NotifyBriefProgress();
    }
}
