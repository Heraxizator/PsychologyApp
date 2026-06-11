using System.Windows.Input;

namespace PsychologyApp.Presentation.Models.Clean;

public class Audio
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? URL { get; set; }
    public ICommand? ClickCommand { get; set; }
}
