using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.ViewModels.Tests;

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
    }
}
