using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfilePracticeHistory;

public partial class PracticeHistoryPage : ContentPage
{
    public PracticeHistoryPage(IPracticeHistoryViewModelFactory practiceHistoryViewModelFactory)
    {
        InitializeComponent();
        BindingContext = practiceHistoryViewModelFactory.Create(this);
    }
}
