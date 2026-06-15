using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Profile;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class UserViewModel
{
    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<QuoteItem> Quotes { get; private set; } = [];
    public ObservableCollection<PracticeHistoryItem> PracticeHistory { get; private set; } = [];
}
