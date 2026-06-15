using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Constructor;

public partial class CreatedViewModel
{
    public ICommand BackCommand { get; private set; } = default!;
    public ICommand Remove { get; private set; } = default!;
    public ICommand Edit { get; private set; } = default!;

    private void WireCommands()
    {
        BackCommand = new AsyncCommand(GoBackAsync);
        Finish = new AsyncCommand(CompleteSessionAsync);
        Remove = new AsyncCommand(() => ToRemoveAsync());
        Edit = new AsyncCommand(() => ToEditAsync());
    }
}
