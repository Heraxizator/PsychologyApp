using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Common;

public sealed class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IDialogService _dialogService;
    private readonly IToastService _toastService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IDialogService dialogService,
        IToastService toastService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _toastService = toastService;
    }

    public void Attach(Microsoft.Maui.Controls.Application application)
    {
        AsyncCommandExtensions.DefaultErrorHandler = ex => LogAndNotify(ex, "Background task failed", useDialog: false);
        AsyncCommandExtensions.DefaultErrorLogger = (ex, origin) => _logger.LogWarning(ex, "Background task started in {Origin} failed", origin);
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

    private void OnDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex)
        {
            return;
        }

        if (e.IsTerminating)
        {
            // The process is ending: a dialog would never be seen and the queued logger may not be drained, so write the crash straight to disk.
            DebugFileLoggerProvider.AppendNow(
                DebugFileLoggerProvider.ErrorLogPath,
                $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}] CRASH Unhandled domain exception: {ex}");
            return;
        }

        LogAndNotify(ex, "Unhandled domain exception", useDialog: false);
    }

    private void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        // Nobody awaited this task, so the person cannot act on it and a red toast for it is only noise (cancellations, dropped connections).
        // It is logged; the task is marked observed so it cannot take the process down.
        try
        {
            _logger.LogWarning(e.Exception.Flatten(), "Unobserved task exception");
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine(e.Exception);
        }

        e.SetObserved();
    }

    private void LogAndNotify(Exception exception, string message, bool useDialog)
    {
        try
        {
            _logger.LogError(
                exception,
                "{Message}. Type={ExceptionType}",
                message,
                exception.GetType().Name);
        }
        catch
        {
            System.Diagnostics.Debug.WriteLine($"{message}: {exception}");
        }

        if (IsExpectedNoise(exception))
        {
            return;
        }

        string userMessage = GetUserMessage(exception);
        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                if (useDialog)
                {
                    await Task.Yield();
                    await _dialogService.ShowAsync(AppStrings.ErrorTitle, userMessage);
                    return;
                }

                _toastService.ShortToast(userMessage, AppToastKind.Error);
            }
            catch
            {
                // Avoid recursive failures during startup or heavy UI passes.
            }
        });
    }

    /// <summary>A cancelled operation is not a failure, and a dropped connection is not something the person caused or can fix by being told.</summary>
    private static bool IsExpectedNoise(Exception exception) =>
        exception is OperationCanceledException or TaskCanceledException
        || exception is AggregateException aggregate && aggregate.Flatten().InnerExceptions.All(IsExpectedNoise);

    // Only messages written for people (localized) are shown; an exception's own text is technical, English and often about SQL or entities.
    private static string GetUserMessage(Exception exception) =>
        exception switch
        {
            TechniqueNotFoundException => AppStrings.TechniqueNotFound,
            QuotNotFoundException => AppStrings.QuoteNotFound,
            NotFoundException => AppStrings.UnexpectedErrorMessage,
            _ => AppStrings.UnexpectedErrorMessage
        };
}
