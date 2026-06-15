using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public abstract partial class BaseTestViewModel : BaseViewModel
{
    public ICommand? Restart { get; set; }
    public ICommand? BlackHandler { get; set; }
    public ICommand? RedHandler { get; set; }
    public ICommand? BlueHandler { get; set; }
    public ICommand? PurpleHandler { get; set; }
    public ICommand? YellowHandler { get; set; }
    public ICommand? BrownHandler { get; set; }
    public ICommand? GreenHandler { get; set; }
    public ICommand? GrayHandler { get; set; }

    protected abstract void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted);
}
