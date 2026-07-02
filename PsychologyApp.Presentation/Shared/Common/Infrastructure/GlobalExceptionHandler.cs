using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Toasts;

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

        LogAndNotify(ex, "Unhandled domain exception", useDialog: e.IsTerminating);
    }

    private void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogAndNotify(e.Exception, "Unobserved task exception", useDialog: false);
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

                _toastService.ShortToast(userMessage);
            }
            catch
            {
                // Avoid recursive failures during startup or heavy UI passes.
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
