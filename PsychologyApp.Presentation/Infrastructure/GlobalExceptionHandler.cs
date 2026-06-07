using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Presentation.Services.Dialogs;

namespace PsychologyApp.Presentation.Infrastructure;

public static class GlobalExceptionHandler
{
    public static void Attach(Microsoft.Maui.Controls.Application application)
    {
        AsyncCommandExtensions.DefaultErrorHandler = ex => LogAndNotify(ex, "Background task failed");
        application.HandlerChanged += OnHandlerChanged;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandled;
        TaskScheduler.UnobservedTaskException += OnUnobservedTask;
    }

    private static void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not Microsoft.Maui.Controls.Application app || app.Handler?.MauiContext is null)
        {
            return;
        }

        app.Dispatcher.Dispatch(() =>
        {
            app.HandlerChanged -= OnHandlerChanged;
        });
    }

    private static void OnDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex)
        {
            return;
        }

        LogAndNotify(ex, "Unhandled domain exception");
    }

    private static void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogAndNotify(e.Exception, "Unobserved task exception");
        e.SetObserved();
    }

    private static void LogAndNotify(Exception exception, string message)
    {
        try
        {
            ILogger logger = MauiServiceProvider.GetRequired<ILoggerFactory>()
                .CreateLogger(typeof(GlobalExceptionHandler));
            logger.LogError(
                exception,
                "{Message}. Type={ExceptionType}",
                message,
                exception.GetType().Name);
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {exception}");
        }

        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                IDialogService dialog = MauiServiceProvider.GetRequired<IDialogService>();
                await dialog.ShowAsync("Ошибка", GetUserMessage(exception));
            }
            catch
            {
                // Avoid recursive failures during startup.
            }
        });
    }

    private static string GetUserMessage(Exception exception) =>
        exception switch
        {
            NotFoundException notFound => notFound.Message,
            AppException app => app.Message,
            _ => "Произошла непредвиденная ошибка. Попробуйте ещё раз."
        };
}
