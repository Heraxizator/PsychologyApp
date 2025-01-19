using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Cleaner;

public class Audio
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? URL { get; set; }
    public ICommand? ClickCommand { get; set; }
}
