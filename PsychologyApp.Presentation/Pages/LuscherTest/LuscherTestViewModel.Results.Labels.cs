using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.LuscherTest;

public partial class LuscherTestViewModel
{
    private void RefreshStandardResultLabels()
    {
        if (ResultItems.Count < 2)
        {
            return;
        }

        ResultItems[0].PropertyName = AppStrings.TestsCoLabel;
        ResultItems[0].PropertyValue = AppStrings.TestsScoreOutOf(_lastCoValue, "32");
        ResultItems[0].PropertyText = LuscherStrings.InterpretCo(_lastCoValue);
        ResultItems[1].PropertyName = AppStrings.TestsBkLabel;
        ResultItems[1].PropertyValue = AppStrings.TestsDecimalScoreOutOf(Math.Round(_lastBkValue, 2), "3.2");
        ResultItems[1].PropertyText = LuscherStrings.InterpretBk(_lastBkValue);
        OnPropertyChanged(nameof(ResultItems));
    }

    private void RefreshBriefResultText()
    {
        if (_colourSelectedItems.Count == 0)
        {
            return;
        }

        (ColourValue colour, _) = _colourSelectedItems[0];
        FirstResult = ColourStrings.GetExplanation(colour, ColourMeaningType.Wanted);
        FirstName = ColourStrings.GetColorName(colour);

        if (_colourSelectedItems.Count < 2)
        {
            return;
        }

        (ColourValue secondColour, _) = _colourSelectedItems[1];
        SecondResult = ColourStrings.GetExplanation(secondColour, ColourMeaningType.Unwanted);
        SecondName = ColourStrings.GetColorName(secondColour);
    }
}
