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
        IReadOnlyList<LuscherColorSelection> colors = _colourSelectedItems
            .Select(item => new LuscherColorSelection(item.Item1.Code, ColourStrings.GetColorName(item.Item1)))
            .ToList();

        return _luscherResultService.SaveStandardAsync(_userProgressService, summary, coValue, bkValue, colors);
    }

    private Task PersistBriefResultAsync()
    {
        if (_userProgressService is null)
        {
            return Task.CompletedTask;
        }

        string summary = $"{FirstName} / {SecondName}";
        return _luscherResultService.SaveBriefAsync(_userProgressService, summary, FirstName, SecondName, FirstResult, SecondResult);
    }
}
