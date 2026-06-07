using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels.Tests;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class AlternativeTestViewModel : BaseTestViewModel
{
    public string PageTitle => AppStrings.TestsBriefTitle;
    public string MoreInfoHeader => AppStrings.TestsMoreInfo;
    public string MoreInfoBody => AppStrings.TestsBriefDescription;
    public string RestartButtonText => AppStrings.TestsRestart;
    public string FirstColorLabel => AppStrings.TestsFirstColor;
    public string SecondColorLabel => AppStrings.TestsSecondColor;

    private readonly IUserProgressService? _userProgressService;

    public AlternativeTestViewModel() { }

    public AlternativeTestViewModel(INavigation navigation, IUserProgressService? userProgressService = null)
    {
        _userProgressService = userProgressService;
        BindNavigation(navigation);

        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsBriefTitle;

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
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(RestartButtonText),
            nameof(FirstColorLabel),
            nameof(SecondColorLabel));
        CurrentInstruction = _colourSelectedItems.Count == 0
            ? AppStrings.TestsLuscherFirstInstruction
            : AppStrings.TestsLuscherSecondInstruction;
        RefreshResultText();
    }

    private void RefreshResultText()
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
        CurrentInstruction = AppStrings.TestsLuscherFirstInstruction;
        _colourSelectedItems.Clear();
        SetColorsVisibility();
        SetStart();
    }

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        if (_colourSelectedItems.Count == 0)
        {
            _colourSelectedItems.Add((colourValue, colourMeaningVoted));
            CurrentInstruction = AppStrings.TestsLuscherSecondInstruction;
            FirstResult = ColourStrings.GetExplanation(colourValue, ColourMeaningType.Wanted);
            FirstColor = Color.FromArgb(colourValue.Code);
            FirstName = ColourStrings.GetColorName(colourValue);
        }
        else
        {
            _colourSelectedItems.Add((colourValue, colourMeaningUnvoted));
            SecondResult = ColourStrings.GetExplanation(colourValue, ColourMeaningType.Unwanted);
            SecondColor = Color.FromArgb(colourValue.Code);
            SecondName = ColourStrings.GetColorName(colourValue);
            SetFinish();
            PersistResultAsync().FireAndForget();
        }
    }

    private async Task PersistResultAsync()
    {
        if (_userProgressService is null)
        {
            return;
        }

        string summary = $"{FirstName} / {SecondName}";
        await _userProgressService.SaveTestResultAsync("luscher_brief", null, summary);
    }
}
