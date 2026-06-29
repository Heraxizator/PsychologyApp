using PsychologyApp.Presentation.Pages.LuscherTest;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.AlternativeTest;

public partial class AlternativeTestPage : LuscherTestPageBase
{
    protected override View LuscherStartSection => StartSection;
    protected override View LuscherFinishSection => FinishSection;

    public AlternativeTestPage(
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
    {
        InitializeComponent();
        InitializeLuscherViewModel(LuscherMode.Brief, pageViewModelActivator, luscherTestViewModelFactory);
    }
}
