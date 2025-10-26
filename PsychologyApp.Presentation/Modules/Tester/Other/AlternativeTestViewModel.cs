using PsychologyApp.Detector.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.ViewModels.TestViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TestViewModels;

public class AlternativeTestViewModel : BaseTestViewModel
{

    private const string firstInstruction = "Выберите приятный вам цвет";
    private const string secondInstruction = "А теперь выберите неприятный вам цвет";

    public AlternativeTestViewModel() { }

    public AlternativeTestViewModel(INavigation navigation)
    {
        Navigation = navigation;

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

    private void ToRestart(object obj)
    {
        Init();
    }

    private void Init()
    {
        CurrentInstruction = firstInstruction;

        _colourSelectedItems.Clear();

        SetColorsVisibility();

        SetStart();
    }

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        if (_colourSelectedItems.Any() is false)
        {
            _colourSelectedItems.Add((colourValue, colourMeaningVoted));

            CurrentInstruction = secondInstruction;

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
