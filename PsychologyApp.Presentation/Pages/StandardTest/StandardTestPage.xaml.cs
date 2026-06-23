using PsychologyApp.Presentation.Pages.LuscherTest;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.StandardTest;

public partial class StandardTestPage : LuscherTestPageBase
{
    protected override View LuscherStartSection => StartSection;
    protected override View LuscherFinishSection => FinishSection;

    public StandardTestPage(
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
        : base(LuscherMode.Standard, pageViewModelActivator, luscherTestViewModelFactory)
    {
        InitializeComponent();
    }
}
