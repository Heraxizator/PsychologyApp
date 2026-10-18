using System.Windows.Input;

namespace PsychologyApp.Presentation.App;

/// <summary>One row of the burger menu: a Material icon name, the label and what a tap opens.</summary>
public sealed record AppMenuEntry(string Icon, string Title, ICommand Command);
