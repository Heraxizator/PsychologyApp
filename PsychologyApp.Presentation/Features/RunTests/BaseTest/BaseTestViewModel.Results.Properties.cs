namespace PsychologyApp.Presentation.Features.RunTests.BaseTest;

public abstract partial class BaseTestViewModel
{
    protected string? currentInstruction { get; set; }
    public string? CurrentInstruction
    {
        get => currentInstruction;
        set
        {
            if (currentInstruction != value)
            {
                currentInstruction = value;
                OnPropertyChanged(nameof(CurrentInstruction));
            }
        }
    }

    protected string? firstResult { get; set; }
    public string? FirstResult
    {
        get => firstResult;
        set
        {
            if (firstResult != value)
            {
                firstResult = value;
                OnPropertyChanged(nameof(FirstResult));
            }
        }
    }

    protected string? secondResult { get; set; }
    public string? SecondResult
    {
        get => secondResult;
        set
        {
            if (secondResult != value)
            {
                secondResult = value;
                OnPropertyChanged(nameof(SecondResult));
            }
        }
    }

    protected string? firstName { get; set; }
    public string? FirstName
    {
        get => firstName;
        set
        {
            if (firstName != value)
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
    }

    protected string? secondName { get; set; }
    public string? SecondName
    {
        get => secondName;
        set
        {
            if (secondName != value)
            {
                secondName = value;
                OnPropertyChanged(nameof(SecondName));
            }
        }
    }

    protected bool isStart { get; set; }
    public bool IsStart
    {
        get => isStart;
        set
        {
            if (isStart != value)
            {
                isStart = value;
                OnPropertyChanged(nameof(IsStart));
            }
        }
    }

    protected bool isFinish { get; set; }
    public bool IsFinish
    {
        get => isFinish;
        set
        {
            if (isFinish != value)
            {
                isFinish = value;
                OnPropertyChanged(nameof(IsFinish));
            }
        }
    }
}
