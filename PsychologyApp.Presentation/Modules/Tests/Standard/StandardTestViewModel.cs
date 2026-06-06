using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.ViewModels.Tests;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class StandardTestViewModel : BaseTestViewModel
{
    public ObservableCollection<ResultItem> ResultItems { get; private set; } = [];

    public StandardTestViewModel() { }

    public StandardTestViewModel(INavigation navigation)
    {
        BindNavigation(navigation);
        ModuleName = "Детектор";
        PageName = "Стандартный тест Люшера";

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
        _colourSelectedItems.Clear();
        ResultItems = [];
        SetColorsVisibility();
        SetStart();
    }

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        _colourSelectedItems.Add((colourValue, colourMeaningVoted));

        if (_colourSelectedItems.Count == 8)
        {
            int coValue = LuscherScoring.CalculateCo(_colourSelectedItems);
            double bkValue = LuscherScoring.CalculateBk(_colourSelectedItems);

            ResultItems.Add(new()
            {
                PropertyName = "Суммарное отклонение от аутогенной нормы (СО)",
                PropertyValue = $"{coValue} из 32",
                PropertyText = LuscherScoring.InterpretCo(coValue)
            });

            ResultItems.Add(new()
            {
                PropertyName = "Вегетативный коэффициент (ВК)",
                PropertyValue = $"{Math.Round(bkValue, 2)} из 3.2",
                PropertyText = LuscherScoring.InterpretBk(bkValue)
            });

            SetFinish();
        }
    }
}

public class ResultItem
{
    public string? PropertyName { get; set; }
    public string? PropertyValue { get; set; }
    public string? PropertyText { get; set; }
}
