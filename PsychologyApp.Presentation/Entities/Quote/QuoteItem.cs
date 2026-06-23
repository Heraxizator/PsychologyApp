using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Quote;

public class QuoteItem
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public bool IsReaded { get; set; }
    public bool IsFavourite { get; set; }
    public ICommand? LikeCommand { get; set; }
    public ICommand? ShareCommand { get; set; }
    public ICommand? CopyCommand { get; set; }
    public ICommand? OpenQuotesTabCommand { get; set; }
}
