using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public abstract partial class BaseTestViewModel
{
    protected readonly List<(ColourValue, ColourMeaning)> _colourSelectedItems = [];

    private sealed record ColorHandlerEntry(
        Action<ICommand?> AssignCommand,
        Action<object> Handler);

    protected void SetColorsVisibility()
    {
        IsBlack = true;
        IsRed = true;
        IsBlue = true;
        IsPurple = true;
        IsYellow = true;
        IsBrown = true;
        IsGreen = true;
        IsGray = true;
    }

    protected void SetStart()
    {
        IsStart = true;
        IsFinish = false;
    }

    protected void SetFinish()
    {
        IsStart = false;
        IsFinish = true;
    }

    protected void HandleColorSelection(ColourValue colour, ColourMeaning voted, ColourMeaning unvoted, Action<bool> setVisibility)
    {
        setVisibility(false);
        SaveResult(colour, voted, unvoted);
    }

    protected void InitializeColorHandlers()
    {
        foreach (ColorHandlerEntry entry in GetColorHandlerEntries())
        {
            entry.AssignCommand(new Command(entry.Handler));
        }
    }

    private IEnumerable<ColorHandlerEntry> GetColorHandlerEntries()
    {
        yield return new ColorHandlerEntry(
            command => BlackHandler = command,
            _ => HandleColorSelection(ColourValue.Black, ColourMeaning.BlackVoted, ColourMeaning.BlackUnvoted, v => IsBlack = v));
        yield return new ColorHandlerEntry(
            command => RedHandler = command,
            _ => HandleColorSelection(ColourValue.Red, ColourMeaning.RedVoted, ColourMeaning.RedUnvoted, v => IsRed = v));
        yield return new ColorHandlerEntry(
            command => BlueHandler = command,
            _ => HandleColorSelection(ColourValue.Blue, ColourMeaning.BlueVoted, ColourMeaning.BlueUnvoted, v => IsBlue = v));
        yield return new ColorHandlerEntry(
            command => PurpleHandler = command,
            _ => HandleColorSelection(ColourValue.Purple, ColourMeaning.PurpleVoted, ColourMeaning.PurpleUnvoted, v => IsPurple = v));
        yield return new ColorHandlerEntry(
            command => YellowHandler = command,
            _ => HandleColorSelection(ColourValue.Yellow, ColourMeaning.YellowVoted, ColourMeaning.YellowUnvoted, v => IsYellow = v));
        yield return new ColorHandlerEntry(
            command => BrownHandler = command,
            _ => HandleColorSelection(ColourValue.Brown, ColourMeaning.BrownVoted, ColourMeaning.BrownUnvoted, v => IsBrown = v));
        yield return new ColorHandlerEntry(
            command => GreenHandler = command,
            _ => HandleColorSelection(ColourValue.Green, ColourMeaning.GreenVoted, ColourMeaning.GreenUnvoted, v => IsGreen = v));
        yield return new ColorHandlerEntry(
            command => GrayHandler = command,
            _ => HandleColorSelection(ColourValue.Gray, ColourMeaning.GrayVoted, ColourMeaning.GrayUnvoted, v => IsGray = v));
    }
}
