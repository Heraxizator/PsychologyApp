using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Shared.Common;

public sealed class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IDialogService _dialogService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IDialogService dialogService)
    {
        _logger = logger;
        _dialogService = dialogService;
    }

    public void Attach(Microsoft.Maui.Controls.Application application)
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

    private void OnDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex)
        {
            return;
        }

        LogAndNotify(ex, "Unhandled domain exception");
    }

    private void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogAndNotify(e.Exception, "Unobserved task exception");
        e.SetObserved();
    }

    private void LogAndNotify(Exception exception, string message)
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

        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await _dialogService.ShowAsync(AppStrings.ErrorTitle, GetUserMessage(exception));
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
            TechniqueNotFoundException => AppStrings.TechniqueNotFound,
            QuotNotFoundException => AppStrings.QuoteNotFound,
            NotFoundException => AppStrings.UnexpectedErrorMessage,
            AppException app => app.Message,
            _ => AppStrings.UnexpectedErrorMessage
        };
}
