using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public class DialogService(IPageHost pageHost) : IDialogService
{
    public Task ShowAsync(string title, string message)
    {
        Page? page = pageHost.GetActivePage()
            ?? throw new InvalidOperationException("No active page available for dialog.");
        return page.DisplayAlertAsync(title, message, AppStrings.Ok);
    }

    public Task<bool> AskAsync(string title, string message, string accept, string cancel)
    {
        Page? page = pageHost.GetActivePage()
            ?? throw new InvalidOperationException("No active page available for dialog.");
        return page.DisplayAlertAsync(title, message, accept, cancel);
    }
}
