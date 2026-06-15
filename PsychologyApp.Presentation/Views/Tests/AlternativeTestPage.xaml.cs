using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class AlternativeTestPage : LuscherTestPageBase
{
    protected override View LuscherStartSection => StartSection;
    protected override View LuscherFinishSection => FinishSection;

    public AlternativeTestPage(
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
        : base(LuscherMode.Brief, pageViewModelActivator, luscherTestViewModelFactory)
    {
        InitializeComponent();
    }
}
