using PsychologyApp.Presentation.Pages.RunTests.LuscherTest;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTests.AlternativeTest;

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
