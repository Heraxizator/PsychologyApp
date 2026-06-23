using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Entities.Quote;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.ProfileUser;

public partial class UserViewModel
{
    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<QuoteItem> Quotes { get; private set; } = [];
    public ObservableCollection<PracticeHistoryItem> PracticeHistory { get; private set; } = [];
}
