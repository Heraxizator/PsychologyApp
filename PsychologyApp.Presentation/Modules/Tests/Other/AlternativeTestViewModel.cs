using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.ViewModels.Tests;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class AlternativeTestViewModel : BaseTestViewModel
{
    private const string FirstInstruction = "Выберите самый приятный для вас цвет";
    private const string SecondInstruction = "Из оставшихся выберите самый неприятный для вас цвет";

    public AlternativeTestViewModel() { }

    public AlternativeTestViewModel(INavigation navigation)
    {
        BindNavigation(navigation);

        ModuleName = "Детектор";
        PageName = "Краткий тест Люшера";

        Restart = new Command(ToRestart);
        BlackHandler = new Command(ToBlackHandler);
        RedHandler = new Command(ToRedHandler);
        BlueHandler = new Command(ToBlueHandler);
        PurpleHandler = new Command(ToPurpleHandler);
        YellowHandler = new Command(ToYellowCommand);
        BrownHandler = new Command(ToBrownHandler);
        GreenHandler = new Command(ToGreenHandler);
        GrayHandler = new Command(ToGrayHandler);

        Init();
    }

    private void ToRestart(object obj) => Init();

    private void Init()
    {
        CurrentInstruction = FirstInstruction;
        _colourSelectedItems.Clear();
        SetColorsVisibility();
        SetStart();
    }

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        if (_colourSelectedItems.Count == 0)
        {
            _colourSelectedItems.Add((colourValue, colourMeaningVoted));
            CurrentInstruction = SecondInstruction;
            FirstResult = colourMeaningVoted.Explaination;
            FirstColor = Color.FromArgb(colourValue.Code);
            FirstName = colourValue.ToString();
        }
        else
        {
            _colourSelectedItems.Add((colourValue, colourMeaningUnvoted));
            SecondResult = colourMeaningUnvoted.Explaination;
            SecondColor = Color.FromArgb(colourValue.Code);
            SecondName = colourValue.ToString();
            SetFinish();
        }
    }
}
