using PsychologyApp.Presentation.Pages.RunTests.LuscherTest;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTests.StandardTest;

public partial class StandardTestPage : LuscherTestPageBase
{
    protected override View LuscherStartSection => StartSection;
    protected override View LuscherFinishSection => FinishSection;

    public StandardTestPage(
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
    {
        InitializeComponent();
        InitializeLuscherViewModel(LuscherMode.Standard, pageViewModelActivator, luscherTestViewModelFactory);
    }
}
