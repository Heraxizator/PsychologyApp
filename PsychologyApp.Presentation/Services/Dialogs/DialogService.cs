using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.Services.Dialogs;

public class DialogService : IDialogService
{
    public Task ShowAsync(string title, string message) =>
        Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert(title, message, AppStrings.Ok);

    public Task<bool> AskAsync(string title, string message, string accept, string cancel) =>
        Microsoft.Maui.Controls.Application.Current!.MainPage!.DisplayAlert(title, message, accept, cancel);
}
