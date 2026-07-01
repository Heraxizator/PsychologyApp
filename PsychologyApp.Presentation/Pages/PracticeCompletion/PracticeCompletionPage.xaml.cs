using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.PracticeCompletion;

public partial class PracticeCompletionPage : ContentPage
{
    public PracticeCompletionPage(IPracticeCompletionViewModelFactory factory, int streakDays)
    {
        BindingContext = factory.Create(this, streakDays);
        InitializeComponent();
    }
}
