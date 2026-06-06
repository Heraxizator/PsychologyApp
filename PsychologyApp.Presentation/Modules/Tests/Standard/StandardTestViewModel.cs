using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels.Tests;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class StandardTestViewModel : BaseTestViewModel
{
    private int _lastCoValue;
    private double _lastBkValue;

    public ObservableCollection<ResultItem> ResultItems { get; private set; } = [];

    public string PageTitle => AppStrings.TestsStandardTitle;
    public string ColorInstruction => AppStrings.TestsColorInstruction;
    public string MoreInfoHeader => AppStrings.TestsMoreInfo;
    public string MoreInfoBody => AppStrings.TestsStandardDescription;
    public string RestartButtonText => AppStrings.TestsRestart;

    public StandardTestViewModel() { }

    public StandardTestViewModel(INavigation navigation)
    {
        BindNavigation(navigation);
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsStandardTitle;
        UserPreferences.Changed += OnPreferencesChanged;

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

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(ColorInstruction));
        OnPropertyChanged(nameof(MoreInfoHeader));
        OnPropertyChanged(nameof(MoreInfoBody));
        OnPropertyChanged(nameof(RestartButtonText));
        RefreshResultLabels();
    }

    private void RefreshResultLabels()
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
            _lastCoValue = LuscherScoring.CalculateCo(_colourSelectedItems);
            _lastBkValue = LuscherScoring.CalculateBk(_colourSelectedItems);

            ResultItems.Add(new()
            {
                PropertyName = AppStrings.TestsCoLabel,
                PropertyValue = AppStrings.TestsScoreOutOf(_lastCoValue, "32"),
                PropertyText = LuscherStrings.InterpretCo(_lastCoValue)
            });

            ResultItems.Add(new()
            {
                PropertyName = AppStrings.TestsBkLabel,
                PropertyValue = AppStrings.TestsDecimalScoreOutOf(Math.Round(_lastBkValue, 2), "3.2"),
                PropertyText = LuscherStrings.InterpretBk(_lastBkValue)
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
