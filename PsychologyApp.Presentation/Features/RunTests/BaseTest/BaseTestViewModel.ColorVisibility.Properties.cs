namespace PsychologyApp.Presentation.Features.RunTests.BaseTest;

public abstract partial class BaseTestViewModel
{
    protected bool isBlack { get; set; }
    public bool IsBlack
    {
        get => isBlack;
        set
        {
            if (isBlack != value)
            {
                isBlack = value;
                OnPropertyChanged(nameof(IsBlack));
            }
        }
    }

    protected bool isRed { get; set; }
    public bool IsRed
    {
        get => isRed;
        set
        {
            if (isRed != value)
            {
                isRed = value;
                OnPropertyChanged(nameof(IsRed));
            }
        }
    }

    protected bool isBlue { get; set; }
    public bool IsBlue
    {
        get => isBlue;
        set
        {
            if (isBlue != value)
            {
                isBlue = value;
                OnPropertyChanged(nameof(IsBlue));
            }
        }
    }

    protected bool isPurple { get; set; }
    public bool IsPurple
    {
        get => isPurple;
        set
        {
            if (isPurple != value)
            {
                isPurple = value;
                OnPropertyChanged(nameof(IsPurple));
            }
        }
    }

    protected bool isYellow { get; set; }
    public bool IsYellow
    {
        get => isYellow;
        set
        {
            if (isYellow != value)
            {
                isYellow = value;
                OnPropertyChanged(nameof(IsYellow));
            }
        }
    }

    protected bool isBrown { get; set; }
    public bool IsBrown
    {
        get => isBrown;
        set
        {
            if (isBrown != value)
            {
                isBrown = value;
                OnPropertyChanged(nameof(IsBrown));
            }
        }
    }

    protected bool isGreen { get; set; }
    public bool IsGreen
    {
        get => isGreen;
        set
        {
            if (isGreen != value)
            {
                isGreen = value;
                OnPropertyChanged(nameof(IsGreen));
            }
        }
    }

    protected bool isGray { get; set; }
    public bool IsGray
    {
        get => isGray;
        set
        {
            if (isGray != value)
            {
                isGray = value;
                OnPropertyChanged(nameof(IsGray));
            }
        }
    }
}
