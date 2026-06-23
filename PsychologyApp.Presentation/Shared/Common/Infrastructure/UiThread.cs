namespace PsychologyApp.Presentation.Shared.Common;

public static class UiThread
{
    public static Task RunAsync(Action action)
    {
        if (!IsUiContextAvailable())
        {
            action();
            return Task.CompletedTask;
        }

        return MainThread.InvokeOnMainThreadAsync(action);
    }

    public static async Task RunAsync(Func<Task> action)
    {
        if (!IsUiContextAvailable())
        {
            await action();
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(action);
    }

    private static bool IsUiContextAvailable() =>
        Microsoft.Maui.Controls.Application.Current?.Dispatcher is not null;
}
