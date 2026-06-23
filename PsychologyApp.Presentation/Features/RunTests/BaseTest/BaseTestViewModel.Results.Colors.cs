namespace PsychologyApp.Presentation.Features.RunTests.BaseTest;

public abstract partial class BaseTestViewModel
{
    protected Color? firstColor { get; set; }
    public Color? FirstColor
    {
        get => firstColor;
        set
        {
            if (firstColor != value)
            {
                firstColor = value;
                OnPropertyChanged(nameof(FirstColor));
            }
        }
    }

    protected Color? secondColor { get; set; }
    public Color? SecondColor
    {
        get => secondColor;
        set
        {
            if (secondColor != value)
            {
                secondColor = value;
                OnPropertyChanged(nameof(SecondColor));
            }
        }
    }
}
