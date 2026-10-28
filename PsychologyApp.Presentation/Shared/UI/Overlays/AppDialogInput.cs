namespace PsychologyApp.Presentation.Shared.UI.Overlays;

/// <summary>The field of a dialog that asks for text: a name to edit, or a secret that is masked.</summary>
public sealed record AppDialogInput(string Placeholder, bool IsPassword, string InitialText = "", int MaxLength = int.MaxValue);
