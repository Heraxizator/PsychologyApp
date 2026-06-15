using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class PolarityViewModel
{
    private string positive = string.Empty;
    public string Positive
    {
        get => positive;
        set => SetProperty(ref positive, value);
    }

    private string negative = string.Empty;
    public string Negative
    {
        get => negative;
        set => SetProperty(ref negative, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }

    private Polarity polarity = default!;
    public Polarity Polarity
    {
        get => polarity;
        set => SetProperty(ref polarity, value);
    }
}
