namespace PsychologyApp.Presentation.Pages.QuoteFeed;

public partial class QuoteViewModel
{
    private bool _showAllReadEmpty;
    public bool ShowAllReadEmpty
    {
        get => _showAllReadEmpty;
        private set => SetProperty(ref _showAllReadEmpty, value);
    }
}
