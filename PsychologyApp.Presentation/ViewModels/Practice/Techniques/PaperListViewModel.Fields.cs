namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class PaperListViewModel
{
    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }
}
