using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

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
