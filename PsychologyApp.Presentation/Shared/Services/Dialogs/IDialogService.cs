namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public interface IDialogService
{
    Task ShowAsync(string? title, string message);
    Task<bool> AskAsync(string? title, string message, string accept, string cancel);
    Task<string?> PickOptionAsync(string title, IReadOnlyList<string> options, string cancel);

    /// <summary>Asks for a line of text in the app's own dialog, filled with <paramref name="initialText"/>; null when cancelled or when
    /// another dialog is on screen. An accepted empty field is an empty string.</summary>
    Task<string?> PromptTextAsync(string? title, string message, string accept, string cancel, string initialText, int maxLength);

    /// <summary>Asks for a secret (masked); null when cancelled or when another dialog is on screen.</summary>
    Task<string?> PromptPasswordAsync(string? title, string message, string placeholder, string accept, string cancel);
}
