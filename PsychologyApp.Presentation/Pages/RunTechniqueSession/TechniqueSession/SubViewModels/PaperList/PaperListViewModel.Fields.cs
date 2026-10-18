namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.PaperList;

public partial class PaperListViewModel
{
    private string text = string.Empty;
    public string Text
    {
        get => text;
        set
        {
            if (SetProperty(ref text, value))
            {
                OnPropertyChanged(nameof(StageText));
            }
        }
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }

    /// <summary>The words on the sheet of the paper stage: what is being written now, otherwise the last thought added.</summary>
    public string StageText => !string.IsNullOrWhiteSpace(Text)
        ? Text
        : PapersObservableCollection.LastOrDefault()?.Text ?? string.Empty;

    /// <summary>How many times the phrase has been said out loud (each repeat is one entry).</summary>
    public int RepeatCount => PapersObservableCollection.Count;
}
