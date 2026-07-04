using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel
{
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

        if (_passNumber == 1)
        {
            _firstPassSelections = _colourSelectedItems.ToList();
            _colourSelectedItems.Clear();
            _passNumber = 2;
            SetColorsVisibility();
            SetStart();
            CurrentInstruction = AppStrings.TestsLuscherSecondPassInstruction;
            NotifyStandardPassProgress();
            return;
        }

        _lastCoValue = LuscherScoring.CalculateCoBetweenPasses(_firstPassSelections, _colourSelectedItems);
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
        LoadRecommendationAsync(_lastCoValue).FireAndForget();
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
            NotifyBriefProgress();
            return;
        }

        _colourSelectedItems.Add((colourValue, colourMeaningUnvoted));
        SecondResult = ColourStrings.GetExplanation(colourValue, ColourMeaningType.Unwanted);
        SecondColor = Color.FromArgb(colourValue.Code);
        SecondName = ColourStrings.GetColorName(colourValue);
        SetFinish();
        PersistBriefResultAsync().FireAndForget();
    }

    private Task PersistStandardResultAsync(int coValue, double bkValue)
    {
        if (_userProgressService is null)
        {
            return Task.CompletedTask;
        }

        string summary = $"{AppStrings.TestsCoLabel}: {coValue}; {AppStrings.TestsBkLabel}: {Math.Round(bkValue, 2)}";
        IReadOnlyList<LuscherColorSelection> firstPass = MapSelections(_firstPassSelections);
        IReadOnlyList<LuscherColorSelection> secondPass = MapSelections(_colourSelectedItems);

        return _luscherResultService.SaveStandardAsync(
            _userProgressService,
            summary,
            coValue,
            bkValue,
            firstPass,
            secondPass);
    }

    private Task PersistBriefResultAsync()
    {
        if (_userProgressService is null)
        {
            return Task.CompletedTask;
        }

        string summary = $"{FirstName} / {SecondName}";
        string? firstCode = _colourSelectedItems.Count > 0 ? _colourSelectedItems[0].Item1.Code : null;
        string? secondCode = _colourSelectedItems.Count > 1 ? _colourSelectedItems[1].Item1.Code : null;

        return _luscherResultService.SaveBriefAsync(
            _userProgressService,
            summary,
            FirstName,
            SecondName,
            firstCode,
            secondCode,
            FirstResult,
            SecondResult);
    }

    private static IReadOnlyList<LuscherColorSelection> MapSelections(
        IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> selections) =>
        selections
            .Select(item => new LuscherColorSelection(item.Colour.Code, ColourStrings.GetColorName(item.Colour)))
            .ToList();
}
