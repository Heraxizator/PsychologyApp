namespace PsychologyApp.Presentation.Services.Dialogs;

public interface IDialogService
{
    Task ShowAsync(string title, string message);
    Task<bool> AskAsync(string title, string message, string accept, string cancel);
}
