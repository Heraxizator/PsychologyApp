using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class LuscherTestViewModel : BaseTestViewModel
{
    private readonly LuscherMode _mode;
    private readonly IUserProgressService? _userProgressService;
    private int _lastCoValue;
    private double _lastBkValue;

    public LuscherMode Mode => _mode;
    public bool IsStandardMode => _mode == LuscherMode.Standard;
    public bool IsBriefMode => _mode == LuscherMode.Brief;

    public ObservableCollection<ResultItem> ResultItems { get; private set; } = [];

    public string PageTitle => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardTitle
        : AppStrings.TestsBriefTitle;

    public string ColorInstruction => AppStrings.TestsColorInstruction;

    public string MoreInfoHeader => AppStrings.TestsMoreInfo;

    public string MoreInfoBody => _mode == LuscherMode.Standard
        ? AppStrings.TestsStandardDescription
        : AppStrings.TestsBriefDescription;

    public string RestartButtonText => AppStrings.TestsRestart;

    public string FirstColorLabel => AppStrings.TestsFirstColor;

    public string SecondColorLabel => AppStrings.TestsSecondColor;

    public LuscherTestViewModel() { }

    public LuscherTestViewModel(
        LuscherMode mode,
        INavigation navigation,
        INavigationService navigationService,
        IUserProgressService? userProgressService = null)
    {
        _mode = mode;
        _userProgressService = userProgressService;
        BindNavigation(navigation, navigationService);
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = PageTitle;

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

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ColorInstruction),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(RestartButtonText),
            nameof(FirstColorLabel),
            nameof(SecondColorLabel));

        if (_mode == LuscherMode.Brief)
        {
            CurrentInstruction = _colourSelectedItems.Count == 0
                ? AppStrings.TestsLuscherFirstInstruction
                : AppStrings.TestsLuscherSecondInstruction;
            RefreshBriefResultText();
        }
        else
        {
            RefreshStandardResultLabels();
        }
    }

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

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        if (_mode == LuscherMode.Standard)
        {
            SaveStandardResult(colourValue, colourMeaningVoted);
            return;
        }

        SaveBriefResult(colourValue, colourMeaningVoted, colourMeaningUnvoted);
    }

    private void SaveStandardResult(ColourValue colourValue, ColourMeaning colourMeaningVoted)
    {
        _colourSelectedItems.Add((colourValue, colourMeaningVoted));

        if (_colourSelectedItems.Count != 8)
        {
            return;
        }

        _lastCoValue = LuscherScoring.CalculateCo(_colourSelectedItems);
        _lastBkValue = LuscherScoring.CalculateBk(_colourSelectedItems);

        ResultItems.Add(new ResultItem
        {
            PropertyName = AppStrings.TestsCoLabel,
            PropertyValue = AppStrings.TestsScoreOutOf(_lastCoValue, "32"),
            PropertyText = LuscherStrings.InterpretCo(_lastCoValue)
        });

        ResultItems.Add(new ResultItem
        {
            PropertyName = AppStrings.TestsBkLabel,
            PropertyValue = AppStrings.TestsDecimalScoreOutOf(Math.Round(_lastBkValue, 2), "3.2"),
            PropertyText = LuscherStrings.InterpretBk(_lastBkValue)
        });

        SetFinish();
        PersistStandardResultAsync(_lastCoValue, _lastBkValue).FireAndForget();
    }

    private void SaveBriefResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        if (_colourSelectedItems.Count == 0)
        {
            _colourSelectedItems.Add((colourValue, colourMeaningVoted));
            CurrentInstruction = AppStrings.TestsLuscherSecondInstruction;
            FirstResult = ColourStrings.GetExplanation(colourValue, ColourMeaningType.Wanted);
            FirstColor = Color.FromArgb(colourValue.Code);
            FirstName = ColourStrings.GetColorName(colourValue);
            return;
        }

        _colourSelectedItems.Add((colourValue, colourMeaningUnvoted));
        SecondResult = ColourStrings.GetExplanation(colourValue, ColourMeaningType.Unwanted);
        SecondColor = Color.FromArgb(colourValue.Code);
        SecondName = ColourStrings.GetColorName(colourValue);
        SetFinish();
        PersistBriefResultAsync().FireAndForget();
    }

    private async Task PersistStandardResultAsync(int coValue, double bkValue)
    {
        if (_userProgressService is null)
        {
            return;
        }

        string summary = $"{AppStrings.TestsCoLabel}: {coValue}; {AppStrings.TestsBkLabel}: {Math.Round(bkValue, 2)}";
        string detailJson = JsonSerializer.Serialize(new
        {
            co = coValue,
            bk = Math.Round(bkValue, 2),
            colors = _colourSelectedItems.Select(item => new
            {
                code = item.Item1.Code,
                name = ColourStrings.GetColorName(item.Item1)
            }).ToList()
        });

        await _userProgressService.SaveTestResultAsync(TestIds.LuscherStandard, coValue, summary, detailJson);
    }

    private async Task PersistBriefResultAsync()
    {
        if (_userProgressService is null)
        {
            return;
        }

        string summary = $"{FirstName} / {SecondName}";
        string detailJson = JsonSerializer.Serialize(new
        {
            first = new { name = FirstName, text = FirstResult },
            second = new { name = SecondName, text = SecondResult }
        });

        await _userProgressService.SaveTestResultAsync(TestIds.LuscherBrief, null, summary, detailJson);
    }
}

public class ResultItem
{
    public string? PropertyName { get; set; }
    public string? PropertyValue { get; set; }
    public string? PropertyText { get; set; }
}
